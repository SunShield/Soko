using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Soko.Unity.Game.Level.Grid.Objects;
using UnityEngine;

namespace Soko.Unity.DataLayer.So
{
    [CreateAssetMenu(fileName = "LevelObjects", menuName = "Data/Level Objects So", order = 1)]
    public class LevelObjectsSo : SerializedScriptableObject
    {
        [field: SerializeField] public GameObject Empty { get; private set; }
        [ListDrawerSettings(ShowPaging = false)]
        [field: SerializeField] public List<LevelObjectBase> LevelObjectsList { get; private set; }
        
        private Dictionary<string, LevelObjectBase> _levelObjects;
        public Dictionary<string, LevelObjectBase> LevelObjects 
            => _levelObjects ??= LevelObjectsList.ToDictionary(lo => lo.PrefabKey);
    }
}