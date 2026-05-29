using NUnit.Framework;
using TerminalRoute.Core;

namespace TerminalRoute.Tests
{
    public sealed class SanitySystemTests
    {
        [TestCase(100f, 1f, 0f, 0f)]
        [TestCase(70f, 0.82f, 0.18f, 0.2f)]
        [TestCase(45f, 0.56f, 0.45f, 0.55f)]
        [TestCase(15f, 0.28f, 0.85f, 0.9f)]
        public void ThresholdsDescribeDrivingAndVisualDegradation(float sanity, float expectedControl, float expectedDrift, float expectedDistortion)
        {
            var profile = SanitySystem.Evaluate(sanity);

            Assert.AreEqual(expectedControl, profile.ControlMultiplier, 0.01f);
            Assert.AreEqual(expectedDrift, profile.DriftStrength, 0.01f);
            Assert.AreEqual(expectedDistortion, profile.VisualDistortion, 0.01f);
        }
    }
}
