using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using VContainer.Unity;

namespace Soko.Unity.Game.DI
{
    [CreateAssetMenu(menuName = "DI/Installers", fileName = "Installers")]
    public class InstallersCollection : SerializedScriptableObject
    {
        [field: OdinSerialize] public List<IInstaller> Installers { get; private set; }
    }
}