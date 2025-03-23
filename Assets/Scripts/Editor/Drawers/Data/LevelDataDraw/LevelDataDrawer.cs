using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Soko.Core.Models.Levels;
using Soko.Editor.Data;
using Soko.Editor.Drawers.Data.LevelDataDraw.Elements;
using Soko.Unity;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects;
using UnityEditor;
using UnityEngine;

namespace Soko.Editor.Drawers.Data.LevelDataDraw
{
    public class LevelDataDrawer : OdinValueDrawer<LevelData>
    {
        private const string NameLabel = "Name";
        private const string LevelSizeLabel = "Size";
        private const string ResizeButtonLabel = "Resize";
        private const int CellSize = 50;
        private const int SolidLayerShift = 6;

        private readonly Dictionary<ObjectLayer, (int yOffset, int height, int fontSize)> _groupStyleSettings = new()
        {
            { ObjectLayer.Ground, (-1, 12, 14) },
            { ObjectLayer.Solid,  (17, 15, 20) },
        };

        private bool _expanded;
        private LevelDataTabsDrawer _tabsDrawer;
        private Vector2Int _newSize;
        private ObjectLayer _selectedLayer;

        private LevelData LevelData => ValueEntry.SmartValue;
        private LevelObjectsSo LevelObjectsSo => EditorDataProvider.Instance.LevelObjectsSo;
        private ColorDataSo ColorDataSo => EditorDataProvider.Instance.ColorDataSo;

        private string SelectedObjectKey => _tabsDrawer.SelectedObjectKey;
        private ObjectColor SelectedColor => _tabsDrawer.SelectedColor;
        private int SelectedGroup => _tabsDrawer.SelectedGroup;

        protected override void Initialize()
        {
            _tabsDrawer = new();
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            _expanded = SirenixEditorGUI.Foldout(_expanded, LevelData.Name);
            if (!_expanded) return;
            
            DrawLevelName();
            InitGridCellsIfNeeded();
            DrawGrid();
            DrawLayerSelector();
            _tabsDrawer.DrawTabs();
            DrawResizeControls();
        }

        private void DrawLevelName()
            => LevelData.Name = EditorGUILayout.TextField(NameLabel, LevelData.Name);

        private void DrawResizeControls()
        {
            _newSize = EditorGUILayout.Vector2IntField(LevelSizeLabel,
                new Vector2Int() 
                { 
                    x = _newSize.x == 0 ? LevelData.Cells.GetLength(1) : _newSize.x,
                    y = _newSize.y == 0 ? LevelData.Cells.GetLength(0) : _newSize.y
                });

            if (GUILayout.Button(ResizeButtonLabel)) ResizeLevel(LevelData, _newSize.y, _newSize.x);
        }
        
        private void ResizeLevel(LevelData level, int width, int height)
        {
            var newCells = new CellData[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    newCells[x, y] = (x < level.Cells.GetLength(0) && y < level.Cells.GetLength(1)) 
                        ? level.Cells[x, y] 
                        : new CellData();
            level.Cells = newCells;
        }

        private void InitGridCellsIfNeeded()
        {
            for (int x = 0; x < LevelData.Cells.GetLength(0); x++)
                for (int y = 0; y < LevelData.Cells.GetLength(1); y++)
                    LevelData.Cells[x, y] ??= new CellData();
        }

        private void DrawLayerSelector()
        {
            _selectedLayer = (ObjectLayer)EditorGUILayout.EnumPopup("Layer", _selectedLayer);
        }

        private void DrawGrid()
        {
            GUI.color = new Color(1f, 1f, 1f, 1f);
            for (int y = 0; y < LevelData.Cells.GetLength(1); y++)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < LevelData.Cells.GetLength(0); x++)
                {
                    var cell = LevelData.Cells[x, y];
                    var cellRect = GetCellRect();
                    
                    DrawCellBg(cellRect);
                    DrawCellGroundObject(cell, cellRect);
                    var hasGroundObject = !string.IsNullOrEmpty(cell.GroundObjectKey);
                    DrawCellColorIfNeeded(hasGroundObject, cell, cellRect, ObjectLayer.Ground);
                    
                    var buttonRect = GetCellSolidLayerRect(cellRect); 
                    DrawSolidObject(cell, buttonRect);

                    var hasSolidObject = !string.IsNullOrEmpty(cell.ObjectKey);
                    DrawCellColorIfNeeded(hasSolidObject, cell, buttonRect, ObjectLayer.Solid);
                    DrawGroupNumberIfNeeded(hasGroundObject, cell, cellRect, ObjectLayer.Ground);
                    DrawGroupNumberIfNeeded(hasSolidObject, cell, cellRect, ObjectLayer.Solid);
                }
                GUILayout.EndHorizontal();
            }
        }

