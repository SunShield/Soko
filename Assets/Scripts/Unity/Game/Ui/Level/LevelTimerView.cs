using System;
using TMPro;
using UnityEngine;

namespace Soko.Unity.Game.Ui.Level
{
    public class LevelTimerView : MonoBehaviour
    {
        private const string TimeFormat = @"mm\:ss";
        
        [SerializeField] private TextMeshProUGUI _timerText;

        public void SetTimePassed(int seconds)
        {
            var timeSpan = TimeSpan.FromSeconds(seconds);
            var formattedTime = timeSpan.ToString(TimeFormat);
            _timerText.text = formattedTime;
        }
    }
}