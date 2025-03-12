using TMPro;
using UnityEngine;

namespace Soko.Unity.Game.Ui.Level
{
    public class LevelTurnsCounterView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        
        public void SetTurns(int turns) => _text.text =  $"Turns: {turns}";
    }
}