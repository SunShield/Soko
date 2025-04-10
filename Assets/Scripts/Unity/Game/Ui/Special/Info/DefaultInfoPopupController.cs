using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;

namespace Soko.Unity.Game.Ui.Special.Info
{
    public class DefaultInfoPopupController : AwaitableUiElement<DefautResult>
    {
        [SerializeField] private DefaultInfoPopupView _view;

        private void Awake() => _view.OnExitButtonClicked += OnExitButtonClicked;
        private void OnExitButtonClicked() => CompletionSource.TrySetResult(new());
    }
}