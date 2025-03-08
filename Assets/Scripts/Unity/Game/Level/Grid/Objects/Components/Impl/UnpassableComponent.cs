using System.Threading.Tasks;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;
using UnityEngine;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class UnpassableComponent : LevelObjectComponent
    {
        [SerializeField] public bool AllowPlayerPass = false;
        
        public override async Task OnObjectAboutToEnter(LevelObjectBase enteringObject, MovementAction action)
        {
            if (AllowPlayerPass && enteringObject.HasComponent<PlayerComponent>()) return;
                
            action.Active = false;
        }
    }
}