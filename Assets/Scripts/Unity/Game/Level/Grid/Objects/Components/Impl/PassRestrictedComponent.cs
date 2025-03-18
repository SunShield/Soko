using System.Collections.Generic;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;
using UnityEngine;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class PassRestrictedComponent : LevelObjectComponent
    {
        [SerializeField] private List<LevelObjectBase> _restrictedObjects = new ();
        
        public override void OnObjectAboutToEnter(LevelObjectBase enteringObject, MovementAction action)
        {
            foreach (var restrictedObject in _restrictedObjects)
            {
                if (enteringObject.PrefabKey != restrictedObject.PrefabKey) continue;
                
                action.Active = false;
                return;
            }
        }
    }
}