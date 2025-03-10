using Soko.Unity.Game.Ui.Enums;
using Soko.Unity.Game.Ui.Management;
using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Ui.MainMenu
{
    public class MainMenuScreenController : UiElement
    {
        [SerializeField] private MainMenuScreenView _view;

        [Inject] private UiManager _uiManager;
        
        private void Awake()
        {
            _view.OnNewGameClicked += GoToLevelSelect;
            _view.OnSettingsClicked += OpenSettings;
            _view.OnExitClicked += ExitGame;
        }

        private void GoToLevelSelect() => _uiManager.OpenUiElement(UiElements.LevelSelectScreen, 2);
        
        private void OpenSettings()
        {
            
        }

        private void ExitGame() => Application.Quit();
    }
}