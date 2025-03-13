using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Soko.Unity.Game.Sounds;
using UnityEngine;

namespace Soko.Unity.DataLayer.So
{
    [CreateAssetMenu(fileName = "Sounds", menuName = "Data/Sounds", order = 4)]
    public class SoundSo : SerializedScriptableObject
    {
        [field: OdinSerialize] public Dictionary<string, AudioClip> Music { get; set; }
        [field: OdinSerialize] public Dictionary<GameSfx, AudioClip> Sfx { get; set; }
    }
}