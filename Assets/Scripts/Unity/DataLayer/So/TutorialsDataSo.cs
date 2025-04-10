using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Soko.Unity.Game.Tutorials;
using UnityEngine;

namespace Soko.Unity.DataLayer.So
{
    [CreateAssetMenu(fileName = "Tutorials", menuName = "Data/Tutorials", order = 5)]
    public class TutorialsDataSo : SerializedScriptableObject
    {
        [field: OdinSerialize] public List<TutorialSequence> Tutorials { get; private set; }
    }
}