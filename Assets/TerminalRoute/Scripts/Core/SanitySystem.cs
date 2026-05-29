namespace TerminalRoute.Core
{
    public static class SanitySystem
    {
        public static SanityProfile Evaluate(float sanity)
        {
            if (sanity >= 75f)
            {
                return new SanityProfile(1f, 0f, 0f);
            }

            if (sanity >= 50f)
            {
                return new SanityProfile(0.82f, 0.18f, 0.2f);
            }

            if (sanity >= 25f)
            {
                return new SanityProfile(0.56f, 0.45f, 0.55f);
            }

            return new SanityProfile(0.28f, 0.85f, 0.9f);
        }
    }
}
