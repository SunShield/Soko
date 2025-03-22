using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Soko.Core.Extensions;
using Soko.Unity.Game.Events;
using Soko.Unity.Game.Events.Impl.Args;
using Soko.Unity.Game.Events.Impl.Events;
using Soko.Unity.Game.Level.Cycle;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl;
using Soko.Unity.Game.Level.Grid.Objects.Movement;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.Level.Management
{
    public class LevelInputManager : IInitializable
    {
        [Inject] private LevelPlayCycleManager _levelPlayCycleManager;
        [Inject] private EventBus _eventBus;
        [Inject] private MoveManager _moveManager;
        
        private readonly List<UserControlledComponent> _controlledObjects = new();
        private PlayerInputActions _playerInputActions;
        private bool _isMovementDisabled;
        
        public void Initialize()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.Move.performed += PerformMove;
            _eventBus.GetEvent<LevelWinEvent>().SubscribeForGlobal(DisableMovement);

            GatherControlledComponents();
        }

        private void GatherControlledComponents()
        {
            foreach (var levelObject in _levelPlayCycleManager.LevelGrid.LevelObjects)
            {
                if (!levelObject.TryGetObjectComponent<UserControlledComponent>(out var userControlledComponent)) 
                    continue;
                _controlledObjects.Add(userControlledComponent);
            }
        }

        private void PerformMove(InputAction.CallbackContext context)
        {
            if (_isMovementDisabled) return;

            var direction = GetMovementDirection(context);
            _controlledObjects.ForEach(co => _moveManager.RegisterObjectToMove(co.Object, direction));
            _moveManager.ExecuteObjectMovement(direction);
        }

        private Direction GetMovementDirection(InputAction.CallbackContext context)
            => context.ReadValue<Vector2>().ToDirection();
        
        private void DisableMovement(EmptyArgs args) => _isMovementDisabled = true;
    }
}