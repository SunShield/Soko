using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Soko.Unity.Game.Level.Grid.Objects;
using UnityEngine;

namespace Soko.Unity.DataLayer.So
{
    [CreateAssetMenu(fileName = "LevelObjects", menuName = "Data/Level Objects So", order = 1)]
    public class LevelObjectsSo : SerializedScriptableObject
    {
        [field: SerializeField] public GameObject Empty { get; private set; }
        [field: OdinSerialize] public Dictionary<string, LevelObjectBase> LevelObjects { get; private set; }
    }
}