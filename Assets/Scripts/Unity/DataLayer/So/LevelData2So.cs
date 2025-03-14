using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Soko.Core.Models.Levels;
using UnityEngine;

namespace Soko.Unity.DataLayer.So
{
    [CreateAssetMenu(fileName = "LevelData2", menuName = "LevelData2")]
    public class LevelData2So : SerializedScriptableObject
    {
        [HideReferenceObjectPicker]
        [NonSerialized][OdinSerialize] public LevelData2 levelData;
    }
}