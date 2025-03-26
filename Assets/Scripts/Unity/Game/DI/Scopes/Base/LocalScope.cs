using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI.Scopes.Base
{
    public class LocalScope : LifetimeScope
    {
        protected sealed override void Configure(IContainerBuilder builder)
        {
            RegisterSetScopeCallback(builder);
            ConfigureInternal(builder);
        }

        private void RegisterSetScopeCallback(IContainerBuilder builder)
            => builder.RegisterBuildCallback(container => { CurrentScopeProvider.Instance.CurrentScope = container; });
        protected virtual void ConfigureInternal(IContainerBuilder builder) { }
    }
}