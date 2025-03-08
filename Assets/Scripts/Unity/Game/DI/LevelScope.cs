using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Level.Grid.Building;
using Soko.Unity.Game.Level.Grid.Objects.Helpers;
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
        [SerializeField] private ColorDataSo _colorDataSo;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_levelManager).AsSelf().AsImplementedInterfaces();
            builder.RegisterComponent(_levelObjectsSo).AsSelf();
            builder.RegisterInstance(_colorDataSo);
            builder.RegisterEntryPoint<LevelGridBuilder>().AsSelf();
            builder.RegisterEntryPoint<LevelObjectOneCellMover>().AsSelf();
        }
    }
}