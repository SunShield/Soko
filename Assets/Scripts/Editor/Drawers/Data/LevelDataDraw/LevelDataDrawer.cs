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
    public class LevelDataDrawer : OdinValueDrawer<LevelData2>
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

        private LevelData2 LevelData => ValueEntry.SmartValue;
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
        
        private void ResizeLevel(LevelData2 level, int width, int height)
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

        private void DrawGrid()
        {
            for (int y = 0; y < LevelData.Cells.GetLength(1); y++)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < LevelData.Cells.GetLength(0); x++)
                {
                    var cell = LevelData.Cells[x, y];
                    var hasObject = !string.IsNullOrEmpty(cell.ObjectKey);
                    var cellTexture = GetCellTexture(hasObject, cell);
                    var buttonStyle = CreateDefaultButtonStyle();
                    
                    DrawCell(cellTexture, buttonStyle, cell);
                    
                    var lastRect = GUILayoutUtility.GetLastRect();
                    DrawCellColorBackgroundIfNeeded(hasObject, cell, lastRect);
                    DrawGroupNumberIfNeeded(hasObject, cell, lastRect);
                }
                GUILayout.EndHorizontal();
            }
        }

        private void DrawCell(Texture2D cellTexture, GUIStyle buttonStyle, CellData cell)
        {
            if (!GUILayout.Button(cellTexture, buttonStyle, GUILayout.Width(CellSize), GUILayout.Height(CellSize))) 
                return;
            var isRightClick = Event.current.type == EventType.Used && Event.current.button == 1;
            HandleCellClick(cell, isRightClick);
        }

        private void DrawCellColorBackgroundIfNeeded(bool hasObject, CellData cell, Rect lastRect)
        {
            if (hasObject && cell.Color != ObjectColor.None && 
                ColorDataSo.ColorMap.TryGetValue(cell.Color, out Color overlayColor))
                EditorGUI.DrawRect(lastRect, new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0.4f));
        }

        private void DrawGroupNumberIfNeeded(bool hasObject, CellData cell, Rect cellRect)
        {
            if (!hasObject || cell.Group < 0) return;
            
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

        private Texture2D GetCellTexture(bool hasObject, CellData cell)
        {
            var iconTexture = AssetPreview.GetAssetPreview(LevelObjectsSo.Empty);

            if (LevelObjectsSo != null && hasObject &&
                LevelObjectsSo.LevelObjects.TryGetValue(cell.ObjectKey, out LevelObjectBase obj))
            {
                var prefab = obj.gameObject;
                if (prefab != null)
                    iconTexture = AssetPreview.GetAssetPreview(prefab) ?? AssetPreview.GetMiniThumbnail(prefab);
            }

            return iconTexture;
        }

        private GUIStyle CreateDefaultButtonStyle()
            => new (GUI.skin.button) { padding = new RectOffset(0, 0, 0, 0), margin = new RectOffset(0, 0, 0, 0) };

        private void HandleCellClick(CellData cell, bool isRightClick)
        {
            var cellHasObject = CellHasObject(cell);
            switch (_tabsDrawer.SelectedTabKey)
            {
                case LevelDataTabsDrawer.ColorsTabName:
                {
                    if (isRightClick)       cell.Color = ObjectColor.None;
                    else if (cellHasObject) cell.Color = SelectedColor;
                    break;
                }
                case LevelDataTabsDrawer.GroupsTabName:
                {
                    if (isRightClick)       cell.Group = -1;
                    else if (cellHasObject) cell.Group = SelectedGroup - 1;
                    break;
                }
                default:
                    cell.Color = ObjectColor.None;
                    cell.Group = -1;
                    cell.ObjectKey = isRightClick ? "" : SelectedObjectKey;
                    break;
            }
        }

        private bool CellHasObject(CellData cell) => !string.IsNullOrEmpty(cell.ObjectKey);
    }
}
