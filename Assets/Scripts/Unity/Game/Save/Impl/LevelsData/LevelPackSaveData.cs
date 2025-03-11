using System;
using System.Collections.Generic;

namespace Soko.Unity.Game.Save.Impl.LevelsData
{
    [Serializable]
    public class LevelPackSaveData
    {
        public List<LevelSaveData> Levels = new();
    }
}