using Soko.Unity.Game.Level.Management;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI.Installers
{
    public class GameInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<LevelsManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }
    }
}