using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Soko.Unity.Game.Ui.Special.Info
{
    public class DefaultInfoPopupView : MonoBehaviour
    {
        [SerializeField] private RectTransform _bottomPoint;
        [SerializeField] private RectTransform _popupArea;
        [SerializeField] private Button _closeButton;
        [SerializeField] private float _appearTime = 0.25f;
        
        private Vector3 _defaultPos;
        private Sequence _appearSequence;

        private void Awake()
        {
            _defaultPos = _popupArea.position;
            
            _closeButton.onClick.AddListener(ExitButtonClickHandler);
        }

        private void OnEnable()
        {
            PlayAppearSequence();
        }

        private void PlayAppearSequence()
        {
            _appearSequence = DOTween.Sequence();
            _appearSequence.AppendCallback(() =>
            {
                _popupArea.position = _bottomPoint.position;
                _popupArea.localScale = Vector3.zero;
            });
            _appearSequence.Join(_popupArea.DOScale(Vector3.one, _appearTime).SetEase(Ease.OutBounce));
            _appearSequence.Join(_popupArea.DOMove(_defaultPos, _appearTime).SetEase(Ease.OutBounce));
            _appearSequence.Play();
        }

        private void OnDisable()
        {
            _popupArea.position = _defaultPos;
            _popupArea.localScale = Vector3.one;
            _appearSequence.Kill();
            _appearSequence = null;
        }
        
        private void ExitButtonClickHandler() => OnExitButtonClicked?.Invoke();

        public event Action OnExitButtonClicked;
    }
}