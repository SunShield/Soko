using Soko.Unity.DataLayer.So;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI
{
    public class RootScope : LifetimeScope
    {
        [SerializeField] private LevelsDataSo _levelsDataSo;
        [SerializeField] private UiDataSo _uiDataSo;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_levelsDataSo);
            builder.RegisterInstance(_uiDataSo);
        }
    }
}