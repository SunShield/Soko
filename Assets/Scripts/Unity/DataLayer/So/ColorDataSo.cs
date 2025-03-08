using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Soko.Core.Models.Levels;
using Soko.Unity.Game.Level.Grid.Enums;
using UnityEngine;
using VContainer.Unity;

namespace Soko.Unity.DataLayer.So
{
    [CreateAssetMenu(fileName = "ColorData", menuName = "Data/Colors So", order = 2)]
    public class ColorDataSo : SerializedScriptableObject
    {
        [field: OdinSerialize] public Dictionary<string, LevelObjectColorData> Colors;

        private Dictionary<ObjectColor, Color> _colorMap;
        public Dictionary<ObjectColor, Color> ColorMap
        {
            get { return _colorMap ??= Colors.Values.ToDictionary(c => c.Color, c => c.UnityColor); }
        }
    }
}