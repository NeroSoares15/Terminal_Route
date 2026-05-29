using NUnit.Framework;
using TerminalRoute.Core;

namespace TerminalRoute.Tests
{
    public sealed class GameStateTests
    {
        [Test]
        public void StartRunInitializesMvpState()
        {
            var state = GameState.CreateNewRun();

            Assert.AreEqual(GamePhase.Driving, state.Phase);
            Assert.AreEqual(1, state.Loop);
            Assert.AreEqual(22, state.PassengerCount);
            Assert.AreEqual(100f, state.Sanity);
            Assert.AreEqual(EndingId.None, state.Ending);
            Assert.AreEqual(EndingCause.None, state.EndingCause);
            Assert.AreEqual(0, state.StopsReached);
            Assert.AreEqual(0, state.MissedStops);
        }

        [Test]
        public void SanityLossClampsAndTriggersLongRouteEnding()
        {
            var state = GameState.CreateNewRun();

            state.ApplySanityDelta(-140f);

            Assert.AreEqual(0f, state.Sanity);
            Assert.AreEqual(GamePhase.Ended, state.Phase);
            Assert.AreEqual(EndingId.LongRoute, state.Ending);
            Assert.AreEqual(EndingCause.SanityZero, state.EndingCause);
        }

        [Test]
        public void CompletingEightStopsTriggersGoodTripEnding()
        {
            var state = GameState.CreateNewRun();

            for (int i = 0; i < GameState.FinalStopCount; i++)
            {
                state.CompleteStop();
            }

            Assert.AreEqual(GamePhase.Ended, state.Phase);
            Assert.AreEqual(EndingId.GoodTrip, state.Ending);
            Assert.AreEqual(EndingCause.CompletedRoute, state.EndingCause);
            Assert.AreEqual(8, state.StopsReached);
        }

        [Test]
        public void TwoMissedStopsTriggersLongRouteWithMissedStopsCause()
        {
            var state = GameState.CreateNewRun();

            state.MissStop();
            Assert.AreEqual(1, state.MissedStops);
            Assert.AreEqual(EndingId.None, state.Ending);

            state.MissStop();

            Assert.AreEqual(GamePhase.Ended, state.Phase);
            Assert.AreEqual(EndingId.LongRoute, state.Ending);
            Assert.AreEqual(EndingCause.MissedStops, state.EndingCause);
            Assert.AreEqual(2, state.MissedStops);
        }

        [Test]
        public void CompletingStopsThinsPassengerCountUntilTheBusIsEmpty()
        {
            var state = GameState.CreateNewRun();

            for (int i = 0; i < 7; i++)
            {
                state.CompleteStop();
            }

            Assert.AreEqual(1, state.PassengerCount);

            state.CompleteStop();
            Assert.AreEqual(0, state.PassengerCount);
        }
    }
}
