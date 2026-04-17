using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    [DefaultExecutionOrder(-100)]
    public class MusicManager : SingletonMonoBehaviour<MusicManager>
    {
        private readonly Dictionary<string, MusicTrack> _musicDictionary = new();

        // Two audio sources for crossfading capability
        private AudioSource _currentAudioSource;
        private AudioSource _nextAudioSource;

        private MusicTrack _currentMusic;
        private int _currentLoopCount;
        private Coroutine _fadeCoroutine;
        [SerializeField, Range(0f, 1f)]
        private float GlobalMusicVolumeValue = 1f;
        private float _lastAppliedGlobalMusicVolume = 1f;

        public event EventHandler<GlobalMusicVolumeChangedEventArgs> GlobalMusicVolumeChanged;

        public float GlobalMusicVolume
        {
            get => GlobalMusicVolumeValue;
            set
            {
                float clampedVolume = Mathf.Clamp01(value);
                if (Mathf.Approximately(_lastAppliedGlobalMusicVolume, clampedVolume))
                {
                    GlobalMusicVolumeValue = clampedVolume;
                    return;
                }

                float previousVolume = _lastAppliedGlobalMusicVolume;
                GlobalMusicVolumeValue = clampedVolume;
                _lastAppliedGlobalMusicVolume = clampedVolume;
                ApplyGlobalVolumeToActiveSources(previousVolume, _lastAppliedGlobalMusicVolume);
                OnGlobalMusicVolumeChanged(previousVolume, _lastAppliedGlobalMusicVolume);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _lastAppliedGlobalMusicVolume = Mathf.Clamp01(GlobalMusicVolumeValue);
            GlobalMusicVolumeValue = _lastAppliedGlobalMusicVolume;
            if (_currentAudioSource == null)
            {
                _currentAudioSource = gameObject.AddComponent<AudioSource>();
                _nextAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        protected virtual void OnGlobalMusicVolumeChanged(float previousVolume, float currentVolume)
        {
            GlobalMusicVolumeChanged?.Invoke(this, new(previousVolume, currentVolume));
        }

        public void PlayMusic(string musicTrackNameAndSettings)
        {
            string[] parts = musicTrackNameAndSettings.Split('|');
            string musicName = parts[0];
            MusicTrackTransition transition = MusicTrackTransition.FadeOutThenStart;

            if (parts.Length > 1)
                Enum.TryParse(parts[1], true, out transition);

            PlayMusic(musicName, transition);
        }

        public void PlayMusic(string musicTrackName, MusicTrackTransition transition)
        {
            if (!_musicDictionary.TryGetValue(musicTrackName, out MusicTrack music))
            {
                music = Resources.Load<MusicTrack>($"MusicTracks/{musicTrackName}");
                if (music == null)
                {
                    Debug.LogWarning($"MusicTrack '{musicTrackName}' not found!");
                    return;
                }
                _musicDictionary.Add(musicTrackName, music);
            }

            PlayMusic(music, transition);
        }
        public void PlayMusic(MusicTrack music, MusicTrackTransition transition)
        {
            if (music == null)
            {
                Debug.LogWarning("Provided music track is null!");
                return;
            }

            if (_currentMusic == music && _currentAudioSource != null && _currentAudioSource.isPlaying)
            {
                return;
            }

            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }

            _fadeCoroutine = StartCoroutine(TransitionToMusic(music, transition));
        }

        private IEnumerator TransitionToMusic(MusicTrack newMusic, MusicTrackTransition transition)
        {
            switch (transition)
            {
                case MusicTrackTransition.FadeOutThenStart:
                    yield return FadeOutCurrent();
                    yield return StartMusic(newMusic, newMusic.FadeIn);
                    break;

                case MusicTrackTransition.Crossfade:
                    yield return CrossfadeMusic(newMusic);
                    break;

                case MusicTrackTransition.StopAndFadeIn:
                    StopCurrentImmediate();
                    yield return StartMusic(newMusic, true);
                    break;

                case MusicTrackTransition.StopAndStart:
                    StopCurrentImmediate();
                    yield return StartMusic(newMusic, false);
                    break;
            }

            _fadeCoroutine = null;
        }

        private IEnumerator StartMusic(MusicTrack music, bool fadeIn)
        {
            SwapAudioSources();

            _currentMusic = music;
            _currentLoopCount = 0;

            _currentAudioSource.clip = music.Clip;
            _currentAudioSource.loop = music.LoopBehavior == MusicTrackLoopBehavior.Infinite;
            _currentAudioSource.volume = fadeIn ? 0 : GetEffectiveMusicVolume(music);
            _currentAudioSource.Play();

            if (fadeIn)
            {
                yield return FadeIn(_currentAudioSource, music, music.FadeInDuration);
            }
        }

        private IEnumerator FadeOutCurrent()
        {
            if (_currentMusic != null && _currentAudioSource.isPlaying)
            {
                // Always use the music's configured fade duration
                if (_currentMusic.FadeOutDuration > 0)
                {
                    yield return FadeOut(_currentAudioSource, _currentMusic.FadeOutDuration);
                }
                _currentAudioSource.Stop();
            }
        }

        private IEnumerator CrossfadeMusic(MusicTrack newMusic)
        {
            AudioSource oldSource = _currentAudioSource;
            float oldVolume = oldSource.volume;
            float fadeOutDuration = _currentMusic != null ? _currentMusic.FadeOutDuration : 1f;
            float fadeInDuration = newMusic.FadeIn ? newMusic.FadeInDuration : 0f;
            float duration = Mathf.Max(fadeOutDuration, fadeInDuration);

            SwapAudioSources();

            _currentMusic = newMusic;
            _currentLoopCount = 0;

            _currentAudioSource.clip = newMusic.Clip;
            _currentAudioSource.loop = newMusic.LoopBehavior == MusicTrackLoopBehavior.Infinite;
            _currentAudioSource.volume = 0;
            _currentAudioSource.Play();

            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                if (fadeInDuration > 0 && elapsed < fadeInDuration)
                {
                    _currentAudioSource.volume = Mathf.Lerp(0, GetEffectiveMusicVolume(newMusic), elapsed / fadeInDuration);
                }
                else if (fadeInDuration == 0)
                {
                    _currentAudioSource.volume = GetEffectiveMusicVolume(newMusic);
                }

                if (fadeOutDuration > 0 && elapsed < fadeOutDuration)
                {
                    oldSource.volume = Mathf.Lerp(oldVolume, 0, elapsed / fadeOutDuration);
                }
                else if (fadeOutDuration == 0)
                {
                    oldSource.volume = 0;
                }

                yield return null;
            }

            oldSource.Stop();
            oldSource.volume = 0;
            _currentAudioSource.volume = GetEffectiveMusicVolume(newMusic);
        }

        private void StopCurrentImmediate()
        {
            if (_currentAudioSource.isPlaying)
            {
                _currentAudioSource.Stop();
            }
            _currentMusic = null;
        }

        private IEnumerator FadeIn(AudioSource source, MusicTrack music, float duration)
        {
            float targetVolume = GetEffectiveMusicVolume(music);
            if (duration <= 0)
            {
                source.volume = targetVolume;
                yield break;
            }

            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                targetVolume = GetEffectiveMusicVolume(music);
                source.volume = Mathf.Lerp(0, targetVolume, elapsed / duration);
                yield return null;
            }
            source.volume = targetVolume;
        }

        private float GetEffectiveMusicVolume(MusicTrack music)
        {
            if (music == null)
            {
                return 0;
            }

            return music.VolumeScale * GlobalMusicVolumeValue;
        }

        private void ApplyGlobalVolumeToActiveSources(float previousGlobalVolume, float newGlobalVolume)
        {
            if (_currentAudioSource == null)
            {
                return;
            }

            if (Mathf.Approximately(previousGlobalVolume, 0f))
            {
                if (_currentMusic != null && _currentAudioSource.isPlaying)
                {
                    _currentAudioSource.volume = GetEffectiveMusicVolume(_currentMusic);
                }
                return;
            }

            float multiplier = newGlobalVolume / previousGlobalVolume;
            _currentAudioSource.volume = Mathf.Clamp01(_currentAudioSource.volume * multiplier);

            if (_nextAudioSource != null)
            {
                _nextAudioSource.volume = Mathf.Clamp01(_nextAudioSource.volume * multiplier);
            }
        }

        private IEnumerator FadeOut(AudioSource source, float duration)
        {
            if (duration <= 0)
            {
                source.volume = 0;
                yield break;
            }

            float startVolume = source.volume;
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0, elapsed / duration);
                yield return null;
            }
            source.volume = 0;
        }

        private void SwapAudioSources()
        {
            AudioSource temp = _currentAudioSource;
            _currentAudioSource = _nextAudioSource;
            _nextAudioSource = temp;
        }

        public void StopMusic(bool fadeOut = true)
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
                _fadeCoroutine = null;
            }

            if (fadeOut)
            {
                _fadeCoroutine = StartCoroutine(FadeOutAndStop());
            }
            else
            {
                StopCurrentImmediate();
            }
        }

        private IEnumerator FadeOutAndStop()
        {
            yield return FadeOutCurrent();
            _currentMusic = null;
            _fadeCoroutine = null;
        }

        void Update()
        {
            // Handle NoLoop and Finite loop behaviors
            if (_currentMusic != null && !_currentAudioSource.isPlaying)
            {
                if (_currentMusic.LoopBehavior == MusicTrackLoopBehavior.NoLoop)
                {
                    // Played once, now stop
                    _currentMusic = null;
                }
                else if (_currentMusic.LoopBehavior == MusicTrackLoopBehavior.Finite)
                {
                    _currentLoopCount++;
                    if (_currentLoopCount <= _currentMusic.LoopCount)
                    {
                        _currentAudioSource.Play();
                    }
                    else
                    {
                        _currentMusic = null;
                    }
                }
                // Infinite loops are handled by AudioSource.loop = true
            }
        }

        public bool IsPlaying()
        {
            return _currentMusic != null && _currentAudioSource.isPlaying;
        }

        public string GetCurrentMusicName()
        {
            return _currentMusic != null ? _currentMusic.Name : null;
        }

        private void OnValidate()
        {
            GlobalMusicVolume = GlobalMusicVolumeValue;
        }
    }

    public sealed class GlobalMusicVolumeChangedEventArgs : EventArgs
    {
        public float PreviousVolume { get; }
        public float CurrentVolume { get; }

        public GlobalMusicVolumeChangedEventArgs(float previousVolume, float currentVolume)
        {
            PreviousVolume = previousVolume;
            CurrentVolume = currentVolume;
        }
    }
}
