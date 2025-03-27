using Soko.Unity.Game.Level.Metrics;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Ui.Level
{
    public class LevelTurnsCounterController : MonoBehaviour
    {
        [SerializeField] private LevelTurnsCounterView _view;
        
        [Inject] private LevelTurnsCountTracker _turnsCountTracker;

        public void Initialize()
        {
            _view.SetTurns(0);
            _turnsCountTracker.OnTurnCountChanged += _view.SetTurns;
        }

        private void OnDisable()
        {
            _turnsCountTracker.OnTurnCountChanged -= _view.SetTurns;
        }
    }
}