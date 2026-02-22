using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    public class MusicEventsController : MonoBehaviour
    {
        [SerializeField] private MusicTrack PlayOnStart;

        void Start()
        {
            if (PlayOnStart != null)
            {
                MusicManager.Instance.PlayMusic(PlayOnStart, MusicTrackTransition.Crossfade);
            }
        }

        public void PlayMusic(string musicNameAndSettings)
        {
            MusicManager.Instance.PlayMusic(musicNameAndSettings);
        }
    }
}
