using Soko.Unity.Game.Level.Cycle;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;
using Soko.Unity.Game.Level.Grid.Objects.Movement;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components
{
    public abstract class LevelObjectComponent : MonoBehaviour
    {
        [Inject] protected LevelPlayCycleManager LevelPlayCycleManager;
        
        public LevelObjectBase Object { get; private set; }
        protected GridCoords Position => Object.Position;
        
        public void Initialize(LevelObjectBase objectBase)
        {
            Object = objectBase;
            PostInitialize();
        }
        
        protected virtual void PostInitialize() { }
        
        public virtual void OnObjectEntered(LevelObjectBase enteringObject) { }
        public virtual void OnObjectLeft(LevelObjectBase enteringObject) { }
        public virtual bool CheckObjectEnter(LevelObjectBase enteringObject) => true;
    }
}