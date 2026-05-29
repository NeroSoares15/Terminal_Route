using TerminalRoute.Core;

namespace TerminalRoute.Runtime
{
    public static class TerminalRouteSession
    {
        public static EndingId Ending { get; private set; }
        public static EndingCause EndingCause { get; private set; }
        public static int StopsReached { get; private set; }
        public static int MissedStops { get; private set; }
        public static float FinalSanity { get; private set; }
        public static float ElapsedRouteTime { get; private set; }

        public static void StartNewRun()
        {
            Ending = EndingId.None;
            EndingCause = EndingCause.None;
            StopsReached = 0;
            MissedStops = 0;
            FinalSanity = 100f;
            ElapsedRouteTime = 0f;
        }

        public static void FinishRun(EndingId ending)
        {
            Ending = ending;
            EndingCause = ending == EndingId.GoodTrip ? EndingCause.CompletedRoute : EndingCause.ManualExit;
        }

        public static void FinishRun(GameState state, float elapsedRouteTime)
        {
            Ending = state.Ending;
            EndingCause = state.EndingCause;
            StopsReached = state.StopsReached;
            MissedStops = state.MissedStops;
            FinalSanity = state.Sanity;
            ElapsedRouteTime = elapsedRouteTime;
        }
    }
}
