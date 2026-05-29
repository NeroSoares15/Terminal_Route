using TerminalRoute.Core;

namespace TerminalRoute.Runtime
{
    public sealed class RouteManager
    {
        private const float FirstStopZ = 75f;
        private const float StopInterval = 82f;
        private const float StopDuration = 2.6f;
        private const float StopCaptureMinimumX = 1.25f;
        private const float StopCaptureLead = 8f;
        private const float StopCaptureTail = 8f;
        private const float StopParkX = 2.55f;

        private float nextStopZ;
        private float stopTimer;
        private float alertTimer;
        private int nextStopIndex;

        public float TimeToNextStop { get; private set; }
        public string AlertText { get; private set; }
        public bool IsPausedAtStop { get { return stopTimer > 0f; } }

        public void Reset()
        {
            nextStopZ = FirstStopZ;
            stopTimer = 0f;
            TimeToNextStop = 55f;
            alertTimer = 0f;
            AlertText = "";
            nextStopIndex = 0;
        }

        public void Tick(float deltaTime, GameState state, BusController bus, EpisodeManager episodes, TerminalRouteAudio audio, SceneFactory scene)
        {
            if (state.Phase == GamePhase.Ended)
            {
                return;
            }

            TickAlert(deltaTime);

            if (state.Phase == GamePhase.Stopped)
            {
                stopTimer -= deltaTime;
                TimeToNextStop = 0f;

                if (stopTimer <= 0f)
                {
                    CompleteStop(state, bus, episodes);
                }

                return;
            }

            TimeToNextStop = UnityEngine.Mathf.Max(0f, (nextStopZ - bus.WorldZ) / 12f);

            if (HasReachedStopBoardingPoint(bus.WorldZ, nextStopZ) && IsInStopCaptureZone(bus.WorldZ, nextStopZ) && IsRightSideStop(bus.LateralPosition))
            {
                BeginStop(state, bus, audio, scene);
                return;
            }

            if (IsInStopCaptureZone(bus.WorldZ, nextStopZ) && !IsRightSideStop(bus.LateralPosition))
            {
                AlertText = "APROXIMA-TE DA DIREITA";
                alertTimer = 0.2f;
                return;
            }

            if (HasPassedStopMissPoint(bus.WorldZ, nextStopZ))
            {
                MissStop(state, episodes);
            }
        }

        public static bool IsInStopCaptureZone(float busZ, float stopZ)
        {
            return busZ >= stopZ - StopCaptureLead && busZ <= stopZ + StopCaptureTail;
        }

        public static bool HasPassedStopMissPoint(float busZ, float stopZ)
        {
            return busZ > stopZ + StopCaptureTail;
        }

        public static bool HasReachedStopBoardingPoint(float busZ, float stopZ)
        {
            return busZ >= stopZ;
        }

        public static bool IsRightSideStop(float lateralPosition)
        {
            return lateralPosition >= StopCaptureMinimumX;
        }

        private void BeginStop(GameState state, BusController bus, TerminalRouteAudio audio, SceneFactory scene)
        {
            state.SetPhase(GamePhase.Stopped);
            state.ApplySanityDelta(12f);
            bus.SnapToStop(StopParkX, nextStopZ);
            bus.SetPaused(true);
            stopTimer = StopDuration;
            scene.BeginStopBoarding(nextStopIndex, StopDuration);
            audio.PlayStopChime();
        }

        private void CompleteStop(GameState state, BusController bus, EpisodeManager episodes)
        {
            state.CompleteStop();
            if (state.Phase == GamePhase.Ended)
            {
                return;
            }

            nextStopZ += StopInterval;
            nextStopIndex++;
            state.SetPhase(GamePhase.Driving);
            bus.SetPaused(false);
            episodes.BeginLoop(state.Loop);
        }

        private void MissStop(GameState state, EpisodeManager episodes)
        {
            state.MissStop();
            AlertText = "PARAGEM PERDIDA  //  APROXIMA-TE DA DIREITA";
            alertTimer = 2.8f;

            if (state.Phase == GamePhase.Ended)
            {
                return;
            }

            nextStopZ += StopInterval;
            nextStopIndex++;
            episodes.BeginLoop(state.Loop);
        }

        private void TickAlert(float deltaTime)
        {
            if (alertTimer <= 0f)
            {
                AlertText = "";
                return;
            }

            alertTimer -= deltaTime;
            if (alertTimer <= 0f)
            {
                AlertText = "";
            }
        }
    }
}
