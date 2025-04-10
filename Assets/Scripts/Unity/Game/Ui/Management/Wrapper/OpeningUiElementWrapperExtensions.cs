using Soko.Unity.Game.Ui.Management.Elements;

namespace Soko.Unity.Game.Ui.Management.Wrapper
{
    public static class OpeningUiElementWrapperExtensions
    {
        public static OpeningUiElementWrapper<TElement> ConfigureElement<TElement, TConfigureData>
            (this OpeningUiElementWrapper<TElement> wrapper, TConfigureData data)
            where TElement : UiElement, IConfigurableUiElement<TConfigureData>
        {
            wrapper.UiElement.Configure(data);
            return wrapper;
        }
    }
}