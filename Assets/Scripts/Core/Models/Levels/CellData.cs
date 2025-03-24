using System;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects;

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

        public void SetObjectKey(string key, ObjectLayer layer)
        {
            if (layer == ObjectLayer.Ground) GroundObjectKey = key;
            else                             ObjectKey = key;
        }

        public void SetColor(ObjectColor color, ObjectLayer layer)
        {
            if (layer == ObjectLayer.Ground) GroundColor = color;
            else                             Color = color;
        }

        public void SetGroup(int group, ObjectLayer layer)
        {
            if (layer == ObjectLayer.Ground) GroundGroup = group;
            else                             Group = group;
        }
    }
}