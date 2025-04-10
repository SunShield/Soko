using Soko.Unity.Game.Tutorials;
using Soko.Unity.Game.Tutorials.Actions;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI.Installers
{
    public class TutorialInstaller : IInstaller
    {
        public void Install(IContainerBuilder builder)
        {
            builder.Register<TutorialActionExecutor>(Lifetime.Singleton).AsSelf();
            builder.Register<TutorialManager>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }
    }
}