namespace TerminalRoute.Core
{
    public readonly struct SanityProfile
    {
        public SanityProfile(float controlMultiplier, float driftStrength, float visualDistortion)
        {
            ControlMultiplier = controlMultiplier;
            DriftStrength = driftStrength;
            VisualDistortion = visualDistortion;
        }

        public float ControlMultiplier { get; }
        public float DriftStrength { get; }
        public float VisualDistortion { get; }
    }
}
