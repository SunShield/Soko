using Cysharp.Threading.Tasks;
using Soko.Unity.Game.Level.Management;
using Soko.Unity.Game.Ui.Enums;
using Soko.Unity.Game.Ui.Management;
using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Ui.Level
{
    public class LevelScreenController : UiElement
    {
        [SerializeField] private LevelScreenView _view;
        [SerializeField] private LevelTurnsCounterController _levelTurnsCounterController;
        
        [Inject] private LevelsManager _levelsManager;
        [Inject] private UiManager _uiManager;

        private void Awake()
        {
            _view.OnBackClicked += EndLevel;
        }

        private void EndLevel()
        {
            _uiManager.CloseUiElement(UiElements.LevelMainScreen);
            _levelsManager.EndCurrentLevel();
        }

        protected override async UniTask OnEnabledAndConstructed()
        {
            _levelTurnsCounterController.Initialize(_levelsManager);
        }
    }
}