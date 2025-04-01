using System;
using Soko.Core.Events;
using Soko.Unity.Game.Level.History;
using VContainer;
using VContainer.Unity;

namespace Soko.Unity.Game.Level.Management
{
    public class LevelInputManager : IInitializable, IDisposable
    {
        [Inject] private UserMovementManager _userMovementManager;
        [Inject] private EventBus _eventBus;
        [Inject] private ContinuousTurnReverter _continuousTurnReverter;
        
        private PlayerInputActions _playerInputActions;
        
        public void Initialize()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();
            _playerInputActions.Player.Move.performed += _userMovementManager.PerformMove;
            _playerInputActions.Player.Revert.performed += _continuousTurnReverter.StartReverting;
            _playerInputActions.Player.Move.canceled += _userMovementManager.CancelMove;
            _playerInputActions.Player.Revert.canceled += _continuousTurnReverter.EndReverting;
        }

        public void Dispose()
        {
            _playerInputActions.Disable();
            _playerInputActions.Dispose();
            _playerInputActions = null;
        }
    }
}