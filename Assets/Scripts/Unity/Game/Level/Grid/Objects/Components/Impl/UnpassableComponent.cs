using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;
using UnityEngine;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class UnpassableComponent : LevelObjectComponent
    {
        [SerializeField] public bool AllowPlayerPass = false;
        
        public override void OnObjectAboutToEnter(LevelObjectBase enteringObject, MovementAction action)
        {
            if (AllowPlayerPass && enteringObject.HasComponent<PlayerComponent>()) return;
            
            // grouped objects dont block objects from the same group,
            // otherwise, for example, 2-block group will be unable to move in some cases
            if (enteringObject.TryGetComponent<GroupComponent>(out var groupComponent)
                && Object.TryGetComponent<GroupComponent>(out var currentGroupComponent)) 
            {
                if (currentGroupComponent.Group != -1 && groupComponent.Group != -1 && 
                    currentGroupComponent.Group == groupComponent.Group) return;
            }
                
            action.Active = false;
        }
    }
}