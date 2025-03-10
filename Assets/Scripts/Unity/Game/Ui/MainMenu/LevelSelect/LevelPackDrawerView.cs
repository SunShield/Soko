using System.Collections.Generic;
using UnityEngine;

namespace Soko.Unity.Game.Ui.MainMenu.LevelSelect
{
    public class LevelPackDrawerView : MonoBehaviour
    {
        private const int LineCapacity = 4;
        
        [SerializeField] private List<RectTransform> _lines;

        private readonly List<LevelBoxView> _levelBoxViews = new(); 

        public void AddLevelBox(LevelBoxView levelBoxView)
        {
            _levelBoxViews.Add(levelBoxView);
            var line = _lines[(_levelBoxViews.Count - 1) / LineCapacity];
            levelBoxView.transform.SetParent(line);
        }

        public void ClearLevelBoxes() => _levelBoxViews.Clear();
    }
}