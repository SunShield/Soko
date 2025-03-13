using System.Collections.Generic;
using Soko.Unity.DataLayer.So;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class GroupComponent : LevelObjectComponent
    {
        public const int NoGroup = -1; 
        
        [SerializeField] private GameObject _groupInfoBlock;
        [SerializeField] private SpriteRenderer _groupSprite;

        [Inject] private GroupSpritesSo _groupSpritesSo;

        public List<LevelObjectBase> GroupObjects { get; private set; } = new();

        public int Group { get; private set; }
        
        public void SetGroup(int group)
        {
            Group = group;
            _groupInfoBlock.SetActive(group != NoGroup);
            
            if (group == NoGroup) return;
            
            _groupSprite.sprite = _groupSpritesSo.GroupSprites[group];
        }

        public void AddObject(LevelObjectBase levelObject) => GroupObjects.Add(levelObject);
    }
}