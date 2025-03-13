using UnityEngine;

namespace Soko.Unity.Game.Level.Visuals
{
    public class LevelBackgroundManager : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _background;
        
        public void SetBackground(Sprite background) => _background.sprite = background;
    }
}