using Soko.Unity.Game.Save.Impl.LevelsData;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI.Installers
{
    public class SaveInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<LevelsProgressSaveDataManager>().AsSelf();
        }
    }
}