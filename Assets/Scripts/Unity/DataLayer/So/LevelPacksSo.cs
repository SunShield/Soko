using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Soko.Unity.DataLayer.So
{
    [CreateAssetMenu(fileName = "LevelPacks", menuName = "Data/Levels/Level Packs", order = 1)]
    public class LevelPacksSo : SerializedScriptableObject
    {
        [field: SerializeField] public List<LevelPackSo> LevelPacks { get; private set; } 
    }
}