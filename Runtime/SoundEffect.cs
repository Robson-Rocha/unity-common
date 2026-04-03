using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    [CreateAssetMenu(fileName = "SoundEffect", menuName = "Audio/Sound Effect", order = 1)]
    public class SoundEffect : ScriptableObject
    {
        public AudioClip Clip;
        public float VolumeScale = 1f;
        public float PitchMin = 1f;
        public float PitchMax = 1f;

        public void Play()
        {
            if (SoundManager.Instance == null || Clip == null) return;
            SoundManager.Instance.PlaySfx(this);
        }
    }
}