using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Soko.Unity
{
    public class GameInitializer : MonoBehaviour, IInitializable
    {
        public async void Initialize()
        {
            await SceneManager.LoadSceneAsync(UnityConstants.Scenes.LevelScene);
        }
    }
}
