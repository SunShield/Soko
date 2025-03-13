using Soko.Unity.Game.Level.Management;
using Soko.Unity.Game.Sounds;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI.Installers
{
    public class GameInstaller : IInstaller
    {
        [SerializeField] private SoundsManager _soundsManager;
        
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterComponentInNewPrefab(_soundsManager, Lifetime.Singleton).AsSelf();
            builder.Register<LevelsManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }
    }
}