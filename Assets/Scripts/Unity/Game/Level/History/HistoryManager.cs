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
        private readonly Stack<TurnImprint> _turnImprints = new ();
        
        public int TurnsPassed => _turnImprints.Count - 1;

        public void Initialize()
        {
            _eventBus.GetEvent<ObjectMovedEvent>().SubscribeForGlobal(RecordObjectMovement);
            _eventBus.GetEvent<MovementFinishedEvent>().SubscribeForGlobal(CreateTurnImprint);
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
            _turnImprints.Push(turnImprint);
            _affectedCells.Clear();
        }

        private CellImprint CreateCellImprint(LevelGridCell cell)
        {
            var imprint = new CellImprint
            {
                GroundObjectImprint = CreateObjectImprint(cell, ObjectLayer.Ground),
                SolidObjectImprint = CreateObjectImprint(cell, ObjectLayer.Solid)
            };
            return imprint;
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
            _turnImprints.Pop();
            var turnImprint = _turnImprints.Peek();
            ApplyTurnImprint(turnImprint);
        }

        private void ApplyTurnImprint(TurnImprint turnImprint)
        {
            
        }
    }
}