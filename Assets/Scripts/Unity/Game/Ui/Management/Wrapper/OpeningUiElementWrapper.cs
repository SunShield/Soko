using Soko.Unity.Game.Ui.Management.Elements;

namespace Soko.Unity.Game.Ui.Management.Wrapper
{
    /// <summary>
    /// Helper class for the UiManager
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    public class OpeningUiElementWrapper<TElement>
        where TElement : UiElement
    {
        public UiManager UiManager { get; private set; }
        public TElement UiElement { get; private set; }
        
        public OpeningUiElementWrapper(UiManager manager, TElement uiElement)
        {
            UiManager = manager;
            UiElement = uiElement;
        }

        public TElement FinishOpeningProcess()
        {
            UiElement.SetFinished();
            UiManager.ActivateUiElement<TElement>();
            return UiElement;
        }
    }
}