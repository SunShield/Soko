using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Soko.Core.Models.Levels;
using UnityEditor;
using UnityEngine;

namespace Soko.Unity.DataLayer.So
{
    [CreateAssetMenu(fileName = "Level Pack", menuName = "Data/Levels/LevelPack", order = 0)]
    public class LevelPackSo : SerializedScriptableObject
    {
        [InlineProperty] [HideLabel] [HideReferenceObjectPicker]
        [NonSerialized][OdinSerialize] public LevelPack LevelPack;
        
#if UNITY_EDITOR
        [Button] private void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
    }
}