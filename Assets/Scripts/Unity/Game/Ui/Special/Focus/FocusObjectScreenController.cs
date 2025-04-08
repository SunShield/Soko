using Cysharp.Threading.Tasks;
using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;

namespace Soko.Unity.Game.Ui.Special.Focus
{
    public class FocusObjectScreenController : UiElement
    {
        [SerializeField] private FocusObjectScreenView _view;

        public async UniTask Setup(GameObject objectToFocus, int finalFocusSize)
        {
            var canvas = Container.Canvas;
            var objectScreenPos = Camera.main.WorldToScreenPoint(objectToFocus.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                objectScreenPos,
                canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
                out var uiPos
            );
            
            await _view.FocusPoint(uiPos, finalFocusSize);
        }
    }
}