using System;
using System.Collections.Generic;
using Soko.Unity.Game.Tutorials.Actions;

namespace Soko.Unity.Game.Tutorials
{
    [Serializable]
    public class TutorialSequence
    {
        public List<TutorialAction> Actions = new();
    }
}