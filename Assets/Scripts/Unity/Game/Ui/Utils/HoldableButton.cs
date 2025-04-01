using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Soko.Unity.Game.Ui.Utils
{
    public class HoldableButton : Button
    {
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            OnClickStart?.Invoke();
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            OnClickRelease?.Invoke();
        }

        public event Action OnClickStart;
        public event Action OnClickRelease;
    }
}