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
            var tabGroup = SirenixEditorGUI.CreateAnimatedTabGroup(_tabNames[0]);
            var tabs = _tabNames.Select(name => tabGroup.RegisterTab(name)).ToList();
            
            tabGroup.BeginGroup(drawToolbar: true);
            
            for (int i = 0; i < tabs.Count; i++)
            {
                var tab = tabs[i];
                if (tab.BeginPage())
                {
                    SelectedTabKey = _tabNames[i];
                    _tabContentDrawers[i].Invoke();
                }
                
                tab.EndPage();
            }
            
            tabGroup.EndGroup();
        }
    }
}