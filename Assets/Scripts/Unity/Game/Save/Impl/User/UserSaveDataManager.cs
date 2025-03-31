namespace Soko.Unity.Game.Save.Impl.User
{
    public class UserSaveDataManager : PlayerPrefsJsonSaveManager<UserSaveData>
    {
        protected override string PrefsKey { get; } = "User";
    }
}