using Soko.Unity.Game.Ui.Management;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI.Installers
{
    public class UiInstaller : IInstaller
    {
        [SerializeField] private UiManager _uiManager;
        
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab(_uiManager, Lifetime.Singleton).AsSelf();
        }
    }
}