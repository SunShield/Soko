using Soko.Unity.Game.Ui.MainMenu;
using Soko.Unity.Game.Ui.Management;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.MainMenu
{
    public class MainMenuSceneInitializer : MonoBehaviour, IInitializable
    {
        [Inject] private UiManager _uiManager;
        
        public void Initialize()
        {
            _uiManager.SimpleOpenUiElement<MainMenuScreenController>();
        }
    }
}