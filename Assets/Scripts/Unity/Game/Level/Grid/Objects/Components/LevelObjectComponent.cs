using System.Threading.Tasks;
using UnityEngine;

namespace Soko.Unity.Game.Level.Grid.Objects.Components
{
    public abstract class LevelObjectComponent : MonoBehaviour
    {
        public async Task OnObjectAboutToEnter(LevelObjectBase enteringObject)
        {
            
        }
    }
}