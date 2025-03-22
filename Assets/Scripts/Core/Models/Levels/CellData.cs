using System;
using Soko.Unity.Game.Level.Grid.Enums;

namespace Soko.Core.Models.Levels
{
    [Serializable]
    public class CellData
    {
        public string GroundObjectKey;
        public ObjectColor GroundColor;
        public int GroundGroup = -1;
        public string ObjectKey;
        public ObjectColor Color;
        public int Group = -1;
    }
}