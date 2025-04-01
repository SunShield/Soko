using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace Soko.Unity.Game.Level.History
{
    public class ContinuousTurnReverter : MonoBehaviour
    {
        private const float DelayUntilRepeatedRevert = 0.35f;
        private const float TurnRevertDelay = 0.15f;
        
        [Inject] private HistoryManager _historyManager;

        private float _continuousTurnRevertTimer;
        private float _turnRevertTimer;
        private bool _isTurnReverting;
        private bool _firstRevertPerformed;

        private bool WaitingForRepeatedRevert => _continuousTurnRevertTimer > 0;
        private bool ExecutingRepeatedRevert => _turnRevertTimer >= 0;

        public void StartReverting() => SetTurnReverting(true);
        public void StartReverting(InputAction.CallbackContext context) => SetTurnReverting(true);
        public void EndReverting() => SetTurnReverting(false);
        public void EndReverting(InputAction.CallbackContext context) => SetTurnReverting(false);
        
        private void SetTurnReverting(bool turnReverting)
        {
            _isTurnReverting = turnReverting;
            
            if (!_isTurnReverting) RestoreState();
        }

        private void Update()
        {
            if (!_isTurnReverting) return;

            if (ProcessFirstRevert()) return;
            if (ProcessWaitingForRepeatedRevert()) return;

            ProcessRepeatedRevert();
        }

        private bool ProcessFirstRevert()
        {
            if (_firstRevertPerformed) return false;
            
            RevertTurn();
            ResetContinuousRevertTimer();
            _firstRevertPerformed = true;
            return true;
        }
        
        private void ResetContinuousRevertTimer() => _continuousTurnRevertTimer = DelayUntilRepeatedRevert;
        private void RevertTurn() => _historyManager.RevertTurn();

        private bool ProcessWaitingForRepeatedRevert()
        {
            if (WaitingForRepeatedRevert)
            {
                AdvanceContinuousRevertTimer();
                if (_continuousTurnRevertTimer != 0) return true;
                
                ResetRevertTimer();
            }

            return false;
        }

        private void AdvanceContinuousRevertTimer()
            => _continuousTurnRevertTimer = Mathf.Max(_continuousTurnRevertTimer - Time.deltaTime, 0f);
        private void ResetRevertTimer() => _turnRevertTimer = TurnRevertDelay;
        private void AdvanceRevertTimer()
            => _turnRevertTimer = Mathf.Clamp(_turnRevertTimer - Time.deltaTime, 0, TurnRevertDelay);
        
        private void ProcessRepeatedRevert()
        {
            if (!ExecutingRepeatedRevert) return;
            AdvanceRevertTimer();

            if (_turnRevertTimer != 0) return;
            RevertTurn();
            ResetRevertTimer();
        }

        private void RestoreState()
        {
            _continuousTurnRevertTimer = 0f;
            _turnRevertTimer = 0f;
            _firstRevertPerformed = false;
        }
    }
}