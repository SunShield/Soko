using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Soko.Unity.Game.Level.Grid.Objects.Components.Impl
{
    public class GroupComponent : LevelObjectComponent
    {
        public const int NoGroup = -1; 
        
        [SerializeField] private GameObject _groupInfoBlock;
        [SerializeField] private TextMeshPro _groupText;

        public List<LevelObjectBase> GroupObjects { get; private set; } = new();

        public int Group { get; private set; }
        
        public void SetGroup(int group)
        {
            Group = group;
            _groupInfoBlock.SetActive(group != NoGroup);
            _groupText.text = group.ToString();
        }

        public void AddObject(LevelObjectBase levelObject) => GroupObjects.Add(levelObject);
    }
}