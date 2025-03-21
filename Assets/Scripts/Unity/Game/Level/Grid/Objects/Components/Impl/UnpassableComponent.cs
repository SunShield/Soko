using UnityEngine;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class UnpassableComponent : LevelObjectComponent
    {
        [SerializeField] public bool AllowPlayerPass = false;

        public override bool CheckObjectEnter(LevelObjectBase enteringObject)
        {
            if (AllowPlayerPass && enteringObject.HasComponent<PlayerComponent>()) return true;

            return false;
        }
    }
}