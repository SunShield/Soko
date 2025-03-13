using Soko.Unity.DataLayer.So;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Sounds
{
    public class SoundsManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicAudioSource;
        
        [Inject] private SoundSo _soundSo;
        
        [Inject] private void Construct()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(true);
        }

        public void PlayMusic(string musicName)
        {
            var music = _soundSo.Music[musicName];
            _musicAudioSource.clip = music;
            _musicAudioSource.Play();
        }
    }
}