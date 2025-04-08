using DG.Tweening;
using UnityEngine;

namespace Soko.Unity.Game.Ui.Special.Focus
{
    public class FocusObjectScreenView : MonoBehaviour
    {
        private const int DefaultFocusSize = 2500;
        private const float DefaultFocusTime = 0.2f;
        
        [SerializeField] private RectTransform _focusGraphics;

        public void FocusPoint(Vector3 pointToFocus, int focusSize)
        {
            _focusGraphics.position = pointToFocus;
            _focusGraphics.sizeDelta = new Vector2(DefaultFocusSize, DefaultFocusSize);
            _focusGraphics.DOSizeDelta(new Vector2(focusSize, focusSize), DefaultFocusTime);
        }
    }
}