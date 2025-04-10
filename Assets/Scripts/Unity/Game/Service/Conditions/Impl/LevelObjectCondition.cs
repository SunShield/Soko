using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Soko.Unity.DataLayer;
using Soko.Unity.Game.Level.Management;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Service.Conditions.Impl
{
    [Serializable]
    public class LevelObjectCondition : AbstractCondition
    {
        [ValueDropdown("GetObjectKeys")]
        [SerializeField] private string _levelObjectKey;
        
        [Inject] private LevelsManager _levelsManager;
        
        public override bool Check()
        {
            if (_levelsManager.PlayCycleManager == null) return false;
            
            return _levelsManager.PlayCycleManager.LevelGrid.LevelObjects
                .Select(lo => lo.PrefabKey)
                .Contains(_levelObjectKey);
        }
        
#if UNITY_EDITOR
        private List<string> GetObjectKeys()
        {
            var levelObjectsSo = EditorDataProvider.Instance.LevelObjectsSo;
            return levelObjectsSo.LevelObjects.Keys.ToList();
        }
#endif
    }
}