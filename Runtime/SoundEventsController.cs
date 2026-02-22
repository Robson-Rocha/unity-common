using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    public class SoundEventsController : MonoBehaviour
    {
        public void PlaySfx(string soundNameAndSettings)
        {
            SoundManager.Instance.PlaySfx(soundNameAndSettings);
        }
    }
}