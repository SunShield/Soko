using Soko.Unity.Game.MainMenu;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI
{
    public class MainMenuScope : LifetimeScope
    {
        [SerializeField] private MainMenuSceneInitializer _mainMenuSceneInitializer;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_mainMenuSceneInitializer).AsSelf().AsImplementedInterfaces();
        }
    }
}