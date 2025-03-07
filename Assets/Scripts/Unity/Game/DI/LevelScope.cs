using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Level.Grid.Building;
using Soko.Unity.Game.Level.Management;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI
{
    public class LevelScope : LifetimeScope
    {
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private LevelObjectsSo _levelObjectsSo;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_levelManager).AsSelf().AsImplementedInterfaces();
            builder.RegisterComponent(_levelObjectsSo).AsSelf();
            builder.RegisterEntryPoint<LevelGridBuilder>().AsSelf();
        }
    }
}