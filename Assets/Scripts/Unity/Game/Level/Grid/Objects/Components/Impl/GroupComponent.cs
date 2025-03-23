using System.Collections.Generic;
using Soko.Unity.DataLayer.So;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class GroupComponent : LevelObjectComponent
    {
        [SerializeField] private GameObject _groupInfoBlock;
        [SerializeField] private SpriteRenderer _groupSprite;

        [Inject] private GroupSpritesSo _groupSpritesSo;

        public List<LevelObjectBase> GroupObjects { get; private set; } = new();

        public int Group { get; private set; }
        
        public void SetGroup(int group)
        {
            Group = group;
            _groupInfoBlock.SetActive(group != UnityConstants.Level.NoBindingGroup);
            
            if (group == UnityConstants.Level.NoBindingGroup) return;
            
            _groupSprite.sprite = _groupSpritesSo.GroupSprites[group];
        }

        public void AddObject(LevelObjectBase levelObject) => GroupObjects.Add(levelObject);
    }
}