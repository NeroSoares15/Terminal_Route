using System;

namespace TerminalRoute.Core
{
    [Serializable]
    public sealed class GameState
    {
        public const int InitialPassengerCount = 22;
        public const int FinalStopCount = 8;
        public const int NightmareStopCount = 20;
        public const int MaxMissedStops = 2;
        public const float DefaultStopInterval = 124f;
        public const float NightmareStopInterval = 166f;

        public GameMode Mode { get; private set; }
        public int Loop { get; private set; }
        public int PassengerCount { get; private set; }
        public float Sanity { get; private set; }
        public GamePhase Phase { get; private set; }
        public EndingId Ending { get; private set; }
        public EndingCause EndingCause { get; private set; }
        public int StopsReached { get; private set; }
        public int MissedStops { get; private set; }
        public bool TutorialActive { get; private set; }
        public int TargetStopCount { get; private set; }
        public float StopInterval { get; private set; }

        public bool IsNightmare
        {
            get { return Mode == GameMode.Nightmare; }
        }

        private GameState()
        {
        }

        public static GameState CreateNewRun()
        {
            return CreateNewRun(GameMode.Route04);
        }

        public static GameState CreateNewRun(GameMode mode)
        {
            return new GameState
            {
                Mode = mode,
                Loop = 1,
                PassengerCount = InitialPassengerCount,
                Sanity = 100f,
                Phase = GamePhase.Driving,
                Ending = EndingId.None,
                EndingCause = EndingCause.None,
                StopsReached = 0,
                MissedStops = 0,
                TutorialActive = true,
                TargetStopCount = mode == GameMode.Nightmare ? NightmareStopCount : FinalStopCount,
                StopInterval = mode == GameMode.Nightmare ? NightmareStopInterval : DefaultStopInterval
            };
        }

        public void SetPhase(GamePhase phase)
        {
            if (Phase == GamePhase.Ended)
            {
                return;
            }

            Phase = phase;
        }

        public void ApplySanityDelta(float delta)
        {
            if (Phase == GamePhase.Ended)
            {
                return;
            }

            Sanity = Math.Max(0f, Math.Min(100f, Sanity + delta));

            if (Sanity <= 0f)
            {
                ResolveEnding(EndingId.LongRoute, EndingCause.SanityZero);
            }
        }

        public void CompleteLoop()
        {
            CompleteStop();
        }

        public void CompleteTutorial()
        {
            if (Phase == GamePhase.Ended)
            {
                return;
            }

            TutorialActive = false;
        }

        public void CompleteStop()
        {
            if (Phase == GamePhase.Ended)
            {
                return;
            }

            StopsReached = Math.Min(TargetStopCount, StopsReached + 1);
            Loop++;
            int passengerDropPerStop = IsNightmare ? 1 : 3;
            PassengerCount = Math.Max(0, InitialPassengerCount - (StopsReached * passengerDropPerStop));

            if (StopsReached >= TargetStopCount)
            {
                ResolveEnding(EndingId.GoodTrip, EndingCause.CompletedRoute);
            }
        }

        public void LosePassenger()
        {
            if (Phase == GamePhase.Ended)
            {
                return;
            }

            PassengerCount = Math.Max(0, PassengerCount - 1);
            ApplySanityDelta(-18f);
        }

        public void MissStop()
        {
            if (Phase == GamePhase.Ended)
            {
                return;
            }

            MissedStops = Math.Min(MaxMissedStops, MissedStops + 1);
            Loop++;
            if (MissedStops >= MaxMissedStops)
            {
                ResolveEnding(EndingId.LongRoute, EndingCause.MissedStops);
            }
        }

        public void ResolveEnding(EndingId ending)
        {
            ResolveEnding(ending, ending == EndingId.GoodTrip ? EndingCause.CompletedRoute : EndingCause.ManualExit);
        }

        public void ResolveEnding(EndingId ending, EndingCause cause)
        {
            Ending = ending;
            EndingCause = cause;
            Phase = GamePhase.Ended;
        }
    }
}
