namespace Soko.Unity.Game.Save.Impl.Tutorial
{
    public class TutorialSaveDataManager : PlayerPrefsJsonSaveManager<TutorialSaveData>
    {
        protected override string PrefsKey { get; } = "Tutorial";
    }
}