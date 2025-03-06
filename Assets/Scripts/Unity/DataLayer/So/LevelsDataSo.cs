using System.Collections.Generic;
using Soko.Core.Models.Levels;
using UnityEngine;

namespace Soko.Unity.DataLayer.So
{
    [CreateAssetMenu(fileName = "LevelsData", menuName = "Data/Levels Data So")]
    public class LevelsDataSo : ScriptableObject
    {
        [field: SerializeField] public List<LevelData> Levels { get; private set; }
    }
}