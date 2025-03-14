using System;
using Soko.Unity.Game.Level.Grid.Enums;

namespace Soko.Core.Models.Levels
{
    [Serializable]
    public class CellData
    {
        public string TileKey;
        public string ObjectKey;
        public ObjectColor Color;
        public int Group = -1;
    }
}