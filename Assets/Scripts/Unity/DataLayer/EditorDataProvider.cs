using Soko.Unity.DataLayer.So;
using UnityEditor;

namespace Soko.Unity.DataLayer
{
    public class EditorDataProvider
    {
        private const string LevelObjectsSoPath = @"Assets/Data/Game Data/LevelObjects.asset";
        private const string ColorObjectsSoPath = @"Assets/Data/Game Data/ColorData.asset";
        
        private static EditorDataProvider _instance;
        protected EditorDataProvider() { }
        public static EditorDataProvider Instance => _instance ??= new();
        
        private LevelObjectsSo _levelObjectsSo;
        private ColorDataSo _colorDataSo;
        
        public LevelObjectsSo LevelObjectsSo => _levelObjectsSo ??= GetLevelObjectsSo();
        public ColorDataSo ColorDataSo => _colorDataSo ??= GetColorDataSo();
        
#if UNITY_EDITOR
        private LevelObjectsSo GetLevelObjectsSo()
            => AssetDatabase.LoadAssetAtPath<LevelObjectsSo>(LevelObjectsSoPath);
        private ColorDataSo GetColorDataSo()
            => AssetDatabase.LoadAssetAtPath<ColorDataSo>(ColorObjectsSoPath);
#endif
    }
}