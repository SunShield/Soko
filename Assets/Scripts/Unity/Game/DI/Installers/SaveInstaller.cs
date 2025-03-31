using Soko.Unity.Game.Save.Impl.LevelsData;
using Soko.Unity.Game.Save.Impl.User;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI.Installers
{
    public class SaveInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<UserSaveDataManager>().AsSelf();
            builder.RegisterEntryPoint<LevelsProgressSaveDataManager>().AsSelf();
        }
    }
}