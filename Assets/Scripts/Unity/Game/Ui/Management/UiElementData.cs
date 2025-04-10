using Sirenix.OdinInspector;
using Soko.Unity.Game.Ui.Management.Elements;

namespace Soko.Unity.Game.Ui.Management
{
    [HideReferenceObjectPicker]
    public class UiElementData
    {
        [HideLabel][SuffixLabel("Prefab", true)][HorizontalGroup("g")]
        public UiElement Prefab;
        [HorizontalGroup("g", width: 100)][HideLabel][SuffixLabel("Sorting order", true)]
        public int DefaultSortingOrder;
    }
}