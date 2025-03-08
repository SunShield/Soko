using System.Threading.Tasks;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class PlayerInteractableComponent : LevelObjectComponent
    {
        public sealed override async Task OnObjectAboutToEnter(LevelObjectBase enteringObject, MovementAction action)
        {
            if (enteringObject.HasComponent<PlayerComponent>())
                await OnPlayerAboutToEnter(enteringObject, action);
            else
                await OnNonPlayerAboutToEnter(enteringObject, action);
        }
        
        protected virtual async Task OnPlayerAboutToEnter(LevelObjectBase enteringObject, MovementAction action) { }
        protected virtual async Task OnNonPlayerAboutToEnter(LevelObjectBase enteringObject, MovementAction action) { }

        public sealed override async Task OnObjectEntered(LevelObjectBase enteringObject)
        {
            if (enteringObject.HasComponent<PlayerComponent>())
                await OnPlayerEntered(enteringObject);
            else
                await OnNonPlayerEntered(enteringObject);
        }
        
        protected virtual async Task OnPlayerEntered(LevelObjectBase enteringObject) { }
        protected virtual async Task OnNonPlayerEntered(LevelObjectBase enteringObject) { }
    }
}