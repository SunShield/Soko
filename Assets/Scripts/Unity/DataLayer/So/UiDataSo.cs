using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Soko.Unity.Game.Ui.Enums;
using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;

namespace Soko.Unity.DataLayer.So
{
    [CreateAssetMenu(fileName = "UiData", menuName = "Fundamental/Ui Data")]
    public class UiDataSo : SerializedScriptableObject
    {
        [field: OdinSerialize] public Dictionary<UiElements, UiElement> UiElements { get; private set; }
    }
}