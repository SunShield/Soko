using Soko.Unity.DataLayer.So;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI.Installers
{
    public class DataInstaller : IInstaller
    {
        [SerializeField] private LevelsDataSo _levelsDataSo;
        [SerializeField] private UiDataSo _uiDataSo;
        
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_levelsDataSo);
            builder.RegisterInstance(_uiDataSo);
        }
    }
}