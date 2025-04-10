using Soko.Core.Events;
using Soko.Unity.Game.Level.Management;
using Soko.Unity.Game.Service.Conditions;
using Soko.Unity.Game.Sounds;
using Soko.Unity.Game.Tutorials;
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
            builder.Register<EventBus>(Lifetime.Singleton).AsSelf();
            builder.RegisterComponentInNewPrefab(_soundsManager, Lifetime.Singleton).AsSelf();
            builder.Register<LevelsManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<ConditionManager>(Lifetime.Singleton).AsSelf();
            builder.Register<TutorialManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }
    }
}