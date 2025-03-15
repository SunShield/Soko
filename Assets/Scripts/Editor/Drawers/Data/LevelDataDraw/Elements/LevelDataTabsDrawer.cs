using System;
using System.Collections.Generic;
using System.Linq;
using Soko.Editor.Data;
using Soko.Editor.Service.Drawers;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Level.Grid.Enums;
using UnityEditor;

namespace Soko.Editor.Drawers.Data.LevelDataDraw.Elements
{
    public class LevelDataTabsDrawer
    {
        public const string ObjectsTabName = "Obects";
        public const string ColorsTabName = "Colors";
        public const string GroupsTabName = "Groups";
        private readonly string[] Groups = { "-1", "0", "1", "2", "3" };
        
        private readonly TabsGroupDrawer _tabsGroupDrawer;
        
        public string SelectedObjectKey { get; private set; }
        public ObjectColor SelectedColor { get; private set; }
        public int SelectedGroup { get; private set; }
        public string SelectedTabKey => _tabsGroupDrawer.SelectedTabKey;
        private LevelObjectsSo LevelObjectsSo => EditorDataProvider.Instance.LevelObjectsSo;

        public LevelDataTabsDrawer()
        {
            _tabsGroupDrawer = new (
                new List<string>() { ObjectsTabName, ColorsTabName, GroupsTabName },
                new List<Action>() { DrawObjectSelector, DrawColorSelector, DrawGroupSelector } );
        }

        public void DrawTabs() => _tabsGroupDrawer.DrawTabGroup();
        
        private void DrawObjectSelector()
        {
            var keys = new string[LevelObjectsSo.LevelObjects.Count + 1];
            LevelObjectsSo.LevelObjects.Keys.CopyTo(keys, 1);

            var selectedIndex = Array.IndexOf(keys, SelectedObjectKey);
            if (selectedIndex == -1) selectedIndex = 0;

            selectedIndex = EditorGUILayout.Popup(ObjectsTabName, selectedIndex, keys);
            SelectedObjectKey = keys[selectedIndex];
        }

        private void DrawColorSelector()
        {
            var colorKeys = Enum.GetValues(typeof(ObjectColor)).Cast<ObjectColor>().ToArray();
            var selectedIndex = Array.IndexOf(colorKeys, SelectedColor);
            selectedIndex = EditorGUILayout.Popup(ColorsTabName, selectedIndex, colorKeys
                .Select(c => c.ToString())
                .ToArray());
            SelectedColor = colorKeys[selectedIndex];
        }
        
        private void DrawGroupSelector()
        {
            SelectedGroup = EditorGUILayout.Popup(GroupsTabName, SelectedGroup, Groups);
        }
    }
}