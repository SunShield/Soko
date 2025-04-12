namespace Soko.Unity.Game.Save.Impl.LevelsData
{
    public class LevelsProgressSaveDataManager : PlayerPrefsJsonSaveManager<ProgressSaveData>
    {
        protected override string PrefsKey { get; } = "LevelsProgress";
    }
}