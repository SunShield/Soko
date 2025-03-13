using System;
using System.Collections.Generic;
using UnityEngine;

namespace Soko.Core.Models.Levels
{
    [Serializable]
    public class LevelPack
    {
        public string Name;
        public string MusicKey;
        public Sprite LevelBackground;
        public Sprite HeaderSprite;
        public List<LevelData> Levels;
    }
}