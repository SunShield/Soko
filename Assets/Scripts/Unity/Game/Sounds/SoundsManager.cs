using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Soko.Unity.DataLayer.So;
using Soko.Unity.Game.Save.Impl.User;
using UnityEngine;
using UnityEngine.Audio;
using VContainer;

namespace Soko.Unity.Game.Sounds
{
    public class SoundsManager : MonoBehaviour
    {
        [SerializeField] private AudioMixerGroup _sfxGroup;
        [SerializeField] private AudioSource _musicAudioSource;
        
        [Inject] private SoundSo _soundSo;
        [Inject] private UserSaveDataManager _userSaveDataManager;
        
        private readonly Dictionary<GameSfx, AudioSource> _audioSources = new ();

        public bool MusicOn => _userSaveDataManager.SaveData.SoundSettings.MusicOn;
        public bool SfxOn => _userSaveDataManager.SaveData.SoundSettings.SfxOn;
        
        [Inject] 
        private void Construct()
        {
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(true);
            StartCoroutine(ApplySoundSettings());
        }

        private IEnumerator ApplySoundSettings()
        {
            yield return null;
            SetMusicOn(MusicOn);
            SetSfxOn(SfxOn);
        }

        public void SetMusicOn(bool on)
        {
            var shit = Mathf.Log(0.0001f) * 20;
            _sfxGroup.audioMixer.SetFloat(UnityConstants.Audio.Params.MusicVolumeFloatKey, on ? 0f : -80f);
            
            _userSaveDataManager.SaveData.SoundSettings.MusicOn = on;
            _userSaveDataManager.Save();
        }

        public void SetSfxOn(bool on)
        {
            _sfxGroup.audioMixer.SetFloat(UnityConstants.Audio.Params.SfxVolumeFloatKey, on ? 0f : -80f);
            
            _userSaveDataManager.SaveData.SoundSettings.SfxOn = on;
            _userSaveDataManager.Save();
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
                audioSource.outputAudioMixerGroup = _sfxGroup;
                _audioSources.Add(sfxKey, audioSource);
            }
            
            _audioSources[sfxKey].Play();
        }
    }
}