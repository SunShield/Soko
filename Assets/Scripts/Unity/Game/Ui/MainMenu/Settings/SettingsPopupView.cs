using System;
using UnityEngine;
using UnityEngine.UI;

namespace Soko.Unity.Game.Ui.MainMenu.Settings
{
    public class SettingsPopupView : MonoBehaviour
    {
        [SerializeField] private Toggle _musicButton;
        [SerializeField] private Toggle _sfxButton;
        [SerializeField] private Button _exitButton;

        private void Awake()
        {
            _musicButton.onValueChanged.AddListener(MusicToggleClickHandler);
            _sfxButton.onValueChanged.AddListener(SfxToggleClickHandler);
            _exitButton.onClick.AddListener(ExitButtonClickHandler);
        }

        public void Setup(bool music, bool sfx)
        {
            _musicButton.SetIsOnWithoutNotify(music);
            _sfxButton.SetIsOnWithoutNotify(sfx);
        }
        
        private void MusicToggleClickHandler(bool value) => OnMusicToggleClick?.Invoke(value);
        private void SfxToggleClickHandler(bool value) => OnSfxToggleClick?.Invoke(value);
        private void ExitButtonClickHandler() => OnExitButtonClick?.Invoke();
        
        public event Action<bool> OnMusicToggleClick;
        public event Action<bool> OnSfxToggleClick;
        public event Action OnExitButtonClick;
    }
}