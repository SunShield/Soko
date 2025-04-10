using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Soko.Unity.Game.Ui.Special.Focus
{
    public class FocusObjectScreenView : MonoBehaviour
    {
        private const int DefaultFocusSize = 2500;
        private const float DefaultFocusTime = 0.2f;
        
        [SerializeField] private RectTransform _focusGraphics;
        [SerializeField] private Button _exitButton;

        private Tween _focusTween;

        private void Awake()
        {
            _exitButton.onClick.AddListener(ExitButtonClickHandler);
        }

        public async UniTask FocusPoint(Vector3 pointToFocus, int focusSize)
        {
            _focusGraphics.localPosition = pointToFocus;
            _focusGraphics.sizeDelta = new Vector2(DefaultFocusSize, DefaultFocusSize);
            _focusTween = _focusGraphics
                .DOSizeDelta(new Vector2(focusSize, focusSize), DefaultFocusTime);
            await _focusTween.AsyncWaitForCompletion();
        }
        
        private void ExitButtonClickHandler() => OnExitButtonClicked?.Invoke();

        private void OnDisable()
        {
            _focusTween?.Kill();
            _focusTween = null;
        }

        public event Action OnExitButtonClicked;
    }
}