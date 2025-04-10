using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Soko.Unity.Game.Tutorials.Actions
{
    public class TutorialActionExecutor
    {
        [Inject] private IObjectResolver _objectResolver;
        
        private readonly HashSet<TutorialAction> _injectedActions = new ();

        public async UniTask ExecuteAction(TutorialAction action)
        {
            InjectActionIfNeeded(action);
            await action.Execute();
        }

        private void InjectActionIfNeeded(TutorialAction action)
        {
            if (!_injectedActions.Contains(action)) _objectResolver.Inject(action);
            _injectedActions.Add(action);
        }
    }
}