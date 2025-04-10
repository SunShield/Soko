using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Soko.Unity.DataLayer;
using Soko.Unity.Game.Level.Management;
using Soko.Unity.Game.Ui.Management;
using Soko.Unity.Game.Ui.Management.Wrapper;
using Soko.Unity.Game.Ui.Special.Focus;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Tutorials.Actions.Impl
{
    public class HighlightObjectAction : TutorialAction
    {
        private const int DefaultFocusSize = 150;
        private const int DefaultAutoCloseTime = 1;
        
        [ValueDropdown("GetObjectKeys")]
        [SerializeField] private string _levelObjectKey;
        
        [Inject] private UiManager _uiManager;
        [Inject] private LevelsManager _levelsManager;
        
        public override async UniTask Execute()
        {
            var objectToHighlight = _levelsManager.PlayCycleManager.LevelGrid.LevelObjects
                .First(lo => lo.PrefabKey == _levelObjectKey);
            if (objectToHighlight == null) return;

            await _uiManager.StartUiElementOpenProcess<FocusObjectScreenController>()
                .ConfigureElement(new FocusData(objectToHighlight.gameObject, DefaultFocusSize, DefaultAutoCloseTime))
                .FinishOpeningProcess()
                .AwaitForResult();
        }
        
#if UNITY_EDITOR
        private List<string> GetObjectKeys()
        {
            var levelObjectsSo = EditorDataProvider.Instance.LevelObjectsSo;
            return levelObjectsSo.LevelObjects.Keys.ToList();
        }
#endif
    }
}