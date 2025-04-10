using Soko.Unity.Game.ScenesManagement;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity
{
    public class GameInitializer : MonoBehaviour, IPostInitializable
    {
        [Inject] private ScenesManager _scenesManager;
        
        public async void PostInitialize()
        {
            await _scenesManager.LoadSceneAsync(UnityConstants.Scenes.MainMenu);
        }
    }
}
