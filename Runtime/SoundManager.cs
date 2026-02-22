using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    [DefaultExecutionOrder(-100)]
    public class SoundManager : SingletonMonoBehaviour<SoundManager>
    {
        public int AudioSourcePoolSize = 10;
        
        private List<AudioSource> _audioSourcesPool;
        private readonly Dictionary<string, SoundEffect> _soundDictionary = new();

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_audioSourcesPool == null)
            {
                _audioSourcesPool = new(AudioSourcePoolSize);
                for (int i = 0; i < AudioSourcePoolSize; i++)
                {
                    AddNewAudioSourceToPool();
                }
            }
        }

        private AudioSource AddNewAudioSourceToPool()
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            _audioSourcesPool.Add(audioSource);
            return audioSource;
        }

        public AudioSource PlaySfx(string soundNameAndSettings)
        {
            string[] parts = soundNameAndSettings.Split("|");
            string soundName = parts[0];
            float? volumeScale = null;

            if (parts.Length > 1 && float.TryParse(parts[1], out float parsedVolumeScale))
                volumeScale = parsedVolumeScale;

            return PlaySfx(soundName, volumeScale);
        }

        public AudioSource PlaySfx(string soundName, float? volumeScale, Action onComplete = null)
        {
            AudioSource audioSource = null;
            SoundEffect sound = null;

            if (!_soundDictionary?.TryGetValue(soundName, out sound) ?? false)
            {
                sound = Resources.Load<SoundEffect>($"SoundEffects/{soundName}");
                if (sound == null)
                {
                    Debug.LogWarning("SoundEffect '" + soundName + "' not found!");
                    return null;
                }
                _soundDictionary.Add(soundName, sound);
            }

            audioSource = PlaySfx(sound, volumeScale, onComplete);
            return audioSource;
        }

        public AudioSource PlaySfx(SoundEffect soundEffect, float? volumeScale = null, Action onComplete = null)
        {
            if (soundEffect == null)
            {
                Debug.LogWarning("Provided SoundEffect is null!");
                return null;
            }
            AudioSource audioSource = GetNextAudioSource();
            audioSource.pitch = UnityEngine.Random.Range(soundEffect.PitchMin, soundEffect.PitchMax);
            audioSource.PlayOneShot(soundEffect.Clip, (volumeScale ?? soundEffect.VolumeScale));
            if (onComplete != null)
            {
                StartCoroutine(WaitForSound(soundEffect.Clip.length, onComplete));
            }
            return audioSource;
        }

        private int _currentAudioSource = -1;
        private AudioSource GetNextAudioSource()
        {
            int remainingSourcesToTry = _audioSourcesPool.Count;
            while (remainingSourcesToTry > 0)
            {
                _currentAudioSource = (_currentAudioSource + 1) % _audioSourcesPool.Count;
                if (!_audioSourcesPool[_currentAudioSource].isPlaying)
                {
                    return _audioSourcesPool[_currentAudioSource];
                }
                else
                {
                    remainingSourcesToTry--;
                }
            }
            return AddNewAudioSourceToPool();
        }

        private static IEnumerator WaitForSound(float clipLength, Action onComplete)
        {
            yield return new WaitForSeconds(clipLength);
            onComplete();
        }
    }
}