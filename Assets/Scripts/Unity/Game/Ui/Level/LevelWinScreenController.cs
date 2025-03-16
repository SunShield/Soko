using Cysharp.Threading.Tasks;
using Soko.Unity.Game.Sounds;
using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Ui.Level
{
    public class LevelWinScreenController : UiElement
    {
        [SerializeField] private LevelWinScreenView _view;
        
        [Inject] private SoundsManager _soundsManager;
        
        private void Awake()
        {
            _view.OnContinueButtonClicked += OnContinueButtonClicked;
        }

        protected override async UniTask OnEnabledAndConstructed()
        {
            _soundsManager.PlaySfx(GameSfx.WinLevel);
        }

        public void SetLevelWinResults(string levelName, int levelTurns)
            => _view.SetLevelWinResults(levelName, levelTurns);

        private void OnContinueButtonClicked() => Close();
    }
}