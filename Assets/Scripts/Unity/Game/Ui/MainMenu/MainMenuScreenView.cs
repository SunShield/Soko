using System;
using UnityEngine;
using UnityEngine.UI;

namespace Soko.Unity.Game.Ui.MainMenu
{
    public class MainMenuScreenView : MonoBehaviour
    {
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;

        private void Awake()
        {
            _newGameButton.onClick.AddListener(NewGameClickHandler);
            _settingsButton.onClick.AddListener(SettingsClickHandler);
            _exitButton.onClick.AddListener(ExitClickHandler);
        }
        
        private void NewGameClickHandler() => OnNewGameClicked?.Invoke();
        private void SettingsClickHandler() => OnSettingsClicked?.Invoke();
        private void ExitClickHandler() => OnExitClicked?.Invoke();
        
        public event Action OnNewGameClicked;
        public event Action OnSettingsClicked;
        public event Action OnExitClicked;
    }
}