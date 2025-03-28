using System;
using Soko.Core.Events;
using Soko.Core.Events.Impl.Args;
using Soko.Core.Events.Impl.Events;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.Level.Management
{
    public class LevelInputManager : IInitializable, IDisposable
    {
        [Inject] private UserMovementManager _userMovementManager;
        [Inject] private EventBus _eventBus;
        
        private PlayerInputActions _playerInputActions;
        
        public void Initialize()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.Move.performed += _userMovementManager.PerformMove;
            _playerInputActions.Player.Move.canceled += _userMovementManager.CancelMove;
        }

        public void Dispose()
        {
            _playerInputActions.Disable();
            _playerInputActions.Dispose();
            _playerInputActions = null;
        }
    }
}