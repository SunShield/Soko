using System;
using System.Collections.Generic;

namespace Soko.Unity.Game.Save.Impl.LevelsData
{
    [Serializable]
    public class LevelsProgressSaveData : AbstractSaveData
    {
        public List<LevelPackSaveData> PackSaveDatas = new();
    }
}