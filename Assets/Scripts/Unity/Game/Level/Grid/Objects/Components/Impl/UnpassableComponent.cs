using System.Threading.Tasks;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class UnpassableComponent : LevelObjectComponent
    {
        public override async Task OnObjectAboutToEnter(LevelObjectBase enteringObject, MovementAction action)
        {
            action.Active = false;
        }
    }
}