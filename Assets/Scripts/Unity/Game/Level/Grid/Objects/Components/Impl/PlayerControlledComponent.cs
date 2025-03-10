using System.Threading.Tasks;
using Soko.Core.Extensions;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;
using Soko.Unity.Game.Level.Grid.Objects.Helpers;
using Soko.Unity.Game.Level.Management;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class PlayerControlledComponent : LevelObjectComponent
    {
        [Inject] private LevelManager _levelManager;
        [Inject] private LevelObjectOneCellMover _levelObjectOneCellMover;
        
        private PlayerInputActions _playerInputActions;
        private Vector2Int GridSize => _levelManager.LevelGrid.Dimensions;
        
        private bool _executingMovement = false;
        private float _defaultScaleX;

        protected override void PostInitialize()
        {
            _defaultScaleX = Object.transform.localScale.x;
            _playerInputActions = new ();
            _playerInputActions.Enable();
            _playerInputActions.Player.Move.performed += PerformMove;
        }

        private async void PerformMove(InputAction.CallbackContext context)
        {
            if (_executingMovement) return;

            var hasMovementTarget = GetMovementTarget(context, out var direction, out var targetCell);
            if (!hasMovementTarget) return;

            var movementAction = new MovementAction(direction);
            targetCell.OnObjectAboutToEnter(Object, movementAction);
            
            if (!movementAction.Active) return;
            
            RotatePlayer(direction);
            await ExecuteMovement(targetCell);
        }

        private void RotatePlayer(Direction direction)
        {
            var scale = Object.transform.localScale;
            if (direction == Direction.Right)     Object.transform.localScale = new Vector3(-_defaultScaleX, scale.y, scale.z);
            else if (direction == Direction.Left) Object.transform.localScale = new Vector3(_defaultScaleX, scale.y, scale.z);
        }

        private bool GetMovementTarget(InputAction.CallbackContext context, out Direction direction, out LevelGridCell targetCell)
        {
            var moveDirection = context.ReadValue<Vector2>();
            direction = moveDirection.ToDirection();
            targetCell = Object.Cell.GetNeighbour(direction);
            return targetCell != null;
        }

        private async Task ExecuteMovement(LevelGridCell targetCell)
        {
            _executingMovement = true;
            await _levelObjectOneCellMover.MoveOneCell(Object, targetCell);
            _executingMovement = false;
        }

        private void OnDisable()
        {
            _playerInputActions.Player.Move.performed -= PerformMove;
            _playerInputActions.Disable();
        }
    }
}