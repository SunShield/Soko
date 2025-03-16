using UnityEngine;

namespace Soko.Unity.Game.Ui.Level
{
    public class LevelTimerController : MonoBehaviour
    {
        private const float Second = 1f;
        
        [SerializeField] private LevelTimerView _view;

        private bool _isActive;
        private float _timePassed = Second;
        private int _secondsPassed = 0;
        private bool SecondPassed => _timePassed == 0f;

        private void OnEnable()
        {
            _timePassed = Second;
            _secondsPassed = 0;
            _view.SetTimePassed(0);
            SetActive(true);
        }
        
        public void SetActive(bool isActive) => _isActive = isActive;

        private void Update()
        {
            if (!_isActive) return;
            if (SecondPassed)
            {
                _secondsPassed++;
                ResetTimer();
                _view.SetTimePassed(_secondsPassed);
            }
            else
                AdvanceTimer();
        }
        
        private void AdvanceTimer() => _timePassed = Mathf.Clamp(_timePassed -= Time.deltaTime, 0f, Second);
        private void ResetTimer() => _timePassed = Second;
    }
}