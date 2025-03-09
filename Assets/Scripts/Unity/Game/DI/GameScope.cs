using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.DI
{
    public class GameScope : LifetimeScope
    {
        [SerializeField] private InstallersCollection _installersCollection;
        
        protected override void Configure(IContainerBuilder builder)
        {
            DontDestroyOnLoad(gameObject);
            _installersCollection.Installers.ForEach(installer => installer.Install(builder));
        }
    }
}