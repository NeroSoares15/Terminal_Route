namespace TerminalRoute.Core
{
    public static class EpisodeScheduler
    {
        public static EpisodeType GetEpisodeForLoop(int loop)
        {
            return GetEpisodeForLoop(loop, GameMode.Route04);
        }

        public static EpisodeType GetEpisodeForLoop(int loop, GameMode mode)
        {
            if (mode == GameMode.Nightmare)
            {
                if (loop <= 1)
                {
                    return EpisodeType.None;
                }

                switch ((loop - 2) % 5)
                {
                    case 0:
                        return EpisodeType.Silence;
                    case 1:
                        return EpisodeType.Monkey;
                    case 2:
                        return EpisodeType.Ball;
                    case 3:
                        return EpisodeType.InvertedControls;
                    default:
                        return EpisodeType.LightsOut;
                }
            }

            switch (loop)
            {
                case 2:
                    return EpisodeType.Silence;
                case 3:
                    return EpisodeType.Monkey;
                case 4:
                    return EpisodeType.Ball;
                case 5:
                    return EpisodeType.InvertedControls;
                case 6:
                    return EpisodeType.LightsOut;
                case 7:
                    return EpisodeType.InvertedControls;
                default:
                    return EpisodeType.None;
            }
        }

        public static float GetMirrorSanityCost(EpisodeType episode)
        {
            switch (episode)
            {
                case EpisodeType.Silence:
                    return -10f;
                case EpisodeType.Monkey:
                    return -20f;
                case EpisodeType.Ball:
                    return -5f;
                case EpisodeType.InvertedControls:
                    return -8f;
                case EpisodeType.LightsOut:
                    return -12f;
                default:
                    return 0f;
            }
        }
    }
}
