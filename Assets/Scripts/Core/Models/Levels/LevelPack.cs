using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Soko.Core.Models.Levels
{
    [Serializable]
    public class LevelPack
    {
        [ReadOnly] [HorizontalGroup("key")] public string Key;
        
        public string Name;
        public string MusicKey;
        public Sprite LevelBackground;
        public Sprite HeaderSprite;
        [HideReferenceObjectPicker] public List<LevelData> Levels;
        
#if UNITY_EDITOR
        [Button] [HorizontalGroup("key", width: 100)] private void GenerateKey() => Key = Guid.NewGuid().ToString();
#endif
    }
}