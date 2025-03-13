using Soko.Unity.DataLayer.So;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI.Installers
{
    public class DataInstaller : IInstaller
    {
        [SerializeField] private LevelPacksSo _levelPacksSo;
        [SerializeField] private UiDataSo _uiDataSo;
        [SerializeField] private GroupSpritesSo _groupSpritesSo;
        [SerializeField] private SoundSo _soundSo;
        
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterInstance(_levelPacksSo);
            builder.RegisterInstance(_uiDataSo);
            builder.RegisterInstance(_groupSpritesSo);
            builder.RegisterInstance(_soundSo);
        }
    }
}