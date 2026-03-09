namespace RobsonRocha.UnityCommon
{
    public enum MusicTrackLoopBehavior
    {
        Infinite,   // Loop forever
        NoLoop,    // Play once, don't loop
        Finite    // Loop LoopCount times (0 = play once, 1 = twice, etc.)
    }
}