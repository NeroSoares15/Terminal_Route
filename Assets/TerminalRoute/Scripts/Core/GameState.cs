using System;

namespace TerminalRoute.Core
{
    [Serializable]
    public sealed class GameState
    {
        public const int InitialPassengerCount = 22;
        public const int FinalStopCount = 8;
        public const int MaxMissedStops = 2;

        public int Loop { get; private set; }
        public int PassengerCount { get; private set; }
        public float Sanity { get; private set; }
        public GamePhase Phase { get; private set; }
        public EndingId Ending { get; private set; }
        public EndingCause EndingCause { get; private set; }
        public int StopsReached { get; private set; }
        public int MissedStops { get; private set; }

        private GameState()
        {
        }

        public static GameState CreateNewRun()
        {
            return new GameState
            {
                Loop = 1,
                PassengerCount = InitialPassengerCount,
                Sanity = 100f,
                Phase = GamePhase.Driving,
                Ending = EndingId.None,
                EndingCause = EndingCause.None,
                StopsReached = 0,
                MissedStops = 0
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

        public void CompleteStop()
        {
            if (Phase == GamePhase.Ended)
            {
                return;
            }

            StopsReached = Math.Min(FinalStopCount, StopsReached + 1);
            Loop++;
            PassengerCount = Math.Max(0, InitialPassengerCount - (StopsReached * 3));

            if (StopsReached >= FinalStopCount)
            {
                ResolveEnding(EndingId.GoodTrip, EndingCause.CompletedRoute);
            }
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
