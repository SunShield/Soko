using System;
using System.Linq;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects.Components.Impl;
using UnityEngine;

namespace Soko.Editor.Drawers.Data.LevelDataDraw.Elements
{
    public class ObjectsDrawer
    {
        private readonly LevelObjectsSo _levelObjectsSo;
        
        public ObjectsDrawer(LevelObjectsSo levelObjectsSo) => _levelObjectsSo = levelObjectsSo;

        public void DrawGroundObject(string key, Rect rect)
        {
            key ??= string.Empty;
            var hasObject = _levelObjectsSo.LevelObjects.TryGetValue(key, out var levelObject);
            var texture = GetCellTexture(key, true);

            if (hasObject)
            {
                var cpb = levelObject.Components.FirstOrDefault(c => c is ColorPushButtonComponent);
                if (cpb != null)
                {
                    var typedCpb = (ColorPushButtonComponent)cpb;
                    DrawColorPushButton(texture, rect, typedCpb.Direction);
                }
                else
                    DrawGroundDefault(texture, rect);
            }
            else
                DrawGroundDefault(texture, rect);
        }

        private void DrawColorPushButton(Texture2D texture, Rect rect, Direction direction)
        {
            var pivot = new Vector2(rect.x + rect.width / 2, rect.y + rect.height / 2);
            var rotation = direction switch
            {
                Direction.Up => 180f,
                Direction.Right => 270f,
                Direction.Down => 0f,
                Direction.Left => 90f,
                _ => 0f,
            };
            
            GUIUtility.RotateAroundPivot(rotation, pivot);
            GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit, true);
            GUIUtility.RotateAroundPivot(-rotation, pivot);
        }

        private void DrawGroundDefault(Texture2D texture, Rect rect)
        {
            if (texture == null) return;
            
            GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit, true);
        }

        public void DrawSolidObject(string key, Rect rect, Action onClick)
        {
            var texture = GetCellTexture(key, false);
            DrawSolidDefault(texture, rect, onClick);
        }

        private void DrawSolidDefault(Texture2D texture, Rect rect, Action onClick)
        {
            if (!GUI.Button(rect, texture, GUIStyle.none)) return;
            
            onClick?.Invoke();
        }
        
        private Texture2D GetCellTexture(string objectKey, bool isGround)
        {
            Texture2D texture = null;
            if (string.IsNullOrEmpty(objectKey)) return texture;

            if (_levelObjectsSo.LevelObjects.TryGetValue(objectKey, out var obj))
            {
                var prefab = obj.gameObject;
                if (prefab != null)
                    texture = prefab.GetComponentInChildren<SpriteRenderer>().sprite.texture;
            }

            return texture;
        }
    }
}