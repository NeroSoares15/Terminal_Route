using TerminalRoute.Core;
using UnityEngine;

namespace TerminalRoute.Runtime
{
    public static class TerminalRouteSession
    {
        private const string NightmareUnlockKey = "TerminalRoute.NightmareUnlocked";

        public static EndingId Ending { get; private set; }
        public static EndingCause EndingCause { get; private set; }
        public static GameMode Mode { get; private set; } = GameMode.Route04;
        public static int StopsReached { get; private set; }
        public static int TargetStopCount { get; private set; } = GameState.FinalStopCount;
        public static int MissedStops { get; private set; }
        public static float FinalSanity { get; private set; }
        public static float ElapsedRouteTime { get; private set; }

        public static bool IsNightmareUnlocked
        {
            get { return PlayerPrefs.GetInt(NightmareUnlockKey, 0) == 1; }
        }

        public static void UnlockNightmare()
        {
            PlayerPrefs.SetInt(NightmareUnlockKey, 1);
            PlayerPrefs.Save();
        }

        public static void StartNewRun()
        {
            StartNewRun(GameMode.Route04);
        }

        public static void StartNewRun(GameMode mode)
        {
            Mode = mode;
            Ending = EndingId.None;
            EndingCause = EndingCause.None;
            StopsReached = 0;
            TargetStopCount = mode == GameMode.Nightmare ? GameState.NightmareStopCount : GameState.FinalStopCount;
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
            Mode = state.Mode;
            StopsReached = state.StopsReached;
            TargetStopCount = state.TargetStopCount;
            MissedStops = state.MissedStops;
            FinalSanity = state.Sanity;
            ElapsedRouteTime = elapsedRouteTime;

            if (state.Mode == GameMode.Route04 && state.Ending == EndingId.GoodTrip && state.EndingCause == EndingCause.CompletedRoute)
            {
                UnlockNightmare();
            }
        }
    }
}
