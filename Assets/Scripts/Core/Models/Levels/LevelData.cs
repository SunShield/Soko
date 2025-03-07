using System;
using UnityEngine;

namespace Soko.Core.Models.Levels
{
    [Serializable]
    public class LevelData
    {
        public string Name;
        
        [TextArea(4, 20)]
        public string LevelMap;
    }
}