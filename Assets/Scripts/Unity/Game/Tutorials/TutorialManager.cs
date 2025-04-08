using Soko.Core.Events;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Level.Management;
using Soko.Unity.Game.Save.Impl.Tutorial;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.Tutorials
{
    public class TutorialManager : IInitializable
    {
        [Inject] private EventBus _eventBus;
        [Inject] private TutorialsDataSo _tutorialsDataSo;
        [Inject] private TutorialSaveDataManager _tutorialSaveDataManager;
        [Inject] private LevelsManager _levelsManager;
        
        public bool IsShowingTutorial { get; private set; }

        public void Initialize()
        {
            
        }

        private void CheckTutorials()
        {
            
        }
    }
}