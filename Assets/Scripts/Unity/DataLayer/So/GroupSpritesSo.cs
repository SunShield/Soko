using System.Collections.Generic;
using UnityEngine;

namespace Soko.Unity.DataLayer.So
{
    [CreateAssetMenu(fileName = "GroupSprites", menuName = "Data/Groups So", order = 3)]
    public class GroupSpritesSo : ScriptableObject
    {
        [field: SerializeField] public List<Sprite> GroupSprites { get; private set; }
    }
}