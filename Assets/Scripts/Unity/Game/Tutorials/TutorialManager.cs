using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Soko.Core.Events;
using Soko.Core.Events.Impl.Args;
using Soko.Core.Events.Impl.Events;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Level.Management;
using Soko.Unity.Game.Save.Impl.Tutorial;
using Soko.Unity.Game.Service.Conditions;
using Soko.Unity.Game.Tutorials.Actions;
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
        [Inject] private ConditionManager _conditionManager;
        [Inject] private TutorialActionExecutor _actionExecutor;
        
        private List<TutorialSequence> Tutorials => _tutorialsDataSo.Tutorials;
        private TutorialSaveData SaveData => _tutorialSaveDataManager.SaveData;
        public bool IsShowingTutorial { get; private set; }

        [Inject]
        private void Construct()
        {
            _eventBus.GetEvent<LevelFullyPreparedEvent>().SubscribeForGlobal(OnLevelFullyPrepared);
        }

        public void Initialize() { }
        
        private void OnLevelFullyPrepared(EmptyArgs args) => CheckTutorials();

        private async void CheckTutorials()
        {
            foreach (var tutorial in Tutorials)
            {
                if (SaveData.CompletedTutorials.Contains(tutorial.Key)) continue;
                if (tutorial.Conditions.Any(c => !_conditionManager.CheckCondition(c))) continue;

                await ExecuteTutorial(tutorial);
            }
        }

        private async UniTask ExecuteTutorial(TutorialSequence tutorial)
        {
            IsShowingTutorial = true;
            
            foreach (var tutorialAction in tutorial.Actions)
                await _actionExecutor.ExecuteAction(tutorialAction);
            CompleteTutorial(tutorial);
            
            IsShowingTutorial = false;
        }

        private void CompleteTutorial(TutorialSequence tutorialSequence)
        {
            SaveData.CompletedTutorials.Add(tutorialSequence.Key);
            _tutorialSaveDataManager.Save();
        }
    }
}