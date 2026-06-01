using TerminalRoute.Core;
using UnityEngine;

namespace TerminalRoute.Runtime
{
    public sealed class EpisodeManager
    {
        private readonly SceneFactory scene;
        private EpisodeType activeEpisode;
        private float episodeTimer;
        private bool mirrorCostApplied;
        private int monkeyRow;
        private bool monkeyMovedAfterView;

        public EpisodeManager(SceneFactory scene)
        {
            this.scene = scene;
        }

        public EpisodeType ActiveEpisode
        {
            get { return activeEpisode; }
        }

        public bool InputsInverted
        {
            get { return activeEpisode == EpisodeType.InvertedControls; }
        }

        public float LightsOutAmount
        {
            get
            {
                if (activeEpisode != EpisodeType.LightsOut)
                {
                    return 0f;
                }

                float fadeIn = Mathf.Clamp01(episodeTimer / 0.35f);
                float fadeOut = Mathf.Clamp01((7.2f - episodeTimer) / 0.65f);
                float flicker = 0.74f + Mathf.PerlinNoise(Time.time * 18f, 0.43f) * 0.26f;
                return Mathf.Clamp01(Mathf.Min(fadeIn, fadeOut) * flicker);
            }
        }

        public void Reset()
        {
            activeEpisode = EpisodeType.None;
            episodeTimer = 0f;
            mirrorCostApplied = false;
            monkeyRow = 10;
            monkeyMovedAfterView = false;
            scene.Monkey.SetActive(false);
            scene.Ball.SetActive(false);
            foreach (var passenger in scene.Passengers)
            {
                passenger.SetStaring(false);
            }
        }

        public void BeginLoop(int loop)
        {
            activeEpisode = EpisodeScheduler.GetEpisodeForLoop(loop);
            episodeTimer = 0f;
            mirrorCostApplied = false;
            monkeyMovedAfterView = false;

            foreach (var passenger in scene.Passengers)
            {
                passenger.SetStaring(false);
            }

            scene.Ball.SetActive(false);

            if (activeEpisode == EpisodeType.Monkey)
            {
                monkeyRow = 10;
                scene.SetMonkeyRow(monkeyRow);
                scene.Monkey.SetActive(true);
            }
            else if (activeEpisode == EpisodeType.Ball)
            {
                scene.Ball.SetActive(true);
                scene.SetBallProgress(0f);
            }
            else
            {
                scene.Monkey.SetActive(false);
            }
        }

        public void Tick(float deltaTime, GameState state, MirrorView mirror, TerminalRouteAudio audio)
        {
            if (activeEpisode == EpisodeType.None || state.Phase != GamePhase.Driving)
            {
                return;
            }

            episodeTimer += deltaTime;

            if (activeEpisode == EpisodeType.Silence)
            {
                TickSilence(state, mirror, audio);
            }
            else if (activeEpisode == EpisodeType.Monkey)
            {
                TickMonkey(state, mirror, audio);
            }
            else if (activeEpisode == EpisodeType.Ball)
            {
                TickBall(state, mirror, audio);
            }
            else if (activeEpisode == EpisodeType.InvertedControls)
            {
                TickInvertedControls(state, mirror, audio);
            }
            else if (activeEpisode == EpisodeType.LightsOut)
            {
                TickLightsOut(state, audio);
            }
        }

        private void TickSilence(GameState state, MirrorView mirror, TerminalRouteAudio audio)
        {
            if (episodeTimer > 7f)
            {
                activeEpisode = EpisodeType.None;
                foreach (var passenger in scene.Passengers)
                {
                    passenger.SetStaring(false);
                }

                return;
            }

            if (mirror.IsActive)
            {
                foreach (var passenger in scene.Passengers)
                {
                    passenger.SetStaring(true);
                }

                ApplyMirrorCostOnce(state, audio);
            }
        }

        private void TickMonkey(GameState state, MirrorView mirror, TerminalRouteAudio audio)
        {
            if (mirror.IsActive)
            {
                ApplyMirrorCostOnce(state, audio);
                monkeyMovedAfterView = false;
                return;
            }

            if (!monkeyMovedAfterView && mirrorCostApplied && monkeyRow > 2)
            {
                monkeyRow -= 1;
                scene.SetMonkeyRow(monkeyRow);
                monkeyMovedAfterView = true;
                mirrorCostApplied = false;
            }
        }

        private void TickBall(GameState state, MirrorView mirror, TerminalRouteAudio audio)
        {
            if (mirror.IsActive)
            {
                ApplyMirrorCostOnce(state, audio);
                scene.Ball.SetActive(false);
                activeEpisode = EpisodeType.None;
                return;
            }

            scene.SetBallProgress(Mathf.Clamp01(episodeTimer / 9f));
        }

        private void TickInvertedControls(GameState state, MirrorView mirror, TerminalRouteAudio audio)
        {
            if (!mirrorCostApplied)
            {
                mirrorCostApplied = true;
                state.ApplySanityDelta(EpisodeScheduler.GetMirrorSanityCost(activeEpisode));
                audio.PlayEpisodePulse(activeEpisode);
            }

            if (episodeTimer > 10f)
            {
                activeEpisode = EpisodeType.None;
            }
        }

        private void TickLightsOut(GameState state, TerminalRouteAudio audio)
        {
            if (!mirrorCostApplied)
            {
                mirrorCostApplied = true;
                state.ApplySanityDelta(EpisodeScheduler.GetMirrorSanityCost(activeEpisode));
                audio.PlayLightsOut();
            }

            if (episodeTimer > 7.2f)
            {
                activeEpisode = EpisodeType.None;
            }
        }

        private void ApplyMirrorCostOnce(GameState state, TerminalRouteAudio audio)
        {
            if (mirrorCostApplied)
            {
                return;
            }

            mirrorCostApplied = true;
            state.ApplySanityDelta(EpisodeScheduler.GetMirrorSanityCost(activeEpisode));
            audio.PlayEpisodePulse(activeEpisode);
        }
    }
}
