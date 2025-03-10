using System.Collections.Generic;
using UnityEngine;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelPackDrawerView : MonoBehaviour
    {
        private const int LineCapacity = 4;
        
        [SerializeField] private List<RectTransform> _lines;

        private List<LevelBoxView> _levelBoxViews = new(); 

        public void AddLevelBox(LevelBoxView levelBoxView)
        {
            
        }
    }
}