using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class PlayerInteractableComponent : LevelObjectComponent
    {
        public sealed override void OnObjectAboutToEnter(LevelObjectBase enteringObject, MovementAction action)
        {
            if (enteringObject.HasComponent<PlayerComponent>())
                OnPlayerAboutToEnter(enteringObject, action);
            else
                OnNonPlayerAboutToEnter(enteringObject, action);
        }
        
        protected virtual void OnPlayerAboutToEnter(LevelObjectBase enteringObject, MovementAction parentMoveAction) { }
        protected virtual void OnNonPlayerAboutToEnter(LevelObjectBase enteringObject, MovementAction action) { }

        public sealed override void OnObjectEntered(LevelObjectBase enteringObject)
        {
            if (enteringObject.HasComponent<PlayerComponent>())
                OnPlayerEntered(enteringObject);
            else
                OnNonPlayerEntered(enteringObject);
        }
        
        protected virtual void OnPlayerEntered(LevelObjectBase enteringObject) { }
        protected virtual void OnNonPlayerEntered(LevelObjectBase enteringObject) { }
    }
}