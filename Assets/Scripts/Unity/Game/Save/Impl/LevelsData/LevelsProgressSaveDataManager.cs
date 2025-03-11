namespace Soko.Unity.Game.Save.Impl.LevelsData
{
    public class LevelsProgressSaveDataManager : PlayerPrefsJsonSaveManager<LevelsProgressSaveData>
    {
        protected override string PrefsKey { get; } = "LevelsProgress";
    }
}