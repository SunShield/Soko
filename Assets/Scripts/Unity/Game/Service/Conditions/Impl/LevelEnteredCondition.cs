using Soko.Unity.Game.Level.Management;
using VContainer;

namespace Soko.Unity.Game.Service.Conditions.Impl
{
    public class LevelEnteredCondition : AbstractCondition
    {
        [Inject] private LevelsManager _levelsManager;
        
        public override bool Check()
            => _levelsManager.PlayCycleManager != null && _levelsManager.PlayCycleManager.LevelGrid != null;
    }
}