namespace RobsonRocha.UnityCommon
{
    public enum MusicTrackTransition
    {
        Crossfade,             // Both fade simultaneously (smooth transition)
        FadeOutThenStart,      // Fadeout current, wait, then start new
        StopAndFadeIn,         // Stop current immediately, fade in new
        StopAndStart           // Stop current, start new immediately (no fades)
    }
}