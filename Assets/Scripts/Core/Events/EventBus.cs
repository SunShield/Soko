using System;
using System.Collections.Generic;

namespace Soko.Core.Events
{
    public class EventBus
    {
        private readonly Dictionary<Type, IGameEvent> _typesToEventsMap = new ();

        public TEventType GetEvent<TEventType>()
            where TEventType : IGameEvent, new()
        {
            var type = typeof(TEventType);

            if (_typesToEventsMap.TryGetValue(type, out var value)) return (TEventType)value;
            
            var newEvent = new TEventType();
            _typesToEventsMap.Add(type, newEvent);

            return (TEventType)_typesToEventsMap[type];
        }
    }
}