using System;
using System.Collections.Generic;
using System.Linq;
using Soko.Core.Events;
using Soko.Core.Events.Impl.Args;
using Soko.Core.Events.Impl.Events;
using Soko.Unity.Game.Level.Cycle;
using Soko.Unity.Game.Level.Grid;
using Soko.Unity.Game.Level.Grid.Objects;
using Soko.Unity.Game.Level.History.Imprints;
using Soko.Unity.Game.Level.History.Interfaces;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.Level.History
{
    public class HistoryManager : IInitializable
    {
        [Inject] private LevelPlayCycleManager _cycleManager;
        [Inject] private EventBus _eventBus;
        
        private readonly HashSet<GridCoords> _affectedCells = new ();
        private readonly List<TurnImprint> _turnImprints = new ();
        
        public int TurnsPassed => _turnImprints.Count - 1;

        public void Initialize()
        {
            _eventBus.GetEvent<MovementStatedEvent>().SubscribeForGlobal(CreateZeroTurnImprintIfNeeded);
            _eventBus.GetEvent<ObjectMovedEvent>().SubscribeForGlobal(RecordObjectMovement);
            _eventBus.GetEvent<MovementFinishedEvent>().SubscribeForGlobal(CreateTurnImprint);
        }

        private void CreateZeroTurnImprintIfNeeded(EmptyArgs args)
        {
            if (_turnImprints.Count > 0) return;
            
            var turnImprint = new TurnImprint();
            var grid = _cycleManager.LevelGrid;
            for (int y = 0; y < grid.Rows; y++)
            {
                for (int x = 0; x < grid.Columns; x++)
                {
                    var cell = grid[y, x];
                    var cellImprint = CreateCellImprint(cell);
                    turnImprint.CellImprints.Add(cell.Coords, cellImprint);
                }
            }
            
            _turnImprints.Add(turnImprint);
        }

        private void RecordObjectMovement(ObjectMoveArgs args)
        {
            _affectedCells.Add(args.StartCell.Coords);
            _affectedCells.Add(args.MovedObject.Position);
        }

        private void CreateTurnImprint(EmptyArgs args)
        {
            var turnImprint = new TurnImprint();
            var grid = _cycleManager.LevelGrid;
            foreach (var affectedCellCoords in _affectedCells)
            {
                var cell = grid[affectedCellCoords];
                turnImprint.CellImprints.Add(affectedCellCoords, CreateCellImprint(cell));
            }
            _turnImprints.Add(turnImprint);
            _affectedCells.Clear();
        }

        private CellImprint CreateCellImprint(LevelGridCell cell)
        {
            var imprint = new CellImprint
            {
                GroundObjectImprint = CreateObjectImprint(cell, ObjectLayer.Ground),
                SolidObjectImprint = CreateObjectImprint(cell, ObjectLayer.Solid)
            };
            var previousCellImprint = GetPreviousCellImprint(cell.Coords);
            imprint.PreviousImprint = previousCellImprint;
            return imprint;
        }

        private CellImprint GetPreviousCellImprint(GridCoords gridCoords)
        {
            var index = _turnImprints.Count - 1;
            while (index >= 0)
            {
                var turnImprint = _turnImprints[index];
                if (turnImprint.CellImprints.TryGetValue(gridCoords, out var previousCellImprint)) 
                    return previousCellImprint;
                index--;
            }

            return null;
        }

        private ObjectImprint CreateObjectImprint(LevelGridCell cell, ObjectLayer layer)
        {
            if (!cell.Objects.TryGetValue(layer, out var levelObject)) return null;
            if (levelObject == null) return null;
            
            var imprint = new ObjectImprint
            {
                Cell = levelObject.Cell,
                ComponentImprints = CreateComponentImprints(levelObject)
            };

            return imprint;
        }

        private Dictionary<IImprintableComponent, ComponentImprint> CreateComponentImprints(LevelObjectBase levelObject)
        {
            var imprintableComponents = levelObject.Components
                .Where(c => c is IImprintableComponent)
                .ToList();
            if (imprintableComponents.Count == 0) return new();
            
            var componentImprints = imprintableComponents
                .Cast<IImprintableComponent>()
                .Select(c => (c, c.CreateComponentImprint()))
                .ToDictionary(c => c.Item1, c => c.Item2);
            return componentImprints;
        }

        public void RevertTime()
        {   
            var currentTurnImprint = _turnImprints[^1];
            _turnImprints.Remove(currentTurnImprint);
            var previousImprint = _turnImprints[^1];
            RevertTurnImprint(currentTurnImprint, previousImprint);
        }

        private void RevertTurnImprint(TurnImprint currentTurnImprint, TurnImprint previousTurnImprint)
        {
            
        }
    }
}