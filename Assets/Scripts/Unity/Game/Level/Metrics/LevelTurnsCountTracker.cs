using System;
using Soko.Core.Events;
using Soko.Core.Events.Impl.Args;
using Soko.Core.Events.Impl.Events;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.Level.Metrics
{
    public class LevelTurnsCountTracker : IInitializable
    {
        [Inject] private EventBus _eventBus;
        
        public int TurnCount { get; private set; }
        
        public void Initialize()
        {
            _eventBus.GetEvent<MovementFinishedEvent>().SubscribeForGlobal(AdvanceTurnCount);
            _eventBus.GetEvent<TurnRevertedEvent>().SubscribeForGlobal(RetreatTurnCount);
        }
        
        private void AdvanceTurnCount(EmptyArgs args)
        {
            TurnCount++;
            OnTurnCountChanged?.Invoke(TurnCount);
        }
        
        private void RetreatTurnCount(EmptyArgs args)
        {
            TurnCount--;
            OnTurnCountChanged?.Invoke(TurnCount);
        }
        
        public event Action<int> OnTurnCountChanged;
    }
}