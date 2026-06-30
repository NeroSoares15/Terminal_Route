using TerminalRoute.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TerminalRoute.Runtime
{
    public sealed class TerminalRouteGame : MonoBehaviour
    {
        private GameState state;
        private SceneFactory scene;
        private BusController bus;
        private RouteManager route;
        private MirrorView mirror;
        private EpisodeManager episodes;
        private TerminalRouteUi ui;
        private VisualDegradation visual;
        private TerminalRouteAudio audioController;
        private MirrorThreatManager mirrorThreat;
        private OncomingBusManager oncomingBuses;
        private RoadHazardManager roadHazards;
        private bool endingShown;
        private bool endingSceneLoading;
        private bool doorsOpen;
        private int visiblePassengerCount;
        private float elapsedRouteTime;
        private float endingSceneTimer;
        private bool introActive;
        private float introTimer;
        private float introVoiceDuration;
        private bool introOpeningEyes;
        private float introEyeOpenTimer;
        private bool passengerCountDue;
        private float passengerCountTimer;
        private float doorOpenDrivingTimer;
        private float passengerFallTimer;
        private bool paused;
        private string routeAlertOverride;
        private float routeAlertOverrideTimer;
        private bool mirrorCheckActive;
        private float mirrorCheckTimer;
        private float mirrorCheckWindow;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            DestroyEditorPreviewObjects();

            scene = new SceneFactory(TerminalRouteSession.Mode);
            scene.Build();

            ui = new TerminalRouteUi();
            ui.BuildRoute(StartRun, ReturnToMenu, ToggleDoors, TogglePause);

            bus = new BusController(scene.Rig, scene.SteeringWheel);
            mirror = new MirrorView(scene.MainCamera.transform, ui);
            episodes = new EpisodeManager(scene);
            route = new RouteManager();
            visual = new VisualDegradation(scene.MainCamera.transform, ui);
            audioController = new TerminalRouteAudio(gameObject);
            mirrorThreat = new MirrorThreatManager(mirror, scene.BuildCloseMirrorThreat(), audioController);
            oncomingBuses = new OncomingBusManager(scene);
            roadHazards = new RoadHazardManager(TerminalRouteSession.Mode);

            StartRun();
        }

        private void Update()
        {
            if (TerminalRouteInput.FullscreenPressed())
            {
                TerminalRouteUi.ToggleFullscreen();
                return;
            }

            if (state.Phase == GamePhase.Ended)
            {
                TickEndingTransition(Time.deltaTime);
                return;
            }

            if (TerminalRouteInput.CancelPressed())
            {
                if (introActive)
                {
                    BeginIntroEyeOpen(true);
                }
                else
                {
                    TogglePause();
                }

                return;
            }

            if (paused)
            {
                return;
            }

            float deltaTime = Time.deltaTime;

            if (introActive)
            {
                TickIntro(deltaTime);
                ui.UpdateHud(state, route.TimeToNextStop, bus.DisplaySpeed, false, episodes.ActiveEpisode, 0f, 0f, PriorityRouteAlert(), "", doorsOpen);
                return;
            }

            if (TerminalRouteInput.DoorPressed())
            {
                ToggleDoors();
            }

            elapsedRouteTime += deltaTime;
            GamePhase phaseBeforeRoute = state.Phase;
            int stopsBeforeRoute = state.StopsReached;
            bool tutorialBeforeRoute = state.TutorialActive;
            TickRouteAlertOverride(deltaTime);
            mirror.Tick(deltaTime);
            scene.SetMirrorMode(mirror.IsActive);
            mirrorThreat.Tick(deltaTime, state);
            TickMirrorCheck(deltaTime);
            bus.Tick(deltaTime, state, episodes.InputsInverted);
            route.Tick(deltaTime, state, bus, episodes, audioController, scene);
            SyncDoorStateAfterRouteTick(phaseBeforeRoute);
            TickStopProgression(stopsBeforeRoute, tutorialBeforeRoute);
            scene.TickStopBoarding(deltaTime);
            TickDoorConsequences(deltaTime);
            TickPassengerCount(deltaTime);
            oncomingBuses.Tick(deltaTime, state, bus);
            roadHazards.Tick(deltaTime, state, bus, audioController);
            SyncPassengerVisuals();
            episodes.Tick(deltaTime, state, mirror, audioController);
            visual.Tick(deltaTime, state, mirror.IsActive, episodes.LightsOutAmount);
            audioController.UpdateRunAudio(state, bus.DisplaySpeed, mirror.IsActive);
            ui.UpdateHud(state, route.TimeToNextStop, bus.DisplaySpeed, mirror.IsActive, episodes.ActiveEpisode, bus.RoadDanger, bus.SteeringVisual, PriorityRouteAlert(), mirrorThreat.WarningText, doorsOpen);
        }

        private void StartRun()
        {
            state = GameState.CreateNewRun(TerminalRouteSession.Mode);
            endingShown = false;
            endingSceneLoading = false;
            visiblePassengerCount = state.PassengerCount;
            elapsedRouteTime = 0f;
            endingSceneTimer = 0f;
            doorsOpen = false;
            introActive = true;
            introTimer = 0f;
            introVoiceDuration = 0f;
            introOpeningEyes = false;
            introEyeOpenTimer = 0f;
            passengerCountDue = false;
            passengerCountTimer = 0f;
            doorOpenDrivingTimer = 0f;
            passengerFallTimer = 0f;
            paused = false;
            routeAlertOverride = "";
            routeAlertOverrideTimer = 0f;
            mirrorCheckActive = false;
            mirrorCheckTimer = state.IsNightmare ? 9f : 18f;
            mirrorCheckWindow = 0f;

            scene.SetPlayObjectsVisible(true);
            scene.ResetDynamicObjects(state.PassengerCount);
            bus.Reset();
            bus.SetPaused(true);
            mirror.Reset();
            mirrorThreat.Reset();
            scene.SetMirrorMode(false);
            episodes.Reset();
            episodes.BeginLoop(state.Loop, state.Mode);
            route.Reset(state);
            oncomingBuses.Reset(state);
            roadHazards.Reset();
            visual.Reset();
            scene.SetDoorsOpen(doorsOpen);

            ui.ShowHud();
            ui.ShowPause(false);
            ui.ShowIntroStory();
            introVoiceDuration = audioController.PlayIntroVoice(state.IsNightmare);
            ui.UpdateHud(state, route.TimeToNextStop, bus.DisplaySpeed, false, episodes.ActiveEpisode, 0f, 0f, "", "", doorsOpen);
        }

        private void TogglePause()
        {
            if (state == null || state.Phase == GamePhase.Ended)
            {
                return;
            }

            paused = !paused;
            ui.ShowPause(paused);

            if (paused)
            {
                bus.SetPaused(true);
                return;
            }

            if (!introActive && state.Phase == GamePhase.Driving)
            {
                bus.SetPaused(false);
            }
        }

        private void ToggleDoors()
        {
            if (introActive || state.Phase == GamePhase.Ended)
            {
                return;
            }

            SetDoorsOpen(!doorsOpen);
        }

        private void SetDoorsOpen(bool open)
        {
            if (doorsOpen == open)
            {
                return;
            }

            doorsOpen = open;
            scene.SetDoorsOpen(open);
            audioController.PlayDoorHiss();
        }

        private void SyncDoorStateAfterRouteTick(GamePhase phaseBeforeRoute)
        {
            if (phaseBeforeRoute != GamePhase.Stopped && state.Phase == GamePhase.Stopped)
            {
                SetDoorsOpen(true);
            }
            else if (phaseBeforeRoute == GamePhase.Stopped && state.Phase == GamePhase.Driving)
            {
                SetDoorsOpen(false);
            }
        }

        private void SyncPassengerVisuals()
        {
            if (visiblePassengerCount == state.PassengerCount)
            {
                return;
            }

            visiblePassengerCount = state.PassengerCount;
            scene.SetPassengerCount(visiblePassengerCount);
        }

        private void TickIntro(float deltaTime)
        {
            introTimer += deltaTime;
            if (TerminalRouteInput.StartPressed() || TerminalRouteInput.CancelPressed())
            {
                BeginIntroEyeOpen(true);
                return;
            }

            if (introOpeningEyes)
            {
                introEyeOpenTimer += deltaTime;
                float openAmount = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(introEyeOpenTimer / 2.15f));
                ui.TickIntroStory(deltaTime, introTimer, openAmount, false);
                if (openAmount >= 1f)
                {
                    EndIntro();
                }

                return;
            }

            ui.TickIntroStory(deltaTime, introTimer, 0f, true);

            float fallbackDuration = introVoiceDuration > 0.1f ? introVoiceDuration + 0.35f : 10.5f;
            if (introTimer >= fallbackDuration)
            {
                BeginIntroEyeOpen(false);
            }
        }

        private void BeginIntroEyeOpen(bool skipVoice)
        {
            if (!introActive)
            {
                return;
            }

            if (introOpeningEyes)
            {
                return;
            }

            if (skipVoice)
            {
                audioController.StopVoice();
            }

            introOpeningEyes = true;
            introEyeOpenTimer = 0f;
            ui.TickIntroStory(0f, introTimer, 0f, false);
        }

        private void EndIntro()
        {
            if (!introActive)
            {
                return;
            }

            introActive = false;
            introOpeningEyes = false;
            audioController.StopVoice();
            ui.HideIntroStory();
            audioController.PlayRunLoop();
            if (state.IsNightmare)
            {
                audioController.PlayNightmareStart();
            }

            bus.SetPaused(false);
            ShowRouteAlert(state.IsNightmare ? "PESADELO  //  20 PARAGENS" : "TUTORIAL  //  ENCOSTA A DIREITA", 4.0f);
        }

        private void TickStopProgression(int stopsBeforeRoute, bool tutorialBeforeRoute)
        {
            if (tutorialBeforeRoute && !state.TutorialActive)
            {
                ShowRouteAlert(state.IsNightmare ? "TUTORIAL COMPLETO  //  PESADELO" : "TUTORIAL COMPLETO  //  ROTA 04", 3.0f);
                audioController.PlayCountConfirm();
            }

            if (state.Phase == GamePhase.Ended)
            {
                return;
            }

            if (state.StopsReached > stopsBeforeRoute)
            {
                BeginPassengerCountCheck();
            }
        }

        private void BeginPassengerCountCheck()
        {
            passengerCountDue = true;
            passengerCountTimer = state.IsNightmare ? 5.2f : 8.0f;
            ShowRouteAlert("CONTA OS PASSAGEIROS  //  Q NO ESPELHO", 4.0f);
            audioController.PlayDispatcher();
        }

        private void TickPassengerCount(float deltaTime)
        {
            if (state.Phase == GamePhase.Ended)
            {
                return;
            }

            if (!passengerCountDue)
            {
                if (TerminalRouteInput.CountPressed() && mirror.IsActive)
                {
                    ShowRouteAlert("CONTAGEM  //  " + state.PassengerCount + " PASS.", 1.6f);
                    audioController.PlayCountConfirm();
                }

                return;
            }

            passengerCountTimer -= deltaTime;
            if (TerminalRouteInput.CountPressed())
            {
                if (mirror.IsActive)
                {
                    passengerCountDue = false;
                    state.ApplySanityDelta(5f);
                    ShowRouteAlert("CONTAGEM CONFIRMADA", 2.4f);
                    audioController.PlayCountConfirm();
                }
                else
                {
                    state.ApplySanityDelta(-2f);
                    ShowRouteAlert("OLHA PELO ESPELHO PARA CONTAR", 1.8f);
                    audioController.PlayCountFailure();
                }
            }

            if (passengerCountDue && passengerCountTimer <= 0f)
            {
                passengerCountDue = false;
                state.ApplySanityDelta(-14f);
                if (state.IsNightmare && state.PassengerCount > 0)
                {
                    state.LosePassenger();
                    visiblePassengerCount = -1;
                }

                ShowRouteAlert("CONTAGEM FALHOU", 2.8f);
                audioController.PlayCountFailure();
            }
        }

        private void TickMirrorCheck(float deltaTime)
        {
            if (state.Phase != GamePhase.Driving || introActive)
            {
                return;
            }

            if (mirrorCheckActive)
            {
                mirrorCheckWindow -= deltaTime;
                ShowRouteAlert("RUIDO ATRAS  //  VERIFICA O ESPELHO", 0.25f);

                if (mirror.IsActive)
                {
                    mirrorCheckActive = false;
                    state.ApplySanityDelta(state.IsNightmare ? 2f : 4f);
                    ShowRouteAlert("ESPELHO CONFIRMADO", 1.4f);
                    audioController.PlayMirrorCheck();
                    ScheduleMirrorCheck();
                }
                else if (mirrorCheckWindow <= 0f)
                {
                    mirrorCheckActive = false;
                    state.ApplySanityDelta(state.IsNightmare ? -12f : -7f);
                    ShowRouteAlert("NAO VERIFICASTE O ESPELHO", 2.2f);
                    audioController.PlayCountFailure();
                    ScheduleMirrorCheck();
                }

                return;
            }

            mirrorCheckTimer -= deltaTime;
            if (mirrorCheckTimer <= 0f)
            {
                mirrorCheckActive = true;
                mirrorCheckWindow = state.IsNightmare ? 4.5f : 6.5f;
                audioController.PlayDispatcher();
            }
        }

        private void ScheduleMirrorCheck()
        {
            mirrorCheckTimer = Random.Range(state.IsNightmare ? 8f : 18f, state.IsNightmare ? 14f : 28f);
            mirrorCheckWindow = 0f;
        }

        private void TickDoorConsequences(float deltaTime)
        {
            if (!doorsOpen || state.Phase != GamePhase.Driving)
            {
                doorOpenDrivingTimer = 0f;
                passengerFallTimer = 0f;
                return;
            }

            doorOpenDrivingTimer += deltaTime;
            passengerFallTimer += deltaTime;
            float doorDrain = state.IsNightmare
                ? Mathf.Lerp(5.0f, 12.0f, Mathf.Clamp01(bus.DisplaySpeed / 58f))
                : Mathf.Lerp(2.6f, 7.4f, Mathf.Clamp01(bus.DisplaySpeed / 52f));
            state.ApplySanityDelta(-deltaTime * doorDrain);

            if (doorOpenDrivingTimer > 0.35f)
            {
                ShowRouteAlert("PORTAS ABERTAS  //  FECHA AS PORTAS", 0.45f);
            }

            float fallDelay = state.IsNightmare ? 2.4f : 3.8f;
            if (passengerFallTimer >= fallDelay && state.PassengerCount > 0)
            {
                state.LosePassenger();
                visiblePassengerCount = -1;
                passengerFallTimer = 0.9f;
                ShowRouteAlert("ALGUEM SAIU DO AUTOCARRO", 2.8f);
                audioController.PlayDoorDanger();
            }
        }

        private void ShowRouteAlert(string text, float seconds)
        {
            routeAlertOverride = text;
            routeAlertOverrideTimer = Mathf.Max(routeAlertOverrideTimer, seconds);
        }

        private void TickRouteAlertOverride(float deltaTime)
        {
            if (routeAlertOverrideTimer <= 0f)
            {
                routeAlertOverride = "";
                return;
            }

            routeAlertOverrideTimer -= deltaTime;
            if (routeAlertOverrideTimer <= 0f)
            {
                routeAlertOverride = "";
            }
        }

        private void FinishRun()
        {
            if (endingShown)
            {
                return;
            }

            endingShown = true;
            paused = false;
            bus.SetPaused(true);
            mirror.Reset();
            TerminalRouteSession.FinishRun(state, elapsedRouteTime);
            ui.ShowRouteEndFlash(state.Ending, state.EndingCause);
            audioController.PlayEndingTone(state.Ending);
            endingSceneTimer = state.Ending == EndingId.GoodTrip ? 2.1f : 3.1f;
        }

        private string PriorityRouteAlert()
        {
            if (!string.IsNullOrEmpty(routeAlertOverride))
            {
                return routeAlertOverride;
            }

            return !string.IsNullOrEmpty(roadHazards.WarningText) ? roadHazards.WarningText : route.AlertText;
        }

        private void TickEndingTransition(float deltaTime)
        {
            if (!endingShown)
            {
                FinishRun();
                return;
            }

            if (endingSceneLoading)
            {
                return;
            }

            ui.TickRouteEndFlash(deltaTime);
            endingSceneTimer -= deltaTime;
            if (endingSceneTimer <= 0f)
            {
                endingSceneLoading = true;
                SceneManager.LoadScene("Ending", LoadSceneMode.Single);
            }
        }

        private void ReturnToMenu()
        {
            SceneManager.LoadScene("Menu");
        }

        private static void DestroyEditorPreviewObjects()
        {
            var preview = GameObject.Find("Terminal Route Editor Preview");
            if (preview != null)
            {
                Destroy(preview);
            }
        }
    }
}
