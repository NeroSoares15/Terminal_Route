using TerminalRoute.Core;

namespace TerminalRoute.Runtime
{
    public sealed class RouteManager
    {
        public const float FirstStopZ = 75f;
        public const float StopInterval = GameState.DefaultStopInterval;
        private const float StopDuration = 2.6f;
        private const float StopCaptureMinimumX = 1.25f;
        private const float TutorialStopCaptureMinimumX = 0.55f;
        private const float StopCaptureLead = 8f;
        private const float StopCaptureTail = 8f;
        private const float StopParkX = 2.55f;
        private const float StopSlowdownLead = 24f;
        private const float StopApproachMinimumSpeed = 0.18f;

        private float nextStopZ;
        private float stopTimer;
        private float stopInterval = StopInterval;
        private float alertTimer;
        private int nextStopIndex;
        private bool approachCuePlayed;

        public float TimeToNextStop { get; private set; }
        public string AlertText { get; private set; }
        public bool IsPausedAtStop { get { return stopTimer > 0f; } }

        public void Reset()
        {
            Reset(GameState.CreateNewRun());
        }

        public void Reset(GameState state)
        {
            nextStopZ = FirstStopZ;
            stopInterval = state.StopInterval;
            stopTimer = 0f;
            TimeToNextStop = 55f;
            alertTimer = 0f;
            AlertText = "";
            nextStopIndex = 0;
            approachCuePlayed = false;
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

            bool tutorialStop = state.TutorialActive && nextStopIndex == 0;
            bus.SetTargetSpeedMultiplier(StopApproachSpeedMultiplier(bus.WorldZ, nextStopZ, tutorialStop ? MathfForTutorialStop(bus.LateralPosition) : bus.LateralPosition));
            float routeSpeed = UnityEngine.Mathf.Max(1f, bus.CurrentRouteSpeed);
            TimeToNextStop = UnityEngine.Mathf.Max(0f, (nextStopZ - bus.WorldZ) / routeSpeed);
            TickStopGuidance(bus, audio, state);

            if (HasReachedStopBoardingPoint(bus.WorldZ, nextStopZ) && IsInStopCaptureZone(bus.WorldZ, nextStopZ) && IsRightSideStop(bus.LateralPosition, tutorialStop))
            {
                BeginStop(state, bus, audio, scene);
                return;
            }

            if (IsInStopCaptureZone(bus.WorldZ, nextStopZ) && !IsRightSideStop(bus.LateralPosition, tutorialStop))
            {
                AlertText = tutorialStop ? "TUTORIAL  //  ENCOSTA A DIREITA" : "APROXIMA-TE DA DIREITA";
                alertTimer = 0.2f;
                return;
            }

            if (HasPassedStopMissPoint(bus.WorldZ, nextStopZ))
            {
                if (tutorialStop)
                {
                    AlertText = "TUTORIAL  //  PARAGEM ASSISTIDA";
                    alertTimer = 2.2f;
                    BeginStop(state, bus, audio, scene);
                    return;
                }

                MissStop(state, bus, episodes, audio);
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

        private static bool IsRightSideStop(float lateralPosition, bool tutorialStop)
        {
            return lateralPosition >= (tutorialStop ? TutorialStopCaptureMinimumX : StopCaptureMinimumX);
        }

        public static float StopApproachSpeedMultiplier(float busZ, float stopZ, float lateralPosition)
        {
            if (!IsRightSideStop(lateralPosition))
            {
                return 1f;
            }

            float distance = stopZ - busZ;
            if (distance >= StopSlowdownLead)
            {
                return 1f;
            }

            if (distance <= 0f)
            {
                return StopApproachMinimumSpeed;
            }

            float approach = 1f - UnityEngine.Mathf.Clamp01(distance / StopSlowdownLead);
            float easedApproach = approach * approach * (3f - 2f * approach);
            return UnityEngine.Mathf.Lerp(1f, StopApproachMinimumSpeed, easedApproach);
        }

        private static float MathfForTutorialStop(float lateralPosition)
        {
            return lateralPosition >= TutorialStopCaptureMinimumX ? StopCaptureMinimumX : lateralPosition;
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
            if (state.TutorialActive)
            {
                state.CompleteTutorial();
                nextStopZ += stopInterval;
                nextStopIndex++;
                approachCuePlayed = false;
                state.SetPhase(GamePhase.Driving);
                bus.SetPaused(false);
                bus.SetTargetSpeedMultiplier(1f);
                episodes.BeginLoop(state.Loop, state.Mode);
                AlertText = state.IsNightmare ? "TUTORIAL COMPLETO  //  PESADELO" : "TUTORIAL COMPLETO  //  ROTA 04";
                alertTimer = 3.2f;
                return;
            }

            state.CompleteStop();
            if (state.Phase == GamePhase.Ended)
            {
                return;
            }

            nextStopZ += stopInterval;
            nextStopIndex++;
            approachCuePlayed = false;
            state.SetPhase(GamePhase.Driving);
            bus.SetPaused(false);
            bus.SetTargetSpeedMultiplier(1f);
            episodes.BeginLoop(state.Loop, state.Mode);
        }

        private void MissStop(GameState state, BusController bus, EpisodeManager episodes, TerminalRouteAudio audio)
        {
            state.MissStop();
            AlertText = "PARAGEM PERDIDA  //  APROXIMA-TE DA DIREITA";
            alertTimer = 2.8f;
            audio.PlayMissedStop();

            if (state.Phase == GamePhase.Ended)
            {
                return;
            }

            nextStopZ += stopInterval;
            nextStopIndex++;
            approachCuePlayed = false;
            bus.SetTargetSpeedMultiplier(1f);
            episodes.BeginLoop(state.Loop, state.Mode);
        }

        private void TickStopGuidance(BusController bus, TerminalRouteAudio audio, GameState state)
        {
            float distance = nextStopZ - bus.WorldZ;
            if (distance > 0f && distance < 36f)
            {
                if (!approachCuePlayed)
                {
                    audio.PlayStopApproach();
                    approachCuePlayed = true;
                }

                bool tutorialStop = state.TutorialActive && nextStopIndex == 0;
                AlertText = IsRightSideStop(bus.LateralPosition, tutorialStop) ? (tutorialStop ? "TUTORIAL  //  ENTRA NA ZONA VERDE" : "APROXIMA-TE DA PARAGEM") : "ENCOSTA A DIREITA PARA PARAR";
                alertTimer = 0.24f;
            }
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
