using UnityEngine;

namespace Soko.Unity.Game.Ui.Management
{
    public class UiContainer : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        public Canvas Canvas => _canvas;
    }
}