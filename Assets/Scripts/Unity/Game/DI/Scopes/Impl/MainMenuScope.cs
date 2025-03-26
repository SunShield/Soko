using Soko.Unity.Game.DI.Scopes.Base;
using Soko.Unity.Game.MainMenu;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI.Scopes.Impl
{
    public class MainMenuScope : LocalScope
    {
        [SerializeField] private MainMenuSceneInitializer _mainMenuSceneInitializer;
        
        protected override void ConfigureInternal(IContainerBuilder builder)
        {
            builder.RegisterComponent(_mainMenuSceneInitializer).AsSelf().AsImplementedInterfaces();
        }
    }
}