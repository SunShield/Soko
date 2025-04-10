using System.Collections.Generic;

namespace Soko.Unity.Game.Save.Impl.Tutorial
{
    public class TutorialSaveData : AbstractSaveData
    {
        public HashSet<string> CompletedTutorials = new();
    }
}