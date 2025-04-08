using Cysharp.Threading.Tasks;

namespace Soko.Unity.Game.Ui.Management.Elements
{
    public abstract class AwaitableUiElement<TResult> : UiElement
    {
        protected UniTaskCompletionSource<TResult> CompletionSource { get; private set; } = new();

        protected override async UniTask OnEnabledAndConstructed()
        {
            CompletionSource = new();
        }

        public async UniTask<TResult> AwaitForResult()
        {
            var result = await CompletionSource.Task;
            Close();
            return result;
        }
    }
}