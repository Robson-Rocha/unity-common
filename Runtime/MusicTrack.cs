using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    public enum MusicTrackLoopBehavior
    {
        Infinite,   // Loop forever
        NoLoop,    // Play once, don't loop
        Finite    // Loop LoopCount times (0 = play once, 1 = twice, etc.)
    }

    public enum MusicTrackTransition
    {
        Crossfade,             // Both fade simultaneously (smooth transition)
        FadeOutThenStart,      // Fadeout current, wait, then start new
        StopAndFadeIn,         // Stop current immediately, fade in new
        StopAndStart           // Stop current, start new immediately (no fades)
    }

    [CreateAssetMenu(fileName = "MusicTrack", menuName = "Audio/Music Track", order = 1)]
    public class MusicTrack : ScriptableObject
    {
        public string Name;
        public AudioClip Clip;
        public float VolumeScale = 1f;
        public MusicTrackLoopBehavior LoopBehavior = MusicTrackLoopBehavior.Infinite;
        public int LoopCount = 0;
        public bool FadeIn = false;
        public float FadeInDuration = 1f;
        public float FadeOutDuration = 1f;
    }
}