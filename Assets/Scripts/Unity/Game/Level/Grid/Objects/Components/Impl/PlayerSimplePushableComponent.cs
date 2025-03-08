using System.Threading.Tasks;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;
using Soko.Unity.Game.Level.Grid.Objects.Helpers;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    /// <summary>
    /// Everything player is capable of pushing 1 space
    /// </summary>
    public class PlayerSimplePushableComponent : PlayerInteractableComponent
    {
        [Inject] private LevelObjectOneCellMover _levelObjectOneCellMover;
        
        protected override async Task OnPlayerAboutToEnter(LevelObjectBase enteringObject, MovementAction action)
        {
            var pushDirection = action.Direction;
            var pushCell = Object.Cell.GetNeighbour(pushDirection);
            if (pushCell == null)
            {
                action.Active = false;
                return;
            }
            
            var pushAction = new MovementAction(pushDirection);
            await pushCell.OnObjectAboutToEnter(Object, pushAction);
            if (!pushAction.Active)
            {
                action.Active = false;
                return;
            }
            
            ExecuteMovement(pushCell);
        }
        
        private void ExecuteMovement(LevelGridCell targetCell) => 
            _levelObjectOneCellMover.MoveOneCell(Object, targetCell);
    }
}