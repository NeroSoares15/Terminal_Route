using NUnit.Framework;
using TerminalRoute.Core;
using TerminalRoute.Runtime;

namespace TerminalRoute.Tests
{
    public sealed class HazardRuleTests
    {
        [Test]
        public void CloseMirrorThreatTimesOutAfterReactionWindow()
        {
            Assert.IsFalse(MirrorThreatManager.ReactionExpired(1.14f));
            Assert.IsTrue(MirrorThreatManager.ReactionExpired(1.15f));
        }

        [Test]
        public void OncomingBusCollisionUsesLaneAndForwardWindow()
        {
            Assert.IsTrue(OncomingBusManager.IsCollision(-1.8f, 100f, -1.9f, 102.5f));
            Assert.IsFalse(OncomingBusManager.IsCollision(1.8f, 100f, -1.9f, 102.5f));
            Assert.IsFalse(OncomingBusManager.IsCollision(-1.8f, 100f, -1.9f, 105f));
        }

        [Test]
        public void StopsHaveAReadableCaptureZone()
        {
            Assert.AreEqual(124f, RouteManager.StopInterval);
            Assert.IsTrue(RouteManager.IsInStopCaptureZone(68f, 75f));
            Assert.IsTrue(RouteManager.IsInStopCaptureZone(82f, 75f));
            Assert.IsFalse(RouteManager.IsInStopCaptureZone(66f, 75f));
            Assert.IsFalse(RouteManager.HasReachedStopBoardingPoint(74.9f, 75f));
            Assert.IsTrue(RouteManager.HasReachedStopBoardingPoint(75f, 75f));
            Assert.IsTrue(RouteManager.HasPassedStopMissPoint(84f, 75f));
            Assert.IsTrue(RouteManager.IsRightSideStop(1.25f));
            Assert.IsFalse(RouteManager.IsRightSideStop(0.9f));
        }

        [Test]
        public void StopsEaseSpeedOnlyWhenBusIsNearRightSide()
        {
            Assert.AreEqual(1f, RouteManager.StopApproachSpeedMultiplier(40f, 75f, 2.0f));
            Assert.AreEqual(1f, RouteManager.StopApproachSpeedMultiplier(70f, 75f, 0.5f));
            Assert.Less(RouteManager.StopApproachSpeedMultiplier(70f, 75f, 2.0f), 1f);
            Assert.GreaterOrEqual(RouteManager.StopApproachSpeedMultiplier(75f, 75f, 2.0f), 0.18f);
        }

        [Test]
        public void ExplicitHazardCausesResolveToLongRoute()
        {
            var state = GameState.CreateNewRun();

            state.ResolveEnding(EndingId.LongRoute, EndingCause.CloseNpcStare);

            Assert.AreEqual(GamePhase.Ended, state.Phase);
            Assert.AreEqual(EndingId.LongRoute, state.Ending);
            Assert.AreEqual(EndingCause.CloseNpcStare, state.EndingCause);
        }
    }
}
