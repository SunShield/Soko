using System.Threading.Tasks;
using Soko.Core.Extensions;
using Soko.Unity.Game.Events;
using Soko.Unity.Game.Events.Impl.Args;
using Soko.Unity.Game.Events.Impl.Events;
using Soko.Unity.Game.Level.Cycle;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl.Movement;
using Soko.Unity.Game.Level.Grid.Objects.Helpers;
using Soko.Unity.Game.Level.Grid.Objects.Movement;
using Soko.Unity.Game.Sounds;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class PlayerControlledComponent : LevelObjectComponent
    {
        [Inject] private LevelPlayCycleManager _levelPlayCycleManager;
        [Inject] private LevelObjectMover _levelObjectMover;
        [Inject] private SoundsManager _soundsManager;
        [Inject] private EventBus _eventBus;
        [Inject] private MoveManager _moveManager;
        
        private PlayerInputActions _playerInputActions;
        private Vector2Int GridSize => _levelPlayCycleManager.LevelGrid.Dimensions;
        
        private bool _executingMovement = false;
        private bool _isMovementDisabled;
        private float _defaultScaleX;

        protected override void PostInitialize()
        {
            _defaultScaleX = Object.transform.localScale.x;
            _playerInputActions = new ();
            _playerInputActions.Enable();
            _playerInputActions.Player.Move.performed += PerformMove;
            _eventBus.GetEvent<LevelWinEvent>().SubscribeForGlobal(DisableMovement);
        }
        
        private void DisableMovement(EmptyArgs args) => _isMovementDisabled = true;

        private async void PerformMove(InputAction.CallbackContext context)
        {
            if (_isMovementDisabled) return;
            if (_executingMovement) return;
            
            _moveManager.ExecutePlayerMovement(this.Object, GetMovementTarget(context));
        }

        private void RotatePlayer(Direction direction)
        {
            var scale = Object.transform.localScale;
            if (direction == Direction.Right)     Object.transform.localScale = new Vector3(-_defaultScaleX, scale.y, scale.z);
            else if (direction == Direction.Left) Object.transform.localScale = new Vector3(_defaultScaleX, scale.y, scale.z);
        }

        private Direction GetMovementTarget(InputAction.CallbackContext context)
        {
            var moveDirection = context.ReadValue<Vector2>();
            return moveDirection.ToDirection();
        }

        private void OnDisable()
        {
            _playerInputActions.Player.Move.performed -= PerformMove;
            _playerInputActions.Disable();
        }
    }
}