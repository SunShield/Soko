using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Soko.Unity.DataLayer.So;
using UnityEngine;

namespace Soko.Core.Models.Levels
{
    [Serializable]
    public class LevelPack
    {
        public string Name;
        public Sprite HeaderSprite;
        public List<LevelData> Levels;
    }
}