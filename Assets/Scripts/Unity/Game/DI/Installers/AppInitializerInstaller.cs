using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI.Installers
{
    public class AppInitializerInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<GameInitializer>().AsSelf();
        }
    }
}