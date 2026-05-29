using NUnit.Framework;
using TerminalRoute.Core;

namespace TerminalRoute.Tests
{
    public sealed class EpisodeSchedulerTests
    {
        [Test]
        public void MvpScheduleKeepsFirstLoopSafeThenUsesSilenceMonkeyAndBall()
        {
            Assert.AreEqual(EpisodeType.None, EpisodeScheduler.GetEpisodeForLoop(1));
            Assert.AreEqual(EpisodeType.Silence, EpisodeScheduler.GetEpisodeForLoop(2));
            Assert.AreEqual(EpisodeType.Monkey, EpisodeScheduler.GetEpisodeForLoop(3));
            Assert.AreEqual(EpisodeType.Ball, EpisodeScheduler.GetEpisodeForLoop(4));
            Assert.AreEqual(EpisodeType.InvertedControls, EpisodeScheduler.GetEpisodeForLoop(5));
            Assert.AreEqual(EpisodeType.InvertedControls, EpisodeScheduler.GetEpisodeForLoop(7));
        }

        [Test]
        public void MirrorObservationAppliesEpisodeCosts()
        {
            Assert.AreEqual(-10f, EpisodeScheduler.GetMirrorSanityCost(EpisodeType.Silence));
            Assert.AreEqual(-20f, EpisodeScheduler.GetMirrorSanityCost(EpisodeType.Monkey));
            Assert.AreEqual(-5f, EpisodeScheduler.GetMirrorSanityCost(EpisodeType.Ball));
            Assert.AreEqual(-8f, EpisodeScheduler.GetMirrorSanityCost(EpisodeType.InvertedControls));
            Assert.AreEqual(0f, EpisodeScheduler.GetMirrorSanityCost(EpisodeType.None));
        }
    }
}
