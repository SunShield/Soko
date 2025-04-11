using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Soko.Unity.DataLayer;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl;
using Soko.Unity.Game.Level.Management;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Service.Conditions.Impl
{
    [Serializable]
    public class LevelObjectCondition : AbstractCondition
    {
        private const string AnyObjectKey = "Any";
        
        [ValueDropdown("GetObjectKeys")]
        [HorizontalGroup("g")][VerticalGroup("g/v1")][HideLabel]
        [SerializeField] private HashSet<string> _levelObjectKeys = new();
        
        [HorizontalGroup("g")][VerticalGroup("g/v2")]
        [SerializeField] private bool _requireColored;
        [HorizontalGroup("g")][VerticalGroup("g/v2")]
        [SerializeField] private bool _requireGrouped;
        
        [Inject] private LevelsManager _levelsManager;
        
        public override bool Check()
        {
            if (_levelsManager.PlayCycleManager == null) return false;

            return _levelsManager.PlayCycleManager.LevelGrid.LevelObjects
                .Where(CheckObjectFulfilsCondition)
                .Any();
        }

        private bool CheckObjectFulfilsCondition(LevelObjectBase lo)
        {
            if (!_levelObjectKeys.Contains(AnyObjectKey))
                if (!_levelObjectKeys.Contains(lo.PrefabKey))
                    return false;

            if (_requireColored)
            {
                lo.TryGetObjectComponent<ColorComponent>(out var colorComponent);
                if (colorComponent == null) return false;
                if (colorComponent.Color == ObjectColor.None) return false;
            }

            if (_requireGrouped)
            {
                lo.TryGetObjectComponent<GroupComponent>(out var groupComponent);
                if (groupComponent == null) return false;
                if (groupComponent.Group == UnityConstants.Level.NoBindingGroup) return false;
            }

            return true;
        }

#if UNITY_EDITOR
        private List<string> GetObjectKeys()
        {
            var levelObjectsSo = EditorDataProvider.Instance.LevelObjectsSo;
            var possibleObjects = new List<string>() { "Any" };
            possibleObjects.AddRange(levelObjectsSo.LevelObjects.Keys.ToList());
            return possibleObjects;
        }
#endif
    }
}