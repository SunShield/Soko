using Soko.Unity.Game.Sounds;
using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Soko.Unity.Game.Ui.Utils
{
    public class ButtonSoundPlayer : UiElement
    {
        [SerializeField] private Button _button;
        [SerializeField] private GameSfx _gameSfx;
        
        [Inject] private SoundsManager _soundsManager;

        private void Awake()
        {
            _button.onClick.AddListener(PlaySound);
        }

        private void PlaySound() => _soundsManager.PlaySfx(_gameSfx);
    }
}