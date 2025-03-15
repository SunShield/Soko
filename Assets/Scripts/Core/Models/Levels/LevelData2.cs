using System;

namespace Soko.Core.Models.Levels
{
    [Serializable]
    public class LevelData2
    {
        public string Name = "New Level";
        public CellData[, ] Cells = new CellData[1 ,1];
    }
}