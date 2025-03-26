using System.Collections.Generic;
using System.Linq;
using Soko.Core.Events;
using Soko.Core.Events.Impl.Args;
using Soko.Core.Events.Impl.Events;
using Soko.Unity.Game.Level.Cycle;
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
        
        private readonly HashSet<LevelObjectBase> _imprintableObjects = new();
        private readonly List<TurnImprint> _turnImprints = new ();

        public void Initialize()
        {
            _eventBus.GetEvent<MovementStatedEvent>().SubscribeForGlobal(CreateZeroTurnImprintIfNeeded);
            _eventBus.GetEvent<MovementFinishedEvent>().SubscribeForGlobal(CreateTurnImprint);
        }

        private void CreateZeroTurnImprintIfNeeded(EmptyArgs args)
        {
            if (_turnImprints.Count > 0) return;
            
            GatherImprintableObjects();
            CreateTurnImprint(null);
        }

        private void GatherImprintableObjects()
        {
            foreach (var levelObject in _cycleManager.LevelGrid.LevelObjects)
            {
                if (!levelObject.Components.Any(c => c is IImprintableComponent)) continue;
                
                _imprintableObjects.Add(levelObject);
            }
        }

        private void CreateTurnImprint(EmptyArgs args)
        {
            var turnImprint = new TurnImprint();
            var grid = _cycleManager.LevelGrid;
            foreach (var levelObject in _imprintableObjects)
            {
                var objectImprint = CreateObjectImprint(levelObject);
                turnImprint.ObjectImprints.Add(objectImprint);
            }
            
            _turnImprints.Add(turnImprint);
        }

        private ObjectImprint CreateObjectImprint(LevelObjectBase levelObject)
        {
            var imprint = new ObjectImprint
            {
                Cell = levelObject.Cell,
                Object = levelObject,
                ComponentImprints = CreateComponentImprints(levelObject)
            };

            return imprint;
        }

        private List<ComponentImprint> CreateComponentImprints(LevelObjectBase levelObject)
        {
            var imprintableComponents = levelObject.Components
                .Where(c => c is IImprintableComponent)
                .ToList();
            if (imprintableComponents.Count == 0) return new();
            
            var componentImprints = imprintableComponents
                .Cast<IImprintableComponent>()
                .Select(c =>
                {
                    var imprint = c.CreateComponentImprint();
                    imprint.Component = c;
                    return imprint;
                })
                .ToList();
            return componentImprints;
        }

        public void RevertTurn()
        {   
            if (_turnImprints.Count <= 1) return;
            
            var currentTurnImprint = _turnImprints[^1];
            _turnImprints.Remove(currentTurnImprint);
            var previousImprint = _turnImprints[^1];
            RevertTurnImprint(currentTurnImprint, previousImprint);
        }

        private void RevertTurnImprint(TurnImprint currentTurnImprint, TurnImprint previousTurnImprint)
        {
            foreach (var objectImprint in currentTurnImprint.ObjectImprints)
            {
                objectImprint.Object.Cell.BaseRemoveObject(objectImprint.Object);
            }

            foreach (var objectImprint in previousTurnImprint.ObjectImprints)
            {
                objectImprint.Cell.BaseAddObject(objectImprint.Object);
            }

            foreach (var objectImprint in previousTurnImprint.ObjectImprints)
            {
                var levelObject = objectImprint.Object;
                var componentImprints = objectImprint.ComponentImprints;
                foreach (var componentImprint in componentImprints)
                {
                    componentImprint.Component.RestoreFromImprint(componentImprint);
                }
            }
        }
    }
}