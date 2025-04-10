using Soko.Core.Events;
using Soko.Core.Events.Impl.Events;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace Soko.Unity.Game.ScenesManagement
{
    public class ScenesManager
    {
        [Inject] private EventBus _eventBus;
        
        public void LoadScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            _eventBus.GetEvent<PreSceneLoadedEvent>().InvokeForGlobal(new());
            SceneManager.LoadScene(sceneName, mode);
        }

        public AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
        {
            _eventBus.GetEvent<PreSceneLoadedEvent>().InvokeForGlobal(new());
            return SceneManager.LoadSceneAsync(sceneName, mode);
        }
    }
}