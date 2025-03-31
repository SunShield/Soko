using System;
using UnityEngine.Serialization;

namespace Soko.Unity.Game.Save.Impl.User
{
    [Serializable]
    public class UserSoundSettings
    {
        public bool MusicOn = true;
        [FormerlySerializedAs("SoundOn")] public bool SfxOn = true;
    }
}