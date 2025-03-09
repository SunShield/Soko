using System.Threading.Tasks;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;
using UnityEngine;

namespace Soko.Unity.Game.Level.Grid.Objects.Components
{
    public abstract class LevelObjectComponent : MonoBehaviour
    {
        public LevelObjectBase Object { get; private set; }
        protected GridCoords Position => Object.Position;
        
        public void Initialize(LevelObjectBase objectBase)
        {
            Object = objectBase;
            PostInitialize();
        }
        
        protected virtual void PostInitialize() { }
        
        public virtual void OnObjectAboutToEnter(LevelObjectBase enteringObject, MovementAction action) { }
        public virtual void OnObjectEntered(LevelObjectBase enteringObject) { }
        public virtual void OnObjectLeft(LevelObjectBase enteringObject) { }
    }
}