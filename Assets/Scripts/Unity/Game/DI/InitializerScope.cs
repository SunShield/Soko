using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI
{
    public class InitializerScope : LifetimeScope
    {
        [SerializeField] private GameInitializer _gameInitializer;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_gameInitializer).AsSelf().AsImplementedInterfaces(); 
        }
    }
}