using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities.Editor;

namespace Soko.Editor.Service.Drawers
{
    public class TabsGroupDrawer
    {
        private readonly List<string> _tabNames;
        private readonly List<Action> _tabContentDrawers;
        
        public string SelectedTabKey { get; private set; }

        public TabsGroupDrawer(List<string> tabNames, List<Action> tabContentDrawers)
        {
            _tabNames = tabNames;
            _tabContentDrawers = tabContentDrawers;
            SelectedTabKey = tabNames[0];
        }
        
        public void DrawTabGroup()
        {
            var tabGroup = CreateTabsGroup();
            var tabs = RegisterTabs(tabGroup);
            
            tabGroup.BeginGroup(drawToolbar: true);
            for (int i = 0; i < tabs.Count; i++)
            {
                var tab = tabs[i];
                if (tab.BeginPage()) 
                    ProcessSelectedTab(i);
                tab.EndPage();
            }
            tabGroup.EndGroup();
        }

        private GUITabGroup CreateTabsGroup() => SirenixEditorGUI.CreateAnimatedTabGroup(_tabNames[0]);
        private List<GUITabPage> RegisterTabs(GUITabGroup tabGroup) =>  _tabNames.Select(tabGroup.RegisterTab).ToList();

        private void ProcessSelectedTab(int tabIndex)
        {
            SelectedTabKey = _tabNames[tabIndex];
            _tabContentDrawers[tabIndex].Invoke();
        }
    }
}