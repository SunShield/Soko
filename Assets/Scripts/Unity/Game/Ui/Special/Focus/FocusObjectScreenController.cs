using Cysharp.Threading.Tasks;
using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;

namespace Soko.Unity.Game.Ui.Special.Focus
{
    public class FocusObjectScreenController : AwaitableUiElement<DefautResult>, 
        IConfigurableUiElement<FocusData>
    {
        private const int NoCloseTime = -1;
        
        [SerializeField] private FocusObjectScreenView _view;

        private void Awake()
        {
            _view.OnExitButtonClicked += OnExitButtonClicked;
        }

        private void OnExitButtonClicked()
        {
            CompletionSource.TrySetResult(new());
        }

        public async void Configure(FocusData data)
        {
            var uiPos = GetFocusPoint(data);
            await AnimateFocus(data, uiPos);
            
            if (data.AutoCloseTime == NoCloseTime) return;
            
            await WaitForAutoClose(data);
        }

        private Vector2 GetFocusPoint(FocusData data)
        {
            var canvas = Container.Canvas;
            var objectScreenPos = Camera.main.WorldToScreenPoint(data.FocusedObject.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                objectScreenPos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out var uiPos
            );
            return uiPos;
        }

        private UniTask AnimateFocus(FocusData data, Vector2 uiPos)
            => _view.FocusPoint(uiPos, data.FinalFocusSize);

        private async UniTask WaitForAutoClose(FocusData data)
        {
            await UniTask.WaitForSeconds((float)data.AutoCloseTime);
            CompletionSource.TrySetResult(new());
        }
    }
}