using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Soko.Core.Models.Levels;
using UnityEngine;

namespace Soko.Unity.DataLayer.So
{
    [CreateAssetMenu(fileName = "Level Pack", menuName = "Data/Levels/LevelPack", order = 0)]
    public class LevelPackSo : SerializedScriptableObject
    {
        [InlineProperty] [HideLabel] [HideReferenceObjectPicker]
        [OdinSerialize] public LevelPack LevelPack;
    }
}