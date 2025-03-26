using System;
using UnityEngine;
using UnityEngine.UI;

namespace Soko.Unity.Game.Ui.Level
{
    public class LevelScreenView : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _revertTurnBiButton;

        private void Awake()
        {
            _backButton.onClick.AddListener(BackButtonClickHandler);
            _revertTurnBiButton.onClick.AddListener(RevertTurnButtonClickHandler);
        }
        
        private void BackButtonClickHandler() => OnBackClicked?.Invoke();
        private void RevertTurnButtonClickHandler() => OnRevertTurnClicked?.Invoke();
        
        public event Action OnBackClicked;
        public event Action OnRevertTurnClicked;
    }
}