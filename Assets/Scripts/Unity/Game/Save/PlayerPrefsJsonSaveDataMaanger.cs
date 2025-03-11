using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Soko.Unity.Game.Save
{
    public abstract class PlayerPrefsJsonSaveManager<TSaveData> : AbstractSaveManager<TSaveData>
        where TSaveData : AbstractSaveData
    {
        protected abstract string PrefsKey { get; }

        protected override TSaveData Load()
        {
            var saveDataRaw = PlayerPrefs.GetString(PrefsKey);
            var saveData = JsonConvert.DeserializeObject<TSaveData>(saveDataRaw);
            return saveData;
        }

        protected override TSaveData Create()
        {
            var saveData = Activator.CreateInstance<TSaveData>();
            return saveData;
        }

        public override void Save()
        {
            var saveDataRow = JsonConvert.SerializeObject(SaveDataInternal);
            PlayerPrefs.SetString(PrefsKey, saveDataRow);
        }
    }
}