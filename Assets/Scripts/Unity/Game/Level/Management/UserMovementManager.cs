using System.Collections.Generic;
using Soko.Core.Events;
using Soko.Core.Events.Impl.Args;
using Soko.Core.Events.Impl.Events;
using Soko.Core.Extensions;
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
    public class UserMovementManager : MonoBehaviour, IInitializable
    {
        private const float MoveDelay = 0.2f;
        
        [Inject] private LevelPlayCycleManager _levelPlayCycleManager;
        [Inject] private EventBus _eventBus;
        [Inject] private MoveManager _moveManager;
        
        private readonly List<UserControlledComponent> _controlledObjects = new();
        private bool _isMovementDisabled;
        private Direction _moveDirection;
        private float _moveDelayTimer = MoveDelay;
        private bool MovementReady => _moveDelayTimer == 0f;
        
        public void Initialize()
        {
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

        public void PerformMove(InputAction.CallbackContext context)
        {
            _moveDirection = context.ReadValue<Vector2>().ToDirection();
        }

        public void CancelMove(InputAction.CallbackContext context)
        {
            _moveDirection = Direction.None;
        }

        private Direction GetMovementDirection(InputAction.CallbackContext context)
            => context.ReadValue<Vector2>().ToDirection();
        
        private void DisableMovement(EmptyArgs args) => _isMovementDisabled = true;

        private void TryMove()
        {
            if (_isMovementDisabled) return;
            if (_moveManager.IsExecuting) return;
            if (_moveDirection == Direction.None) return;
            
            _controlledObjects.ForEach(co => _moveManager.RegisterObjectToMove(co.Object, _moveDirection));
            _moveManager.ExecuteObjectsMovement(_moveDirection);
        }

        private void Update()
        {
            if (MovementReady)
            {
                TryMove();
                if (_moveDirection != Direction.None) RestoreTimer();
            }
            else
                AdvanceTimer();
        }
        
        private void AdvanceTimer() => _moveDelayTimer = Mathf.Clamp(_moveDelayTimer - Time.deltaTime, 0f, MoveDelay);
        private void RestoreTimer() => _moveDelayTimer = MoveDelay;
    }
}