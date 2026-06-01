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

        private void Awake()
        {
            Application.targetFrameRate = 60;
            DestroyEditorPreviewObjects();

            scene = new SceneFactory();
            scene.Build();

            ui = new TerminalRouteUi();
            ui.Build(StartRun, ReturnToMenu, ReturnToMenu, ToggleDoors);

            bus = new BusController(scene.Rig, scene.SteeringWheel);
            mirror = new MirrorView(scene.MainCamera.transform, ui);
            episodes = new EpisodeManager(scene);
            route = new RouteManager();
            visual = new VisualDegradation(scene.MainCamera.transform, ui);
            audioController = new TerminalRouteAudio(gameObject);
            mirrorThreat = new MirrorThreatManager(mirror, scene.BuildCloseMirrorThreat(), audioController);
            oncomingBuses = new OncomingBusManager(scene);
            roadHazards = new RoadHazardManager();

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
                state.ResolveEnding(EndingId.LongRoute, EndingCause.ManualExit);
                FinishRun();
                return;
            }

            if (TerminalRouteInput.DoorPressed())
            {
                ToggleDoors();
            }

            float deltaTime = Time.deltaTime;
            elapsedRouteTime += deltaTime;
            GamePhase phaseBeforeRoute = state.Phase;
            mirror.Tick(deltaTime);
            scene.SetMirrorMode(mirror.IsActive);
            mirrorThreat.Tick(deltaTime, state);
            bus.Tick(deltaTime, state, episodes.InputsInverted);
            route.Tick(deltaTime, state, bus, episodes, audioController, scene);
            SyncDoorStateAfterRouteTick(phaseBeforeRoute);
            scene.TickStopBoarding(deltaTime);
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
            state = GameState.CreateNewRun();
            endingShown = false;
            endingSceneLoading = false;
            visiblePassengerCount = state.PassengerCount;
            elapsedRouteTime = 0f;
            endingSceneTimer = 0f;
            doorsOpen = false;

            scene.SetPlayObjectsVisible(true);
            scene.ResetDynamicObjects(state.PassengerCount);
            bus.Reset();
            mirror.Reset();
            mirrorThreat.Reset();
            scene.SetMirrorMode(false);
            episodes.Reset();
            episodes.BeginLoop(state.Loop);
            route.Reset();
            oncomingBuses.Reset();
            roadHazards.Reset();
            visual.Reset();
            scene.SetDoorsOpen(doorsOpen);
            audioController.PlayRunLoop();
            ui.ShowHud();
            ui.UpdateHud(state, route.TimeToNextStop, bus.DisplaySpeed, false, episodes.ActiveEpisode, 0f, 0f, "", "", doorsOpen);
        }

        private void ToggleDoors()
        {
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

        private void FinishRun()
        {
            if (endingShown)
            {
                return;
            }

            endingShown = true;
            bus.SetPaused(true);
            mirror.Reset();
            TerminalRouteSession.FinishRun(state, elapsedRouteTime);
            ui.ShowRouteEndFlash(state.Ending, state.EndingCause);
            audioController.PlayEndingTone();
            endingSceneTimer = 2.1f;
        }

        private string PriorityRouteAlert()
        {
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
