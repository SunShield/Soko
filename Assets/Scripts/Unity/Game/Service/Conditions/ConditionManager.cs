using System.Collections.Generic;
using VContainer;

namespace Soko.Unity.Game.Service.Conditions
{
    public class ConditionManager
    {
        [Inject] private IObjectResolver _objectResolver;
        
        private readonly HashSet<AbstractCondition> _injectedConditions = new ();
        
        public bool CheckCondition(AbstractCondition condition)
        {
            InjectConditionIfNeeded(condition);

            return condition.Check();
        }

        private void InjectConditionIfNeeded(AbstractCondition condition)
        {
            if (_injectedConditions.Contains(condition)) return;
            
            _objectResolver.Inject(condition);
            _injectedConditions.Add(condition);
        }
    }
}