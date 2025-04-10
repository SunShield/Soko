using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Soko.Unity.Game.Service.Conditions;
using Soko.Unity.Game.Tutorials.Actions;

namespace Soko.Unity.Game.Tutorials
{
    [Serializable][HideReferenceObjectPicker]
    public class TutorialSequence
    {
        public string Key;
        [GUIColor("#CCCCEE")] public List<AbstractCondition> Conditions = new();
        [GUIColor("#EECCCC")] public List<TutorialAction> Actions = new();
    }
}