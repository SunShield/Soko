using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.DI.Scopes.Base;
using Soko.Unity.Game.Level.Cycle;
using Soko.Unity.Game.Level.Grid.Building;
using Soko.Unity.Game.Level.Grid.Objects.Movement;
using Soko.Unity.Game.Level.History;
using Soko.Unity.Game.Level.Management;
using Soko.Unity.Game.Level.Metrics;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI.Scopes.Impl
{
    public class LevelScope : LocalScope
    {
        [SerializeField] private LevelPlayCycleManager _levelPlayCycleManager;
        [SerializeField] private UserMovementManager _userMovementManager;
        [SerializeField] private LevelObjectsSo _levelObjectsSo;
        [SerializeField] private ColorDataSo _colorDataSo;
        
        protected override void ConfigureInternal(IContainerBuilder builder)
        {
            builder.Register<HistoryManager>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
            builder.RegisterComponent(_levelPlayCycleManager).AsSelf().AsImplementedInterfaces();
            builder.Register<LevelInputManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.RegisterComponent(_userMovementManager).AsSelf().AsImplementedInterfaces();
            builder.RegisterComponent(_levelObjectsSo).AsSelf();
            builder.RegisterInstance(_colorDataSo);
            builder.RegisterEntryPoint<LevelGridBuilder>().AsSelf();
            builder.RegisterEntryPoint<LevelObjectMover>().AsSelf();
            builder.RegisterEntryPoint<MoveManager>().AsSelf();
            builder.Register<LevelTurnsCountTracker>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }
    }
}