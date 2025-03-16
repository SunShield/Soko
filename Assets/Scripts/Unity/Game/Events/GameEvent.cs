using System;
using System.Collections.Generic;

namespace Soko.Unity.Game.Events
{
    public class GameEvent<TArgs> : IGameEvent
        where TArgs : IGameEventArgs
    {
        private readonly HashSet<Action<TArgs>> _globalSubscribedActions = new ();
        private readonly HashSet<Action<TArgs>> _subscriptionsDuringInvoke = new ();
        private readonly HashSet<Action<TArgs>> _unsubscriptionsDuringInvoke = new ();
        private bool _isInvokingNow;

        public void SubscribeForGlobal(Action<TArgs> action)
        {
            if (!_isInvokingNow)
                _globalSubscribedActions.Add(action);
            else
                _subscriptionsDuringInvoke.Add(action);
        }

        public void UnsubscribeFromGlobal(Action<TArgs> action)
        {
            if (!_isInvokingNow)
                _globalSubscribedActions.Remove(action);
            else
                _unsubscriptionsDuringInvoke.Add(action);
        }

        public void InvokeForGlobal(TArgs args)
        {
            _isInvokingNow = true;

            foreach(var subscribedAction in _globalSubscribedActions)
            {
                if(!_unsubscriptionsDuringInvoke.Contains(subscribedAction)) 
                    subscribedAction.Invoke(args);
            }

            foreach (var subscribedAction in _subscriptionsDuringInvoke)
            {
                if(!_unsubscriptionsDuringInvoke.Contains(subscribedAction)) 
                    subscribedAction.Invoke(args);
            }

            _isInvokingNow = false;
            OnFinishInvoke();
        }

        private void OnFinishInvoke()
        {
            RemoveUnsubscribedDuringInvokeActions();
            AddSubscribedDuringInvokeActions();
        }

        private void RemoveUnsubscribedDuringInvokeActions()
        {
            foreach (var action in _unsubscriptionsDuringInvoke)
            {
                _globalSubscribedActions.Remove(action);
            }

            _unsubscriptionsDuringInvoke.Clear();
        }

        private void AddSubscribedDuringInvokeActions()
        {
            foreach (var action in _subscriptionsDuringInvoke)
            {
                _globalSubscribedActions.Add(action);
            }

            _subscriptionsDuringInvoke.Clear();
        }
    }
}