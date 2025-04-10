using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Soko.Unity.Game.Ui.Enums;
using Soko.Unity.Game.Ui.Management;
using Soko.Unity.Game.Ui.Management.Elements;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Tutorials.Actions.Impl
{
    public class OpenUiElementAction : TutorialAction
    {
        private const string ElementOpenMethod = "SimpleOpenUiElement";
        
        [Inject] private UiManager _uiManager;
        
        [ValueDropdown("GetUiElementTypes")]
        [SerializeField] private Type _uiElementType; 
        
        public override async UniTask Execute()
        {
            // this is simpler than creating the whole thing with Type passing instead of generic   
            var method = typeof(UiManager)
                .GetMethods()
                .FirstOrDefault(m => m.Name == ElementOpenMethod);
            var genericMethod = method.MakeGenericMethod(_uiElementType);
            genericMethod.Invoke(_uiManager, new [] { Type.Missing });
            await UniTask.WaitUntil(() => _uiManager.GetUiElementState(_uiElementType) == UiElementState.Inactive);
        }

#if UNITY_EDITOR
        private static IEnumerable<ValueDropdownItem<Type>> GetUiElementTypes()
        {
            var baseType = typeof(UiElement);
            return baseType.Assembly.GetTypes()
                .Where(t => baseType.IsAssignableFrom(t) && t != baseType && !t.IsAbstract)
                .Select(t => new ValueDropdownItem<Type>(t.Name, t));
        }
#endif
    }
}