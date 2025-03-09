using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Soko.Unity
{
    public class GameInitializer : MonoBehaviour, IPostInitializable
    {
        public async void PostInitialize()
        {
            await SceneManager.LoadSceneAsync(UnityConstants.Scenes.MainMenu);
        }
    }
}
