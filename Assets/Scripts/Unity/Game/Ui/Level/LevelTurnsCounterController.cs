using Soko.Unity.Game.Level.Management;
using UnityEngine;

namespace Soko.Unity.Game.Ui.Level
{
    public class LevelTurnsCounterController : MonoBehaviour
    {
        [SerializeField] private LevelTurnsCounterView _view;

        public void Initialize(LevelsManager levelManager)
        {
            levelManager.PlayCycleManager.OnTurnCountChanged += _view.SetTurns;
        }
    }
}