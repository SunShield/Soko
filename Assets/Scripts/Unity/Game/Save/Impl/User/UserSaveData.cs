using System;

namespace Soko.Unity.Game.Save.Impl.User
{
    [Serializable]
    public class UserSaveData : AbstractSaveData
    {
        public UserSoundSettings SoundSettings { get; set; } = new();
    }
}