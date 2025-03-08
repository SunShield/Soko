using System.Collections.Generic;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Level.Grid.Enums;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class ColorComponent : LevelObjectComponent
    {
        [SerializeField] private List<SpriteRenderer> _coloredElements;

        [Inject] private ColorDataSo _colorDataSo;
        
        public ObjectColor Color { get; private set; }

        public void SetColor(ObjectColor color)
        {
            Color = color;
            _coloredElements.ForEach(obj => obj.color = GetColor(Color));
        }

        private Color GetColor(ObjectColor color) => _colorDataSo.ColorMap[color];
    }
}