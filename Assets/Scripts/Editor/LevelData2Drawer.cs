using System;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Soko.Core.Models.Levels;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Level.Grid.Enums;
using Soko.Unity.Game.Level.Grid.Objects;
using UnityEditor;
using UnityEngine;

namespace Soko.Editor
{
    public class LevelData2Drawer : OdinValueDrawer<LevelData2>
    {
        private string selectedObjectKey = "";
        private LevelObjectsSo objectDatabase;
        private ColorDataSo colorData;
        private bool colorMode = false; // Toggle for color mode
        private ObjectColor selectedColor = ObjectColor.None; // Selected color
        private bool groupMode = false; // Toggle for group mode
        private int selectedGroup = 0;  // Default group

        private int newWidth;
        private int newHeight;

        protected override void DrawPropertyLayout(GUIContent label)
        {
            LevelData2 level = ValueEntry.SmartValue;

            if (level.Cells == null || level.Cells.Length == 0)
            {
                level.Cells = new CellData[1, 1];
            }

            // Ensure no null values inside the grid
            for (int x = 0; x < level.Cells.GetLength(0); x++)
            {
                for (int y = 0; y < level.Cells.GetLength(1); y++)
                {
                    level.Cells[x, y] ??= new CellData();
                }
            }

            if (objectDatabase == null)
            {
                objectDatabase = AssetDatabase.LoadAssetAtPath<LevelObjectsSo>("Assets/Data/Game Data/LevelObjects.asset");
            }

            if (colorData == null)
            {
                colorData = AssetDatabase.LoadAssetAtPath<ColorDataSo>("Assets/Data/Game Data/ColorData.asset");
            }

            // Box Group for Object Mode
            SirenixEditorGUI.BeginBox("Object Mode");
            DrawObjectSelector();
            SirenixEditorGUI.EndBox();

            GUILayout.Space(10);

            // Box Group for Color Mode
            SirenixEditorGUI.BeginBox("Color Mode");
            DrawColorSelector();
            SirenixEditorGUI.EndBox();

            GUILayout.Space(10);

            // Box Group for Group Mode
            SirenixEditorGUI.BeginBox("Group Mode");
            DrawGroupSelector();
            SirenixEditorGUI.EndBox();

            DrawGrid(level);
            DrawResizeControls(level);
        }

