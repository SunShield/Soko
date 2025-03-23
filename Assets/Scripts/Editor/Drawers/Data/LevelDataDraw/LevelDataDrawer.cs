using System;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Soko.Core.Models.Levels;
using Soko.Editor.Data;
using Soko.Editor.Drawers.Data.LevelDataDraw.Elements;
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
        private const int GroupNumberYOffset = 17;
        private const int GroupNumberHeight = 15;
        private const int GroupNumberFontSize = 20;

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
                    var hasObject = !string.IsNullOrEmpty(cell.ObjectKey);
                    var buttonStyle = CreateDefaultButtonStyle();
                    
                    var cellRect = GUILayoutUtility.GetRect(CellSize, CellSize, GUILayout.Width(CellSize), 
                        GUILayout.Height(CellSize));

                    var bgTexture = GetBgTexture();
                    GUI.DrawTexture(cellRect, bgTexture, ScaleMode.ScaleToFit, true);
                    
                    var groundTexture = GetCellTexture(cell.GroundObjectKey, true);
                    if (groundTexture != null)
                    {
                        GUI.DrawTexture(cellRect, groundTexture, ScaleMode.ScaleToFit, true);
                    }
                    
                    DrawCellColorBackgroundIfNeeded(!string.IsNullOrEmpty(cell.GroundObjectKey), cell, cellRect, 
                        ObjectLayer.Ground);
                    
                    var solidTexture = GetCellTexture(cell.ObjectKey, false);
                    var buttonRect = new Rect(cellRect.x + 6, cellRect.y + 6, cellRect.width - 12, cellRect.height - 12); 
                    if (GUI.Button(buttonRect, solidTexture, GUIStyle.none))
                    {
                        var isRightClick = Event.current.type == EventType.Used && Event.current.button == 1;
                        HandleCellClick(cell, isRightClick);
                    }
                    
                    DrawCellColorBackgroundIfNeeded(!string.IsNullOrEmpty(cell.ObjectKey), cell, buttonRect, 
                        ObjectLayer.Solid);
                    DrawGroupNumberIfNeeded(!string.IsNullOrEmpty(cell.GroundObjectKey), cell, cellRect, ObjectLayer.Ground);
                    DrawGroupNumberIfNeeded(!string.IsNullOrEmpty(cell.ObjectKey), cell, cellRect, ObjectLayer.Solid);
                }
                GUILayout.EndHorizontal();
            }
        }

        private void DrawCellColorBackgroundIfNeeded(bool hasObject, CellData cell, Rect rect, ObjectLayer layer)
        {
            Func<ObjectColor> colorGetter = layer == ObjectLayer.Ground ? () => cell.GroundColor : () => cell.Color; 
            
            if (hasObject && colorGetter() != ObjectColor.None && 
                ColorDataSo.ColorMap.TryGetValue(colorGetter(), out Color overlayColor))
                EditorGUI.DrawRect(rect, new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0.4f));
        }

        private void DrawGroupNumberIfNeeded(bool hasObject, CellData cell, Rect cellRect, ObjectLayer layer)
        {
            var hasGroup = layer == ObjectLayer.Ground ? cell.GroundGroup >= 0 : cell.Group >= 0;
            
            if (!hasObject || !hasGroup) return;

            if (layer == ObjectLayer.Ground)
            {
                var groupTextRect = new Rect(cellRect.x, cellRect.y + GroupNumberYOffset + 18, cellRect.width, 14);
                var groupStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 12,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.white }
                };
                GUI.Label(groupTextRect, cell.GroundGroup.ToString(), groupStyle);
            }
            else
            {
                var groupTextRect = new Rect(cellRect.x, cellRect.y + GroupNumberYOffset, cellRect.width, GroupNumberHeight);
                var groupStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = GroupNumberFontSize,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.white }
                };
                GUI.Label(groupTextRect, cell.Group.ToString(), groupStyle);
            }
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

        private GUIStyle CreateDefaultButtonStyle()
            => new (GUI.skin.button) { padding = new RectOffset(0, 0, 0, 0), margin = new RectOffset(0, 0, 0, 0) };

        private void HandleCellClick(CellData cell, bool isRightClick)
        {
            var cellHasObject = CellHasObject(cell, _selectedLayer);
            switch (_tabsDrawer.SelectedTabKey)
            {
                case LevelDataTabsDrawer.ColorsTabName:
                {
                    if (isRightClick)
                    {
                        if (_selectedLayer == ObjectLayer.Ground) cell.GroundColor = ObjectColor.None;
                        else                                      cell.Color = ObjectColor.None;
                    }
                    else if (cellHasObject)
                    {
                        if (_selectedLayer == ObjectLayer.Ground) cell.GroundColor = SelectedColor;
                        else                                      cell.Color = SelectedColor;
                    }
                    break;
                }
                case LevelDataTabsDrawer.GroupsTabName:
                {
                    if (isRightClick)
                    {
                        if (_selectedLayer == ObjectLayer.Ground) cell.GroundGroup = -1;
                        else                                      cell.Group = -1;
                        
                    }
                    else if (cellHasObject)
                    {
                        if (_selectedLayer == ObjectLayer.Ground) cell.GroundGroup = SelectedGroup - 1;
                        else                                      cell.Group = SelectedGroup - 1;
                    }
                    break;
                }
                default:
                    if (isRightClick)
                    {
                        if (_selectedLayer == ObjectLayer.Ground)
                        {
                            cell.GroundColor = ObjectColor.None;
                            cell.GroundGroup = -1;
                            cell.GroundObjectKey = "";
                        }
                        else
                        {
                            cell.Color = ObjectColor.None;
                            cell.Group = -1;
                            cell.ObjectKey = "";
                        }
                    }
                    else if (LevelObjectsSo.LevelObjects.TryGetValue(SelectedObjectKey, out var obj))
                    {
                        switch (obj.Layer)
                        {
                            case ObjectLayer.Ground:
                                cell.GroundColor = ObjectColor.None;
                                cell.GroundGroup = -1;
                                cell.GroundObjectKey = SelectedObjectKey;
                                break;
                            case ObjectLayer.Solid:
                                cell.Color = ObjectColor.None;
                                cell.Group = -1;
                                cell.ObjectKey = SelectedObjectKey;
                                break;
                        }
                    }
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
