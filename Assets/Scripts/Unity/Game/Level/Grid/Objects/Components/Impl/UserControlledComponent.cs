using Soko.Core.Extensions;
using Soko.Unity.Game.Events;
using Soko.Unity.Game.Events.Impl.Args;
using Soko.Unity.Game.Events.Impl.Events;
using Soko.Unity.Game.Level.Cycle;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects.Helpers;
using Soko.Unity.Game.Level.Grid.Objects.Movement;
using Soko.Unity.Game.Sounds;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class UserControlledComponent : LevelObjectComponent
    {
        [Inject] private LevelPlayCycleManager _levelPlayCycleManager;
        [Inject] private LevelObjectMover _levelObjectMover;
        [Inject] private SoundsManager _soundsManager;
        [Inject] private EventBus _eventBus;
        [Inject] private MoveManager _moveManager;
        
        private PlayerInputActions _playerInputActions;
        
        private bool _isMovementDisabled;

        protected override void PostInitialize()
        {
            _playerInputActions = new ();
            _playerInputActions.Enable();
            _playerInputActions.Player.Move.performed += PerformMove;
            _eventBus.GetEvent<LevelWinEvent>().SubscribeForGlobal(DisableMovement);
        }
        
        private void DisableMovement(EmptyArgs args) => _isMovementDisabled = true;

        private void PerformMove(InputAction.CallbackContext context)
        {
            if (_isMovementDisabled) return;
            
            _moveManager.ExecuteControlledObjectMovement(this.Object, GetMovementDirection(context));
        }

        private Direction GetMovementDirection(InputAction.CallbackContext context)
            => context.ReadValue<Vector2>().ToDirection();

        private void OnDisable()
        {
            _playerInputActions.Player.Move.performed -= PerformMove;
            _playerInputActions.Disable();
        }
    }
}