        /// <summary>
        /// Object selection UI.
        /// </summary>
        private void DrawObjectSelector()
        {
            if (objectDatabase == null)
            {
                EditorGUILayout.HelpBox("No LevelObjectsSo found! Assign it manually.", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField("Object Selector", EditorStyles.boldLabel);

            string[] keys = new string[objectDatabase.LevelObjects.Count + 1];
            keys[0] = "(None)";
            objectDatabase.LevelObjects.Keys.CopyTo(keys, 1);

            int selectedIndex = Array.IndexOf(keys, selectedObjectKey);
            if (selectedIndex == -1) selectedIndex = 0;

            selectedIndex = EditorGUILayout.Popup("Select Object", selectedIndex, keys);
            selectedObjectKey = keys[selectedIndex];
        }

        /// <summary>
        /// Color selection UI.
        /// </summary>
        private void DrawColorSelector()
        {
            if (colorData == null)
            {
                EditorGUILayout.HelpBox("No ColorDataSo found! Assign it manually.", MessageType.Warning);
                return;
            }

            colorMode = EditorGUILayout.Toggle("Enable Color Mode", colorMode);

            if (colorMode)
            {
                var colorKeys = Enum.GetValues(typeof(ObjectColor)).Cast<ObjectColor>().ToArray();
                int selectedIndex = Array.IndexOf(colorKeys, selectedColor);
                selectedIndex = EditorGUILayout.Popup("Select Color", selectedIndex, colorKeys.Select(c => c.ToString()).ToArray());
                selectedColor = colorKeys[selectedIndex];
            }
        }
        
        private void DrawGroupSelector()
        {
            groupMode = EditorGUILayout.Toggle("Enable Group Mode", groupMode);

            if (groupMode)
            {
                string[] groupOptions = { "-1", "0", "1", "2", "3" };
                selectedGroup = EditorGUILayout.Popup("Select Group", selectedGroup, groupOptions);
            }
        }

        private void DrawGrid(LevelData2 level)
        {
            GUILayout.Label("Level Grid", EditorStyles.boldLabel);

            int cellSize = 50;
            int gapSize = 0;

            for (int y = 0; y < level.Cells.GetLength(1); y++)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < level.Cells.GetLength(0); x++)
                {
                    CellData cell = level.Cells[x, y];

                    Texture2D iconTexture = Texture2D.whiteTexture; // Default empty cell
                    bool hasObject = !string.IsNullOrEmpty(cell.ObjectKey);

                    if (objectDatabase != null && hasObject &&
                        objectDatabase.LevelObjects.TryGetValue(cell.ObjectKey, out LevelObjectBase obj))
                    {
                        GameObject prefab = obj.gameObject;
                        if (prefab != null)
                        {
                            // Get preview of the prefab
                            iconTexture = AssetPreview.GetAssetPreview(prefab) ?? AssetPreview.GetMiniThumbnail(prefab);
                        }
                    }
                    else
                    {
                        iconTexture = AssetPreview.GetAssetPreview(objectDatabase.Empty) 
                                      ?? AssetPreview.GetMiniThumbnail(objectDatabase.Empty);
                    }

                    GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
                    {
                        padding = new RectOffset(0, 0, 0, 0),
                        margin = new RectOffset(gapSize, gapSize, gapSize, gapSize)
                    };

                    // Detect Mouse Click
                    var e = Event.current;

                    if (GUILayout.Button(iconTexture, buttonStyle, GUILayout.Width(cellSize), GUILayout.Height(cellSize)))
                    {
                        var isRightClick = e.type == EventType.Used && e.button == 1;
                        HandleCellClick(cell, isRightClick);
                    }

                    Rect lastRect = GUILayoutUtility.GetLastRect();

                    // Rule: Only draw color overlay if an object exists
                    if (hasObject && cell.Color != ObjectColor.None && colorData.ColorMap.TryGetValue(cell.Color, out Color overlayColor))
                    {
                        EditorGUI.DrawRect(lastRect, new Color(overlayColor.r, overlayColor.g, overlayColor.b, 0.4f));
                    }

                    // Draw Group Number Above Object
                    if (hasObject && cell.Group >= 1)
                    {
                        var groupTextRect = new Rect(lastRect.x, lastRect.y + 17, lastRect.width, 15);
                        GUIStyle groupStyle = new GUIStyle(EditorStyles.boldLabel)
                        {
                            alignment = TextAnchor.MiddleCenter,
                            fontSize = 20,
                            fontStyle = FontStyle.Bold,
                            normal = { textColor = Color.white }
                        };

                        GUI.Label(groupTextRect, cell.Group.ToString(), groupStyle);
                    }
                }
                GUILayout.EndHorizontal();
            }
        }

        private void HandleCellClick(CellData cell, bool isRightClick)
        {
            if (isRightClick)
            {
                // Right-click: Remove object, color, and group
                cell.ObjectKey = "";
                cell.Color = ObjectColor.None;
                cell.Group = -1; // Reset group
            }
            else if (colorMode)
            {
                // Left-click in Color Mode: Only apply color if an object exists
                if (!string.IsNullOrEmpty(cell.ObjectKey))
                {
                    cell.Color = selectedColor;
                }
            }
            else if (groupMode)
            {
                // Left-click in Group Mode: Only apply group if an object exists
                if (!string.IsNullOrEmpty(cell.ObjectKey))
                {
                    cell.Group = selectedGroup;
                }
            }
            else
            {
                // Left-click in Object Mode: Place object
                if (!string.IsNullOrEmpty(selectedObjectKey))
                {
                    cell.ObjectKey = selectedObjectKey;
                }
            }
        }

        private void DrawResizeControls(LevelData2 level)
        {
            GUILayout.Label("Resize Level", EditorStyles.boldLabel);

            newWidth = EditorGUILayout.IntField("New Width", newWidth == 0 ? level.Cells.GetLength(0) : newWidth);
            newHeight = EditorGUILayout.IntField("New Height", newHeight == 0 ? level.Cells.GetLength(1) : newHeight);

            if (GUILayout.Button("Resize Level"))
            {
                ResizeLevel(level, newWidth, newHeight);
            }
        }

        private void ResizeLevel(LevelData2 level, int newWidth, int newHeight)
        {
            if (newWidth < 1 || newHeight < 1)
            {
                Debug.LogError("Invalid level size. Must be at least 1x1.");
                return;
            }

            CellData[,] newCells = new CellData[newWidth, newHeight];

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    newCells[x, y] = (x < level.Cells.GetLength(0) && y < level.Cells.GetLength(1)) ? level.Cells[x, y] : new CellData();
                }
            }

            level.Cells = newCells;
        }
    }
}
