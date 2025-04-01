using System;
using Soko.Unity.Game.Ui.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Soko.Unity.Game.Ui.Level
{
    public class LevelScreenView : MonoBehaviour
    {
        [SerializeField] private Button _backButton;
        [SerializeField] private HoldableButton _revertTurnButton;

        private void Awake()
        {
            _backButton.onClick.AddListener(BackButtonClickHandler);
            _revertTurnButton.OnClickStart += RevertTurnButtonClickStartHandler;
            _revertTurnButton.OnClickRelease += RevertTurnButtonClickReleaseHandler;
        }
        
        private void BackButtonClickHandler() => OnBackClicked?.Invoke();
        private void RevertTurnButtonClickStartHandler() => OnRevertTurnClickStarted?.Invoke();
        private void RevertTurnButtonClickReleaseHandler() => OnRevertTurnClickReleased?.Invoke();
        
        public event Action OnBackClicked;
        public event Action OnRevertTurnClickStarted;
        public event Action OnRevertTurnClickReleased;
        
    }
}