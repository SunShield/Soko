using Soko.Unity.DataLayer.So;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Level.Management
{
    public class LevelManager : MonoBehaviour
    {
        [Inject] private LevelsDataSo _levelsDataSo;

        public void StartLevel(string levelName)
        {
            
        }
    }
}