using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;

namespace Soko.Unity.Game.Ui.Special.Focus
{
    public class FocusObjectScreenController : AwaitableUiElement<DefautResult>, 
        IConfigurableUiElement<FocusData>
    {
        [SerializeField] private FocusObjectScreenView _view;
        
        public async void Configure(FocusData data)
        {
            var canvas = Container.Canvas;
            var objectScreenPos = Camera.main.WorldToScreenPoint(data.FocusedObject.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                objectScreenPos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out var uiPos
            );
            
            await _view.FocusPoint(uiPos, data.FinalFocusSize);
            CompletionSource.TrySetResult(new());
        }
    }
}