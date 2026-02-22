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

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_currentAudioSource == null)
            {
                _currentAudioSource = gameObject.AddComponent<AudioSource>();
                _nextAudioSource = gameObject.AddComponent<AudioSource>();
            }
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
            _currentAudioSource.volume = fadeIn ? 0 : music.VolumeScale;
            _currentAudioSource.Play();

            if (fadeIn)
            {
                yield return FadeIn(_currentAudioSource, music.VolumeScale, music.FadeInDuration);
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
            float fadeOutDuration = _currentMusic?.FadeOutDuration ?? 1f;
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
                    _currentAudioSource.volume = Mathf.Lerp(0, newMusic.VolumeScale, elapsed / fadeInDuration);
                }
                else if (fadeInDuration == 0)
                {
                    _currentAudioSource.volume = newMusic.VolumeScale;
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
            _currentAudioSource.volume = newMusic.VolumeScale;
        }

        private void StopCurrentImmediate()
        {
            if (_currentAudioSource.isPlaying)
            {
                _currentAudioSource.Stop();
            }
            _currentMusic = null;
        }

        private IEnumerator FadeIn(AudioSource source, float targetVolume, float duration)
        {
            if (duration <= 0)
            {
                source.volume = targetVolume;
                yield break;
            }

            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(0, targetVolume, elapsed / duration);
                yield return null;
            }
            source.volume = targetVolume;
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
            return _currentMusic?.Name;
        }
    }
}
