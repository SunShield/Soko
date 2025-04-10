using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;

namespace Soko.Unity.Game.Ui.Special.Info
{
    public class DefaultInfoPopupController : UiElement
    {
        [SerializeField] private DefaultInfoPopupView _view;

        private void Awake() => _view.OnExitButtonClicked += Close;
    }
}