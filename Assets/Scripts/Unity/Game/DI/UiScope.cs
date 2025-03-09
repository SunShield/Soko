using Soko.Unity.Game.Ui.Management;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI
{
    public class UiScope : LifetimeScope
    {
        [SerializeField] private UiManager _uiManager;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_uiManager).AsSelf().AsImplementedInterfaces();
        }
    }
}