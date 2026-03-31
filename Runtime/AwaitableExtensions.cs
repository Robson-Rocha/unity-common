using UnityEngine;

namespace RobsonRocha.UnityCommon
{
    public static class AwaitableExtensions
    {
        public static void AndForget(this Awaitable awaitable)
        {
            _ = awaitable;
        }
    }
    
}
