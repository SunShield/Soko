using Cysharp.Threading.Tasks;
using Soko.Unity.Game.Sounds;
using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Ui.MainMenu.Settings
{
    public class SettingsPopupController : UiElement
    {
        [SerializeField] private SettingsPopupView _view;
        
        [Inject] private SoundsManager _soundsManager;

        private void Awake()
        {
            _view.OnMusicToggleClick += ToggleMusic;
            _view.OnSfxToggleClick += ToggleSfx;
            _view.OnExitButtonClick += Close;
        }

        protected override async UniTask OnEnabledAndConstructed()
            => _view.Setup(_soundsManager.MusicOn, _soundsManager.SfxOn);

        private void ToggleMusic(bool isOn) => _soundsManager.SetMusicOn(isOn);
        private void ToggleSfx(bool isOn) => _soundsManager.SetSfxOn(isOn);
    }
}