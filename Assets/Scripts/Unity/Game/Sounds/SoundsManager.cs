using System.Collections.Generic;
using Soko.Unity.DataLayer.So;
using UnityEngine;
using VContainer;

namespace Soko.Unity.Game.Sounds
{
    public class SoundsManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _musicAudioSource;
        
        [Inject] private SoundSo _soundSo;
        
        private readonly Dictionary<GameSfx, AudioSource> _audioSources = new ();
        
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

        public void PlaySfx(GameSfx sfxKey)
        {
            if (!_audioSources.ContainsKey(sfxKey))
            {
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.clip = _soundSo.Sfx[sfxKey];
                _audioSources.Add(sfxKey, audioSource);
            }
            
            _audioSources[sfxKey].Play();
        }
    }
}