        private Rect GetCellRect()
        {
            var cellRect = GUILayoutUtility.GetRect(CellSize, CellSize, GUILayout.Width(CellSize), 
                GUILayout.Height(CellSize));
            return cellRect;
        }

        private void DrawCellBg(Rect cellRect)
        {
            var bgTexture = GetBgTexture();
            GUI.DrawTexture(cellRect, bgTexture, ScaleMode.ScaleToFit, true);
        }
        
        private void DrawCellGroundObject(CellData cell, Rect cellRect)
        {
            var groundTexture = GetCellTexture(cell.GroundObjectKey, true);
            if (groundTexture != null)
            {
                GUI.DrawTexture(cellRect, groundTexture, ScaleMode.ScaleToFit, true);
            }
        }

        private void DrawCellColorIfNeeded(bool hasObject, CellData cell, Rect rect, ObjectLayer layer)
        {
            Func<ObjectColor> colorGetter = layer == ObjectLayer.Ground ? () => cell.GroundColor : () => cell.Color;
            if (hasObject && colorGetter() != ObjectColor.None && 
                ColorDataSo.ColorMap.TryGetValue(colorGetter(), out Color overlayColor))
                EditorGUI.DrawRect(rect, new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0.4f));
        }

        private Rect GetCellSolidLayerRect(Rect cellRect)
            => new (cellRect.x + SolidLayerShift, cellRect.y + SolidLayerShift, 
                cellRect.width - SolidLayerShift * 2, cellRect.height - SolidLayerShift * 2);

        private void DrawSolidObject(CellData cell, Rect buttonRect)
        {
            var solidTexture = GetCellTexture(cell.ObjectKey, false);
            if (GUI.Button(buttonRect, solidTexture, GUIStyle.none))
            {
                var isRightClick = Event.current.type == EventType.Used && Event.current.button == 1;
                HandleCellClick(cell, isRightClick);
            }
        }

        private void DrawGroupNumberIfNeeded(bool hasObject, CellData cell, Rect cellRect, ObjectLayer layer)
        {
            var hasGroup = layer == ObjectLayer.Ground ? cell.GroundGroup >= 0 : cell.Group >= 0;
            Func<int> groupGetter = layer == ObjectLayer.Ground ? () => cell.GroundGroup : () => cell.Group;
            
            if (!hasObject || !hasGroup) return;
            
            var yOffset = _groupStyleSettings[layer].yOffset;
            var numberHeight = _groupStyleSettings[layer].height;
            var fontSize = _groupStyleSettings[layer].fontSize;
            var groupTextRect = new Rect(cellRect.x, cellRect.y + yOffset, cellRect.width, numberHeight);
            var groupStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = fontSize,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };
            GUI.Label(groupTextRect, groupGetter().ToString(), groupStyle);
        }

        private Texture2D GetBgTexture() =>
            LevelObjectsSo.Empty.GetComponentInChildren<SpriteRenderer>().sprite.texture;

        private Texture2D GetCellTexture(string objectKey, bool isGround)
        {
            Texture2D texture = null;
            if (string.IsNullOrEmpty(objectKey)) return texture;

            if (LevelObjectsSo.LevelObjects.TryGetValue(objectKey, out var obj))
            {
                var prefab = obj.gameObject;
                if (prefab != null)
                    texture = prefab.GetComponentInChildren<SpriteRenderer>().sprite.texture;
            }

            return texture;
        }

        private void HandleCellClick(CellData cell, bool isRightClick)
        {
            var cellHasObject = CellHasObject(cell, _selectedLayer);
            switch (_tabsDrawer.SelectedTabKey)
            {
                case LevelDataTabsDrawer.ColorsTabName:
                {
                    if (isRightClick)       cell.SetColor(ObjectColor.None, _selectedLayer);
                    else if (cellHasObject) cell.SetColor(SelectedColor, _selectedLayer);
                    break;
                }
                case LevelDataTabsDrawer.GroupsTabName:
                {
                    if (isRightClick)       cell.SetGroup(UnityConstants.Level.NoBindingGroup, _selectedLayer);
                    else if (cellHasObject) cell.SetGroup(SelectedGroup - 1, _selectedLayer);
                    break;
                }
                default:
                    cell.SetColor(ObjectColor.None, _selectedLayer);
                    cell.SetGroup(UnityConstants.Level.NoBindingGroup, _selectedLayer);
                    if (isRightClick)
                        cell.SetObjectKey("", _selectedLayer);
                    else if (LevelObjectsSo.LevelObjects.TryGetValue(SelectedObjectKey, out var obj))
                        cell.SetObjectKey(SelectedObjectKey, _selectedLayer);
                    break;
            }
        }

        private bool CellHasObject(CellData cell, ObjectLayer layer) => layer switch
        {
            ObjectLayer.Ground => !string.IsNullOrEmpty(cell.GroundObjectKey),
            ObjectLayer.Solid => !string.IsNullOrEmpty(cell.ObjectKey),
        };
    }
}
