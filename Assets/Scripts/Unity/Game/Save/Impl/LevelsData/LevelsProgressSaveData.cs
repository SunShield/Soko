using System;
using System.Collections.Generic;

namespace Soko.Unity.Game.Save.Impl.LevelsData
{
    [Serializable]
    public class LevelsProgressSaveData : AbstractSaveData
    {
        public string LastPackKey;
        public string LastLevelKey;
        public Dictionary<string, LevelPackSaveData> PackSaveDatas = new();
    }
}