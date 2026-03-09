using UnityEngine;

namespace RobsonRocha.UnityCommon
{
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