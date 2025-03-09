using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Soko.Unity
{
    public class GameInitializer : MonoBehaviour, IInitializable
    {
        public async void Initialize()
        {
            await SceneManager.LoadSceneAsync(UnityConstants.Scenes.UI, LoadSceneMode.Additive);
            await Task.Delay(2000);
            await SceneManager.LoadSceneAsync(UnityConstants.Scenes.MainMenu);
        }
    }
}
