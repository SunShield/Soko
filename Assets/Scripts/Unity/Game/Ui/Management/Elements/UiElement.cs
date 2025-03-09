using Sirenix.OdinInspector;

namespace Soko.Unity.Game.Ui.Management.Elements
{
    public class UiElement : SerializedMonoBehaviour
    {
        private UiContainer _container;

        public void Initialize(UiContainer container)
        {
            _container = container;
        }
    }
}