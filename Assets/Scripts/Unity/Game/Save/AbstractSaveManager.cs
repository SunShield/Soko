namespace Soko.Unity.Game.Save
{
    public abstract class AbstractSaveManager<TSaveData>
        where TSaveData : AbstractSaveData
    {
        protected TSaveData SaveDataInternal;
        public TSaveData SaveData
        {
            get
            {
                if (SaveDataInternal != null) return SaveDataInternal;
                SaveDataInternal = Load();
                
                if (SaveDataInternal != null) return SaveDataInternal;
                SaveDataInternal = Create();
                Save();

                return SaveDataInternal;
            }
        }

        protected abstract TSaveData Create();
        protected abstract TSaveData Load();
        public abstract void Save();
    }
}