using System;
using UnityEngine;
using UnityEngine.UI;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelSelectScreenView : MonoBehaviour
    {
        [SerializeField] private Button _previousLevelPackButton;
        [SerializeField] private Button _nextLevelPackButton;
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _backButton;

        private void Awake()
        {
            _previousLevelPackButton.onClick.AddListener(PreviousLevelPackClickHandler);
            _nextLevelPackButton.onClick.AddListener(NextLevelPackClickHandler);
            _startButton.onClick.AddListener(StartClickHandler);
            _backButton.onClick.AddListener(BackClickHandler);
        }

        public void SetLevelPackButtonsState(bool isFirstPack, bool isLastPack)
        {
            _previousLevelPackButton.gameObject.SetActive(!isFirstPack);
            _nextLevelPackButton.gameObject.SetActive(!isLastPack);
        }
        
        private void PreviousLevelPackClickHandler() => OnPreviousLevelPackClicked?.Invoke();
        private void NextLevelPackClickHandler() => OnNextLevelPackClicked?.Invoke();
        private void StartClickHandler() => OnStartClicked?.Invoke();
        private void BackClickHandler() => OnBackClicked?.Invoke();
        
        public event Action OnPreviousLevelPackClicked;
        public event Action OnNextLevelPackClicked;
        public event Action OnStartClicked;
        public event Action OnBackClicked;
    }
}