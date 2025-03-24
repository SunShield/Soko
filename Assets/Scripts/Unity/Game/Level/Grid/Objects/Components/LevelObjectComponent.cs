using Soko.Unity.Game.Level.Cycle;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components
{
    public abstract class LevelObjectComponent : MonoBehaviour
    {
        [Inject] protected LevelPlayCycleManager LevelPlayCycleManager;
        
        public LevelObjectBase Object { get; private set; }
        
        public void Initialize(LevelObjectBase objectBase)
        {
            Object = objectBase;
            PostInitialize();
        }
        
        protected virtual void PostInitialize() { }
        
        public virtual void OnStartWithObject(LevelObjectBase enteringObject) { }
        public virtual void OnObjectEntered(LevelObjectBase enteringObject) { }
        public virtual void OnObjectLeft(LevelObjectBase enteringObject) { }
        public virtual bool CheckObjectEnter(LevelObjectBase enteringObject) => true;
    }
}