using Soko.Unity.Game.Level.Management;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI
{
    public class LevelScope : LifetimeScope
    {
        [SerializeField] private LevelManager _levelManager;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_levelManager).AsSelf();
        }
    }
}