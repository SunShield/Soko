using VContainer;

namespace Soko.Unity.Game.DI.Scopes.Base
{
    /// <summary>
    /// A bit hacky but very convenient solution which allow to UI elements (created by UI manager registered in
    /// game scope) to inject classes registered in child scopes like LevelScope.
    /// </summary>
    public class CurrentScopeProvider
    {
        private static CurrentScopeProvider _instance;
        protected CurrentScopeProvider() { }
        public static CurrentScopeProvider Instance => _instance ??= new();
        
        public IObjectResolver CurrentScope { get; set; }
    }
}