using System;
using TerminalRoute.Core;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif
using UnityEngine.UI;

namespace TerminalRoute.Runtime
{
    public enum TerminalRouteLanguage
    {
        Portuguese,
        English
    }

    public sealed class TerminalRouteUi
    {
        private static TerminalRouteLanguage language = TerminalRouteLanguage.English;

        private Font font;
        private GameObject menuPanel;
        private GameObject hudPanel;
        private GameObject endingPanel;
        private GameObject creditsPanel;
        private GameObject controlsPanel;
        private GameObject mirrorPanel;
        private GameObject pausePanel;
        private GameObject routeEndFlashPanel;
        private RawImage cockpitOverlay;
        private RawImage endingArtwork;
        private Material cockpitPixelMaterial;
        private Material uiPixelMaterial;
        private Material vhsOverlayMaterial;
        private Image sanityOverlay;
        private Image lightsOutOverlay;
        private Image cabinStatusBacking;
        private Image sanityBarFill;
        private Text clockText;
        private Text routeText;
        private Text passengerText;
        private Text sanityBarText;
        private Text speedText;
        private Text promptText;
        private Text eventText;
        private Image eventBacking;
        private Image eventAccentLeft;
        private Image eventAccentRight;
        private Image routeProgressFill;
        private Image missedPipA;
        private Image missedPipB;
        private Image promptBacking;
        private Text objectiveText;
        private Text phaseStatusText;
        private Text doorStatusText;
        private Text mirrorStatusText;
        private Text endingTitleText;
        private Text endingBodyText;
        private Text endingCauseText;
        private Text endingStatsText;
        private Text menuSubtitleText;
        private Text menuStartText;
        private Text menuNightmareText;
        private Image menuNightmareButtonImage;
        private Text menuControlsText;
        private Text menuCreditsText;
        private Text menuFullscreenText;
        private Text menuLanguageText;
        private Text doorButtonText;
        private Text pauseButtonText;
        private Text mirrorLabelText;
        private Text endingReturnText;
        private Text endingShortcutText;
        private Text creditsTitleText;
        private Text creditsBodyText;
        private Text creditsReturnText;
        private Text creditsShortcutText;
        private Text controlsTitleText;
        private Text controlsBodyText;
        private Text controlsReturnText;
        private Text controlsShortcutText;
        private Text pauseTitleText;
        private Text pauseBodyText;
        private Text pauseResumeText;
        private Text pauseMenuText;
        private Text pauseShortcutText;
        private Image routeEndTopLid;
        private Image routeEndBottomLid;
        private Image routeEndCenterDarkness;
        private Text routeEndFlashText;
        private Text routeEndFlashSubtitleText;
        private float routeEndFlashElapsed;
        private GameObject introPanel;
        private Text introTitleText;
        private Text introBodyText;
        private Text introContinueText;
        private Image introBackdrop;
        private Image introBacking;
        private Image introTopLid;
        private Image introBottomLid;
        private RectTransform introBodyRect;
        private Vector2 introBodyBasePosition;
        private RectTransform wheelSpokeA;
        private RectTransform wheelSpokeB;
        private bool lastDoorOpen;
        private bool nightmareUnlocked;
        private EndingId lastFlashEnding = EndingId.None;
        private EndingCause lastFlashCause = EndingCause.None;

        public void Build(Action startRun, Action quitGame)
        {
            Build(startRun, quitGame, startRun, null);
        }

        public void BuildMenu(Action startRun, Action nightmareRun, Action quitGame)
        {
            Build(startRun, quitGame, startRun, null, nightmareRun, null);
        }

        public void BuildMenu(Action startRun, Action nightmareRun, Action quitGame, Action logoClick)
        {
            Build(startRun, quitGame, startRun, null, nightmareRun, logoClick);
        }

        public void Build(Action startRun, Action quitGame, Action endingReturn)
        {
            Build(startRun, quitGame, endingReturn, null);
        }

        public void Build(Action startRun, Action quitGame, Action endingReturn, Action toggleDoors)
        {
            Build(startRun, quitGame, endingReturn, toggleDoors, null, null);
        }

        public void BuildRoute(Action startRun, Action returnToMenu, Action toggleDoors, Action resumeRun)
        {
            Build(startRun, returnToMenu, returnToMenu, toggleDoors, null, null, resumeRun, returnToMenu);
        }

        private void Build(Action startRun, Action quitGame, Action endingReturn, Action toggleDoors, Action nightmareRun, Action logoClick, Action pauseResume = null, Action pauseMenu = null)
        {
            font = Font.CreateDynamicFontFromOSFont("Consolas", 18);
            uiPixelMaterial = PixelMaterial(210, 0.04f, 0.08f, 0.0007f);

            var canvasObject = new GameObject("Terminal Route UI");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObject.AddComponent<GraphicRaycaster>();
            EnsureEventSystem();

            menuPanel = Panel("Menu", canvasObject.transform, Color.black);
            TextureLayer("Terminal Artwork", menuPanel.transform, "TerminalRoute/Art/MenuBackground");
            Panel("Menu Shade", menuPanel.transform, new Color(0f, 0f, 0f, 0.42f));
            HudBox("Title Sign Backing", menuPanel.transform, new Vector2(0.34f, 0.785f), new Vector2(610f, 132f), new Color(0.004f, 0.018f, 0.012f, 0.58f));
            HudBox("Menu Action Backing", menuPanel.transform, new Vector2(0.76f, 0.455f), new Vector2(390f, 338f), new Color(0.004f, 0.018f, 0.012f, 0.76f));
            BuildMenuTitle(menuPanel.transform, logoClick);
            menuSubtitleText = Text("", menuPanel.transform, new Vector2(0.34f, 0.652f), new Vector2(610f, 42f), 20, TextAnchor.MiddleCenter, new Color(0.82f, 0.88f, 0.80f, 0.92f));
            menuStartText = Button("", menuPanel.transform, new Vector2(0.76f, 0.555f), new Vector2(290f, 56f), startRun);
            if (nightmareRun != null)
            {
                menuNightmareText = Button("", menuPanel.transform, new Vector2(0.76f, 0.468f), new Vector2(290f, 52f), nightmareRun);
                menuNightmareButtonImage = menuNightmareText.transform.parent.GetComponent<Image>();
            }

            menuControlsText = Button("", menuPanel.transform, new Vector2(0.76f, nightmareRun != null ? 0.382f : 0.468f), new Vector2(250f, 48f), ShowControls);
            menuCreditsText = Button("", menuPanel.transform, new Vector2(0.76f, nightmareRun != null ? 0.305f : 0.382f), new Vector2(250f, 48f), ShowCredits);
            menuLanguageText = SmallButton("", menuPanel.transform, new Vector2(0.12f, 0.08f), new Vector2(180f, 34f), ToggleLanguage);
            menuFullscreenText = SmallButton("", menuPanel.transform, new Vector2(0.88f, 0.08f), new Vector2(170f, 34f), ToggleFullscreen);

            hudPanel = Panel("HUD", canvasObject.transform, new Color(0f, 0f, 0f, 0f));
            cockpitOverlay = TextureLayer("Cockpit Artwork", hudPanel.transform, "TerminalRoute/Art/CockpitOverlay");
            cockpitPixelMaterial = PixelMaterial(224, 0.08f, 0.12f, 0.001f);
            if (cockpitPixelMaterial != null)
            {
                cockpitOverlay.material = cockpitPixelMaterial;
            }

            HudBox("Clock Backing", hudPanel.transform, new Vector2(0.078f, 0.946f), new Vector2(148f, 36f), new Color(0.005f, 0.018f, 0.012f, 0.46f));
            HudBox("Route Backing", hudPanel.transform, new Vector2(0.5f, 0.944f), new Vector2(500f, 34f), new Color(0.005f, 0.018f, 0.012f, 0.42f));
            HudBox("Route Progress Backing", hudPanel.transform, new Vector2(0.5f, 0.913f), new Vector2(360f, 5f), new Color(0.01f, 0.04f, 0.02f, 0.42f));
            routeProgressFill = HudBox("Route Progress Fill", hudPanel.transform, new Vector2(0.5f, 0.913f), new Vector2(360f, 5f), new Color(0.22f, 0.95f, 0.26f, 0.84f));
            ConfigureLeftFill(routeProgressFill.rectTransform, 360f);
            phaseStatusText = Text("", hudPanel.transform, new Vector2(0.5f, 0.895f), new Vector2(390f, 16f), 11, TextAnchor.MiddleCenter, new Color(0.55f, 0.76f, 0.58f, 0.92f));
            phaseStatusText.enabled = false;
            HudBox("Passenger Backing", hudPanel.transform, new Vector2(0.912f, 0.946f), new Vector2(150f, 36f), new Color(0.005f, 0.018f, 0.012f, 0.46f));
            HudBox("Miss Backing", hudPanel.transform, new Vector2(0.912f, 0.903f), new Vector2(86f, 10f), new Color(0f, 0f, 0f, 0f));
            missedPipA = HudBox("Miss Pip A", hudPanel.transform, new Vector2(0.894f, 0.903f), new Vector2(28f, 5f), new Color(0.95f, 0.12f, 0.08f, 0.96f));
            missedPipB = HudBox("Miss Pip B", hudPanel.transform, new Vector2(0.930f, 0.903f), new Vector2(28f, 5f), new Color(0.95f, 0.12f, 0.08f, 0.96f));
            missedPipA.enabled = false;
            missedPipB.enabled = false;
            cabinStatusBacking = HudBox("Cabin Status Backing", hudPanel.transform, new Vector2(0.082f, 0.836f), new Vector2(164f, 46f), new Color(0.005f, 0.018f, 0.012f, 0.38f));
            cabinStatusBacking.enabled = false;
            BuildSanityBar(hudPanel.transform);
            eventBacking = HudBox("Event Backing", hudPanel.transform, new Vector2(0.225f, 0.868f), new Vector2(420f, 38f), new Color(0.04f, 0.015f, 0.008f, 0.52f));
            eventAccentLeft = HudBox("Event Accent Left", hudPanel.transform, new Vector2(0.061f, 0.868f), new Vector2(3f, 38f), new Color(1.00f, 0.55f, 0.10f, 0.74f));
            eventAccentRight = HudBox("Event Accent Bottom", hudPanel.transform, new Vector2(0.225f, 0.837f), new Vector2(330f, 2f), new Color(1.00f, 0.55f, 0.10f, 0.54f));
            objectiveText = Text("", hudPanel.transform, new Vector2(0.5f, 0.786f), new Vector2(560f, 22f), 12, TextAnchor.MiddleCenter, new Color(0.66f, 0.90f, 0.68f, 0.88f));
            objectiveText.enabled = false;
            promptBacking = HudBox("Prompt Backing", hudPanel.transform, new Vector2(0.5f, 0.059f), new Vector2(340f, 28f), new Color(0.005f, 0.018f, 0.012f, 0.38f));
            HudBox("Speed Backing", hudPanel.transform, new Vector2(0.91f, 0.078f), new Vector2(132f, 34f), new Color(0.035f, 0.016f, 0.006f, 0.42f));
            wheelSpokeA = SteeringSpoke("Wheel Spoke A", hudPanel.transform, 0f);
            wheelSpokeB = SteeringSpoke("Wheel Spoke B", hudPanel.transform, 90f);
            clockText = Text("00:55", hudPanel.transform, new Vector2(0.078f, 0.946f), new Vector2(130f, 30f), 22, TextAnchor.MiddleLeft, new Color(0.34f, 1.00f, 0.40f, 0.95f));
            routeText = Text("ROTA 04  //  PARAGEM 00/08  //  FALHAS 0/2", hudPanel.transform, new Vector2(0.5f, 0.945f), new Vector2(470f, 26f), 14, TextAnchor.MiddleCenter, new Color(0.62f, 0.80f, 0.64f, 0.92f));
            passengerText = Text("22", hudPanel.transform, new Vector2(0.912f, 0.946f), new Vector2(130f, 30f), 22, TextAnchor.MiddleRight, new Color(0.34f, 1.00f, 0.40f, 0.95f));
            eventText = Text("", hudPanel.transform, new Vector2(0.225f, 0.868f), new Vector2(388f, 32f), 17, TextAnchor.MiddleLeft, new Color(1.00f, 0.68f, 0.18f));
            speedText = Text("42", hudPanel.transform, new Vector2(0.91f, 0.078f), new Vector2(115f, 30f), 20, TextAnchor.MiddleRight, new Color(0.98f, 0.62f, 0.18f, 0.96f));
            promptText = Text("A/D  DIRECAO    F  ESPELHO", hudPanel.transform, new Vector2(0.5f, 0.059f), new Vector2(320f, 24f), 13, TextAnchor.MiddleCenter, new Color(0.54f, 1.00f, 0.54f, 0.92f));
            promptBacking.enabled = false;
            promptText.enabled = false;
            doorStatusText = Text("", hudPanel.transform, new Vector2(0.082f, 0.846f), new Vector2(146f, 18f), 11, TextAnchor.MiddleCenter, new Color(0.54f, 1.00f, 0.54f));
            mirrorStatusText = Text("", hudPanel.transform, new Vector2(0.082f, 0.824f), new Vector2(146f, 18f), 11, TextAnchor.MiddleCenter, new Color(0.54f, 1.00f, 0.54f));
            doorStatusText.enabled = false;
            mirrorStatusText.enabled = false;
            if (toggleDoors != null)
            {
                doorButtonText = SmallButton("", hudPanel.transform, new Vector2(0.112f, 0.078f), new Vector2(156f, 32f), toggleDoors);
            }

            if (pauseResume != null)
            {
                pauseButtonText = SmallButton("", hudPanel.transform, new Vector2(0.112f, 0.124f), new Vector2(156f, 28f), pauseResume);
            }

            mirrorPanel = Panel("Mirror Overlay", canvasObject.transform, new Color(0f, 0f, 0f, 0f));
            AddOutline(mirrorPanel.transform);
            HudBox("Mirror Label Backing", mirrorPanel.transform, new Vector2(0.5f, 0.91f), new Vector2(230f, 28f), new Color(0.01f, 0.04f, 0.02f, 0.46f));
            mirrorLabelText = Text("", mirrorPanel.transform, new Vector2(0.5f, 0.91f), new Vector2(220f, 26f), 15, TextAnchor.MiddleCenter, new Color(0.34f, 1.00f, 0.40f));

            endingPanel = Panel("Ending", canvasObject.transform, Color.black);
            endingArtwork = TextureLayer("Ending Artwork", endingPanel.transform, "TerminalRoute/Art/EndingLong");
            Panel("Ending Shade", endingPanel.transform, new Color(0f, 0f, 0f, 0.76f));
            HudBox("Result Backing", endingPanel.transform, new Vector2(0.5f, 0.50f), new Vector2(860f, 430f), new Color(0.004f, 0.018f, 0.012f, 0.82f));
            HudBox("Result Header Line", endingPanel.transform, new Vector2(0.5f, 0.773f), new Vector2(760f, 4f), new Color(0.20f, 0.95f, 0.20f, 0.52f));
            HudBox("Result Footer Line", endingPanel.transform, new Vector2(0.5f, 0.243f), new Vector2(760f, 4f), new Color(0.20f, 0.95f, 0.20f, 0.30f));
            HudBox("Result Left Accent", endingPanel.transform, new Vector2(0.166f, 0.50f), new Vector2(4f, 340f), new Color(0.20f, 0.95f, 0.20f, 0.58f));
            HudBox("Result Right Accent", endingPanel.transform, new Vector2(0.834f, 0.50f), new Vector2(4f, 340f), new Color(0.20f, 0.95f, 0.20f, 0.35f));
            endingTitleText = Text("", endingPanel.transform, new Vector2(0.5f, 0.69f), new Vector2(820f, 82f), 58, TextAnchor.MiddleCenter, new Color(0.30f, 0.95f, 0.30f));
            endingCauseText = Text("", endingPanel.transform, new Vector2(0.5f, 0.585f), new Vector2(800f, 46f), 23, TextAnchor.MiddleCenter, new Color(0.98f, 0.35f, 0.20f));
            endingStatsText = Text("", endingPanel.transform, new Vector2(0.5f, 0.455f), new Vector2(780f, 112f), 23, TextAnchor.MiddleCenter, new Color(0.84f, 0.88f, 0.78f));
            endingBodyText = Text("", endingPanel.transform, new Vector2(0.5f, 0.338f), new Vector2(780f, 42f), 18, TextAnchor.MiddleCenter, new Color(0.58f, 0.72f, 0.60f));
            endingReturnText = Button("", endingPanel.transform, new Vector2(0.5f, 0.255f), new Vector2(360f, 58f), endingReturn);
            endingShortcutText = Text("", endingPanel.transform, new Vector2(0.5f, 0.178f), new Vector2(620f, 40f), 18, TextAnchor.MiddleCenter, new Color(0.55f, 0.62f, 0.58f));

            creditsPanel = Panel("Credits", canvasObject.transform, Color.black);
            TextureLayer("Credits Artwork", creditsPanel.transform, "TerminalRoute/Art/MenuBackground");
            Panel("Credits Shade", creditsPanel.transform, new Color(0f, 0f, 0f, 0.78f));
            creditsTitleText = Text("", creditsPanel.transform, new Vector2(0.5f, 0.72f), new Vector2(620f, 70f), 46, TextAnchor.MiddleCenter, new Color(0.30f, 0.95f, 0.30f));
            creditsBodyText = Text("", creditsPanel.transform, new Vector2(0.5f, 0.50f), new Vector2(930f, 330f), 20, TextAnchor.MiddleCenter, new Color(0.82f, 0.88f, 0.80f));
            creditsReturnText = Button("", creditsPanel.transform, new Vector2(0.5f, 0.22f), new Vector2(220f, 54f), ShowMenu);
            creditsShortcutText = Text("", creditsPanel.transform, new Vector2(0.5f, 0.15f), new Vector2(420f, 32f), 17, TextAnchor.MiddleCenter, new Color(0.55f, 0.62f, 0.58f));

            controlsPanel = Panel("Controls", canvasObject.transform, Color.black);
            TextureLayer("Controls Artwork", controlsPanel.transform, "TerminalRoute/Art/MenuBackground");
            Panel("Controls Shade", controlsPanel.transform, new Color(0f, 0f, 0f, 0.82f));
            HudBox("Controls Backing", controlsPanel.transform, new Vector2(0.5f, 0.52f), new Vector2(780f, 420f), new Color(0.01f, 0.03f, 0.02f, 0.72f));
            controlsTitleText = Text("", controlsPanel.transform, new Vector2(0.5f, 0.72f), new Vector2(620f, 70f), 44, TextAnchor.MiddleCenter, new Color(0.30f, 0.95f, 0.30f));
            controlsBodyText = Text("", controlsPanel.transform, new Vector2(0.5f, 0.51f), new Vector2(820f, 260f), 24, TextAnchor.MiddleCenter, new Color(0.82f, 0.88f, 0.80f));
            controlsReturnText = Button("", controlsPanel.transform, new Vector2(0.5f, 0.25f), new Vector2(220f, 54f), ShowMenu);
            controlsShortcutText = Text("", controlsPanel.transform, new Vector2(0.5f, 0.18f), new Vector2(420f, 32f), 17, TextAnchor.MiddleCenter, new Color(0.55f, 0.62f, 0.58f));

            var overlay = new GameObject("Sanity Distortion Overlay");
            overlay.transform.SetParent(canvasObject.transform, false);
            sanityOverlay = overlay.AddComponent<Image>();
            sanityOverlay.color = new Color(0.55f, 0.02f, 0.04f, 0f);
            sanityOverlay.raycastTarget = false;
            Stretch(overlay.GetComponent<RectTransform>());
            var blackout = new GameObject("Lights Out Overlay");
            blackout.transform.SetParent(canvasObject.transform, false);
            lightsOutOverlay = blackout.AddComponent<Image>();
            lightsOutOverlay.color = new Color(0f, 0f, 0f, 0f);
            lightsOutOverlay.raycastTarget = false;
            Stretch(blackout.GetComponent<RectTransform>());
            routeEndFlashPanel = Panel("Route End Flash", canvasObject.transform, new Color(0f, 0f, 0f, 0f));
            routeEndCenterDarkness = HudBox("Route End Darkness", routeEndFlashPanel.transform, new Vector2(0.5f, 0.5f), new Vector2(1280f, 720f), new Color(0f, 0f, 0f, 0f));
            routeEndTopLid = HudBox("Driver Top Eyelid", routeEndFlashPanel.transform, new Vector2(0.5f, 1.0f), new Vector2(1280f, 0f), Color.black);
            routeEndTopLid.rectTransform.pivot = new Vector2(0.5f, 1f);
            routeEndBottomLid = HudBox("Driver Bottom Eyelid", routeEndFlashPanel.transform, new Vector2(0.5f, 0.0f), new Vector2(1280f, 0f), Color.black);
            routeEndBottomLid.rectTransform.pivot = new Vector2(0.5f, 0f);
            routeEndFlashText = Text("", routeEndFlashPanel.transform, new Vector2(0.5f, 0.54f), new Vector2(980f, 92f), 54, TextAnchor.MiddleCenter, new Color(0.95f, 0.20f, 0.16f, 0f));
            routeEndFlashSubtitleText = Text("", routeEndFlashPanel.transform, new Vector2(0.5f, 0.43f), new Vector2(760f, 42f), 22, TextAnchor.MiddleCenter, new Color(0.70f, 0.86f, 0.70f, 0f));
            BuildIntroPanel(canvasObject.transform);
            BuildPausePanel(canvasObject.transform, pauseResume, pauseMenu);
            BuildVhsOverlay(canvasObject.transform);
            ApplyLanguage();
        }

        public void ShowMenu()
        {
            menuPanel.SetActive(true);
            hudPanel.SetActive(false);
            endingPanel.SetActive(false);
            creditsPanel.SetActive(false);
            controlsPanel.SetActive(false);
            pausePanel.SetActive(false);
            routeEndFlashPanel.SetActive(false);
            mirrorPanel.SetActive(false);
            introPanel.SetActive(false);
            SetVisualDistortion(0f);
            SetLightsOut(0f);
            ApplyLanguage();
        }

        public void SetNightmareUnlocked(bool unlocked)
        {
            nightmareUnlocked = unlocked;
            ApplyLanguage();
        }

        public void ShowNightmareLockedMessage()
        {
            if (menuSubtitleText != null)
            {
                menuSubtitleText.text = L("Completa a rota normal para desbloquear o pesadelo.", "Complete the normal route to unlock Nightmare.");
            }
        }

        public void ShowNightmareUnlockProgress(int clicks)
        {
            if (menuSubtitleText != null)
            {
                int remaining = Mathf.Max(0, 5 - clicks);
                menuSubtitleText.text = L("O sinal respondeu. Faltam " + remaining + " toques.", "The sign answered. " + remaining + " taps left.");
            }
        }

        public void ShowNightmareUnlockedMessage()
        {
            SetNightmareUnlocked(true);
            if (menuSubtitleText != null)
            {
                menuSubtitleText.text = L("Modo Pesadelo desbloqueado.", "Nightmare mode unlocked.");
            }
        }

        public void ShowHud()
        {
            menuPanel.SetActive(false);
            hudPanel.SetActive(true);
            endingPanel.SetActive(false);
            creditsPanel.SetActive(false);
            controlsPanel.SetActive(false);
            pausePanel.SetActive(false);
            routeEndFlashPanel.SetActive(false);
            if (cockpitOverlay != null)
            {
                cockpitOverlay.enabled = true;
            }
        }

        public void ShowEnding(EndingId ending)
        {
            ShowEnding(ending, TerminalRouteSession.EndingCause, TerminalRouteSession.StopsReached, TerminalRouteSession.MissedStops, TerminalRouteSession.FinalSanity, TerminalRouteSession.ElapsedRouteTime, TerminalRouteSession.TargetStopCount);
        }

        public void ShowEnding(EndingId ending, EndingCause cause, int stopsReached, int missedStops, float finalSanity, float elapsedTime)
        {
            ShowEnding(ending, cause, stopsReached, missedStops, finalSanity, elapsedTime, GameState.FinalStopCount);
        }

        public void ShowEnding(EndingId ending, EndingCause cause, int stopsReached, int missedStops, float finalSanity, float elapsedTime, int targetStopCount)
        {
            ApplyLanguage();
            menuPanel.SetActive(false);
            hudPanel.SetActive(false);
            endingPanel.SetActive(true);
            creditsPanel.SetActive(false);
            controlsPanel.SetActive(false);
            pausePanel.SetActive(false);
            routeEndFlashPanel.SetActive(false);
            mirrorPanel.SetActive(false);
            introPanel.SetActive(false);
            SetVisualDistortion(0f);
            SetLightsOut(0f);

            if (ending == EndingId.GoodTrip)
            {
                endingArtwork.texture = Resources.Load<Texture2D>("TerminalRoute/Art/EndingGood");
                endingTitleText.text = IsEnglish ? "ROUTE COMPLETE" : "ROTA COMPLETA";
                endingTitleText.color = new Color(0.34f, 1.00f, 0.40f);
                endingCauseText.color = new Color(0.66f, 0.92f, 0.68f);
                endingCauseText.text = IsEnglish ? "THE LAST STOP IS BEHIND YOU" : "A ULTIMA PARAGEM FICOU PARA TRAS";
                endingBodyText.text = IsEnglish ? "The terminal lights are still on. For now." : "As luzes do terminal continuam acesas. Por agora.";
            }
            else
            {
                endingArtwork.texture = Resources.Load<Texture2D>("TerminalRoute/Art/EndingLong");
                endingTitleText.text = IsEnglish ? "YOU LOST" : "PERDESTE";
                endingTitleText.color = new Color(0.95f, 0.20f, 0.16f);
                endingCauseText.color = new Color(1.00f, 0.48f, 0.20f);
                endingCauseText.text = (IsEnglish ? "CAUSE  //  " : "CAUSA  //  ") + CauseLabel(cause);
                endingBodyText.text = EndingFailureLine(cause);
            }

            endingStatsText.text =
                L("MODO", "MODE") + "  " + (TerminalRouteSession.Mode == GameMode.Nightmare ? L("PESADELO", "NIGHTMARE") : L("ROTA 04", "ROUTE 04")) + "     " +
                L("TEMPO", "TIME") + "  " + FormatTime(elapsedTime) + "\n" +
                L("PARAGENS", "STOPS") + "  " + Mathf.Clamp(stopsReached, 0, targetStopCount).ToString("00") + "/" + targetStopCount.ToString("00") + "     " +
                L("FALHAS", "MISSES") + "  " + Mathf.Clamp(missedStops, 0, GameState.MaxMissedStops) + "/" + GameState.MaxMissedStops + "\n" +
                L("SANIDADE", "SANITY") + "  " + Mathf.RoundToInt(finalSanity).ToString("00") + "%";
        }

        public bool IsCreditsVisible
        {
            get { return creditsPanel != null && creditsPanel.activeSelf; }
        }

        public bool IsControlsVisible
        {
            get { return controlsPanel != null && controlsPanel.activeSelf; }
        }

        public void ShowCredits()
        {
            menuPanel.SetActive(false);
            hudPanel.SetActive(false);
            endingPanel.SetActive(false);
            creditsPanel.SetActive(true);
            controlsPanel.SetActive(false);
            pausePanel.SetActive(false);
            routeEndFlashPanel.SetActive(false);
            mirrorPanel.SetActive(false);
            introPanel.SetActive(false);
            SetVisualDistortion(0f);
            SetLightsOut(0f);
        }

        public void ShowControls()
        {
            menuPanel.SetActive(false);
            hudPanel.SetActive(false);
            endingPanel.SetActive(false);
            creditsPanel.SetActive(false);
            controlsPanel.SetActive(true);
            pausePanel.SetActive(false);
            routeEndFlashPanel.SetActive(false);
            mirrorPanel.SetActive(false);
            introPanel.SetActive(false);
            SetVisualDistortion(0f);
            SetLightsOut(0f);
        }

        public void ShowRouteEndFlash(EndingId ending, EndingCause cause)
        {
            lastFlashEnding = ending;
            lastFlashCause = cause;
            routeEndFlashElapsed = 0f;
            menuPanel.SetActive(false);
            hudPanel.SetActive(true);
            endingPanel.SetActive(false);
            creditsPanel.SetActive(false);
            controlsPanel.SetActive(false);
            pausePanel.SetActive(false);
            mirrorPanel.SetActive(false);
            introPanel.SetActive(false);
            routeEndFlashPanel.SetActive(true);
            if (cause == EndingCause.SanityZero)
            {
                SetSanityBar(0f);
            }

            routeEndFlashText.text = RouteEndFlashLabel(ending, cause);
            routeEndFlashSubtitleText.text = RouteEndFlashSubtitle(ending, cause);
            routeEndFlashText.color = ending == EndingId.GoodTrip
                ? new Color(0.34f, 1.00f, 0.40f, 0f)
                : new Color(0.95f, 0.20f, 0.16f, 0f);
            routeEndFlashSubtitleText.color = new Color(0.70f, 0.86f, 0.70f, 0f);
            SetRouteEndEyeAmount(ending == EndingId.GoodTrip ? 1f : 0f, 0f);
        }

        public void TickRouteEndFlash(float deltaTime)
        {
            if (routeEndFlashPanel == null || !routeEndFlashPanel.activeSelf)
            {
                return;
            }

            routeEndFlashElapsed += deltaTime;
            bool won = lastFlashEnding == EndingId.GoodTrip;
            if (won)
            {
                float fade = Mathf.Clamp01(routeEndFlashElapsed / 0.75f);
                SetRouteEndEyeAmount(1f, fade * 0.82f);
                routeEndFlashText.color = new Color(0.34f, 1.00f, 0.40f, Mathf.Clamp01((routeEndFlashElapsed - 0.25f) / 0.45f));
                routeEndFlashSubtitleText.color = new Color(0.70f, 0.86f, 0.70f, Mathf.Clamp01((routeEndFlashElapsed - 0.55f) / 0.50f));
                return;
            }

            float eyeClose = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(routeEndFlashElapsed / 1.75f));
            float darkness = Mathf.Clamp01((routeEndFlashElapsed - 0.35f) / 1.25f) * 0.92f;
            SetRouteEndEyeAmount(eyeClose, darkness);
            float textAlpha = Mathf.Clamp01((routeEndFlashElapsed - 1.35f) / 0.65f);
            routeEndFlashText.color = new Color(0.95f, 0.20f, 0.16f, textAlpha);
            routeEndFlashSubtitleText.color = new Color(0.70f, 0.86f, 0.70f, Mathf.Clamp01(textAlpha - 0.18f));
        }

        private void SetRouteEndEyeAmount(float amount, float darkness)
        {
            float lidHeight = Mathf.Lerp(0f, 380f, Mathf.Clamp01(amount));
            if (routeEndTopLid != null)
            {
                routeEndTopLid.rectTransform.sizeDelta = new Vector2(1280f, lidHeight);
            }

            if (routeEndBottomLid != null)
            {
                routeEndBottomLid.rectTransform.sizeDelta = new Vector2(1280f, lidHeight);
            }

            if (routeEndCenterDarkness != null)
            {
                routeEndCenterDarkness.color = new Color(0f, 0f, 0f, Mathf.Clamp01(darkness));
            }
        }

        public void ShowIntroStory()
        {
            if (introPanel == null)
            {
                return;
            }

            introPanel.SetActive(true);
            SetIntroEyeOpen(0f);
            ApplyLanguage();
        }

        public void HideIntroStory()
        {
            if (introPanel != null)
            {
                introPanel.SetActive(false);
            }
        }

        public void ShowPause(bool paused)
        {
            if (pausePanel == null)
            {
                return;
            }

            pausePanel.SetActive(paused);
            if (paused)
            {
                hudPanel.SetActive(true);
            }

            ApplyLanguage();
        }

        public void TickIntroStory(float deltaTime, float elapsed, float eyeOpenAmount, bool skipAvailable)
        {
            if (introPanel == null || !introPanel.activeSelf || introBodyRect == null)
            {
                return;
            }

            float textAlpha = Mathf.Clamp01(1f - eyeOpenAmount * 1.45f);
            float shakeScale = 1f - eyeOpenAmount;
            float shakeX = (Mathf.Sin(elapsed * 17f) * 0.28f + Mathf.Sin(elapsed * 39f) * 0.10f) * shakeScale;
            float shakeY = Mathf.Cos(elapsed * 21f) * 0.16f * shakeScale;
            introBodyRect.anchoredPosition = introBodyBasePosition + new Vector2(shakeX, shakeY);
            introTitleText.color = new Color(0.34f, 1.00f, 0.40f, textAlpha);
            introBodyText.color = new Color(0.82f, 0.90f, 0.80f, textAlpha);
            introContinueText.color = new Color(0.54f, 1.00f, 0.54f, skipAvailable ? textAlpha * (0.50f + Mathf.PingPong(elapsed * 0.85f, 0.45f)) : textAlpha * 0.38f);
            SetIntroEyeOpen(eyeOpenAmount);
        }

        private void SetIntroEyeOpen(float eyeOpenAmount)
        {
            float open = Mathf.Clamp01(eyeOpenAmount);
            float closed = 1f - open;
            float lidHeight = Mathf.Lerp(0f, 390f, closed);

            if (introBackdrop != null)
            {
                introBackdrop.color = new Color(0f, 0f, 0f, Mathf.Lerp(0f, 0.97f, closed));
            }

            if (introBacking != null)
            {
                introBacking.color = new Color(0.01f, 0.03f, 0.02f, 0.74f * closed);
            }

            if (introTopLid != null)
            {
                introTopLid.rectTransform.sizeDelta = new Vector2(1280f, lidHeight);
            }

            if (introBottomLid != null)
            {
                introBottomLid.rectTransform.sizeDelta = new Vector2(1280f, lidHeight);
            }
        }

        public static void ToggleFullscreen()
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.fullScreen = !Screen.fullScreen;
        }

        private void ToggleLanguage()
        {
            language = language == TerminalRouteLanguage.Portuguese ? TerminalRouteLanguage.English : TerminalRouteLanguage.Portuguese;
            ApplyLanguage();
        }

        private bool IsEnglish
        {
            get { return language == TerminalRouteLanguage.English; }
        }

        private string L(string portuguese, string english)
        {
            return IsEnglish ? english : portuguese;
        }

        private void ApplyLanguage()
        {
            if (menuSubtitleText != null)
            {
                menuSubtitleText.text = L("A ultima carreira nao aparece no horario.", "The last route is not on the schedule.");
                menuStartText.text = L("NOVA VIAGEM", "NEW TRIP");
                if (menuNightmareText != null)
                {
                    menuNightmareText.text = nightmareUnlocked ? L("MODO PESADELO", "NIGHTMARE MODE") : L("PESADELO BLOQUEADO", "NIGHTMARE LOCKED");
                    UpdateNightmareButtonVisual();
                }

                menuControlsText.text = L("CONTROLOS", "CONTROLS");
                menuCreditsText.text = L("CREDITOS", "CREDITS");
                menuFullscreenText.text = L("TELA CHEIA", "FULLSCREEN");
                menuLanguageText.text = L("IDIOMA: PT", "LANGUAGE: EN");
            }

            if (doorButtonText != null)
            {
                doorButtonText.text = lastDoorOpen ? L("FECHAR PORTAS", "CLOSE DOORS") : L("ABRIR PORTAS", "OPEN DOORS");
            }

            if (pauseButtonText != null)
            {
                pauseButtonText.text = L("PAUSA", "PAUSE");
            }

            if (mirrorLabelText != null)
            {
                mirrorLabelText.text = L("ESPELHO", "MIRROR");
            }

            if (endingReturnText != null)
            {
                endingReturnText.text = L("VOLTAR AO TERMINAL", "RETURN TO TERMINAL");
                endingShortcutText.text = L("ENTER  //  VOLTAR", "ENTER  //  RETURN");
            }

            if (creditsTitleText != null)
            {
                creditsTitleText.text = L("CREDITOS", "CREDITS");
                creditsBodyText.text = L(
                    "Terminal Route\n\nEquipa: Nero Soares & Paulo Monteiro\nDirecao, programacao e design: Grupo 4\nPrototipo tecnico: Unity 6000.3.8f1 + URP 17\n\nAssets externos: Elbolilloduro / itch.io\nCharacters PSX, Bus Stop, Roads Procedural\n\nArte de menu/cockpit/finais/macaco: gerada para este prototipo\nAudio: clips importados + fallback sintetizado\nURLs e licencas: ASSET_CREDITS.md",
                    "Terminal Route\n\nTeam: Nero Soares & Paulo Monteiro\nDirection, programming and design: Group 4\nTechnical prototype: Unity 6000.3.8f1 + URP 17\n\nExternal assets: Elbolilloduro / itch.io\nCharacters PSX, Bus Stop, Roads Procedural\n\nMenu/cockpit/ending/monkey art: generated for this prototype\nAudio: imported clips + synthesized fallback\nURLs and licenses: ASSET_CREDITS.md");
                creditsReturnText.text = L("VOLTAR", "BACK");
                creditsShortcutText.text = L("ESC  //  VOLTAR", "ESC  //  BACK");
            }

            if (controlsTitleText != null)
            {
                controlsTitleText.text = L("CONTROLOS", "CONTROLS");
                controlsBodyText.text = L(
                    "A / D ou setas  -  direcao\nF  -  olhar para o espelho\nE  -  abrir / fechar portas\nQ  -  confirmar contagem no espelho\nESC ou botao PAUSA  -  pausar a viagem\n\nEncosta a direita quando a paragem brilhar.\nFecha as portas antes de arrancar.\nSe alguem aparecer perto no espelho, olha para a frente.",
                    "A / D or arrows  -  steer\nF  -  look in the mirror\nE  -  open / close doors\nQ  -  confirm passenger count in the mirror\nESC or PAUSE button  -  pause the trip\n\nMove right when the stop glows.\nClose the doors before leaving.\nIf someone appears close in the mirror, look forward.");
                controlsReturnText.text = L("VOLTAR", "BACK");
                controlsShortcutText.text = L("ESC  //  VOLTAR", "ESC  //  BACK");
            }

            if (pauseTitleText != null)
            {
                pauseTitleText.text = L("ROTA EM PAUSA", "ROUTE PAUSED");
                pauseBodyText.text = L(
                    "SISTEMA DO AUTOCARRO EM ESPERA\nMantem a calma. Verifica portas, espelho e proxima paragem antes de continuar.",
                    "BUS SYSTEM ON HOLD\nStay calm. Check doors, mirror, and next stop before resuming.");
                pauseResumeText.text = L("CONTINUAR", "RESUME");
                pauseMenuText.text = L("VOLTAR AO MENU", "RETURN TO MENU");
                pauseShortcutText.text = L("ESC  //  CONTINUAR", "ESC  //  RESUME");
            }

            if (introTitleText != null)
            {
                bool nightmareIntro = TerminalRouteSession.Mode == GameMode.Nightmare;
                introTitleText.text = nightmareIntro ? L("RADIO 04  //  PESADELO", "RADIO 04  //  NIGHTMARE") : L("RADIO 04  //  CENTRAL", "RADIO 04  //  DISPATCH");
                introBodyText.text = nightmareIntro
                    ? L(
                        "Mantem os olhos fechados e ouve a chamada.\nQuando a mensagem terminar, assume o volante.\nA/D guia. E controla portas. F verifica o espelho.\nQ confirma a contagem quando entrarem passageiros.",
                        "Keep your eyes closed and listen to the call.\nWhen the message ends, take the wheel.\nA/D steers. E controls doors. F checks the mirror.\nQ confirms the count after passengers board.")
                    : L(
                        "Mantem os olhos fechados e ouve a chamada.\nQuando a mensagem terminar, assume o volante.\nA/D guia. Encosta a direita nas paragens verdes.\nE controla portas. F espelho. Q contagem.",
                        "Keep your eyes closed and listen to the call.\nWhen the message ends, take the wheel.\nA/D steers. Pull right into green stops.\nE controls doors. F mirror. Q count.");
                introContinueText.text = L("ENTER  //  SALTAR INTRO", "ENTER  //  SKIP INTRO");
            }

            if (routeEndFlashPanel != null && routeEndFlashPanel.activeSelf)
            {
                routeEndFlashText.text = RouteEndFlashLabel(lastFlashEnding, lastFlashCause);
                routeEndFlashSubtitleText.text = RouteEndFlashSubtitle(lastFlashEnding, lastFlashCause);
            }
        }

        public void SetMirror(bool active)
        {
            mirrorPanel.SetActive(active);
            if (cockpitOverlay != null)
            {
                cockpitOverlay.enabled = !active;
            }

            if (wheelSpokeA != null)
            {
                wheelSpokeA.gameObject.SetActive(!active);
                wheelSpokeB.gameObject.SetActive(!active);
            }
        }

        public void SetVisualDistortion(float amount)
        {
            sanityOverlay.color = new Color(0.55f, 0.02f, 0.04f, amount * 0.24f);
        }

        public void SetLightsOut(float amount)
        {
            if (lightsOutOverlay == null)
            {
                return;
            }

            float flicker = Mathf.PerlinNoise(Time.time * 26f, 0.77f) * 0.10f;
            lightsOutOverlay.color = new Color(0f, 0f, 0f, Mathf.Clamp01(amount) * (0.44f + flicker));
        }

        public void UpdateHud(GameState state, float timeToNextStop, float speed, bool mirrorActive, EpisodeType activeEpisode, float roadDanger, float steering, string routeAlert, string mirrorAlert, bool doorsOpen)
        {
            if (!hudPanel.activeSelf)
            {
                return;
            }

            lastDoorOpen = doorsOpen;
            int seconds = Mathf.CeilToInt(timeToNextStop);
            clockText.text = "00:" + seconds.ToString("00");
            string routeName = state.IsNightmare ? L("PESADELO", "NIGHTMARE") : L("ROTA 04", "ROUTE 04");
            routeText.text = state.TutorialActive
                ? L("TUTORIAL  //  PARAGEM DE TESTE", "TUTORIAL  //  TEST STOP")
                : routeName + L("  //  PARAGEM ", "  //  STOP ") + Mathf.Clamp(state.StopsReached, 0, state.TargetStopCount).ToString("00") + "/" + state.TargetStopCount.ToString("00") + "  //  " + L("FALHAS ", "MISSES ") + Mathf.Clamp(state.MissedStops, 0, GameState.MaxMissedStops) + "/" + GameState.MaxMissedStops;
            passengerText.text = PassengerDisplayText(state, activeEpisode, mirrorActive);
            speedText.text = Mathf.RoundToInt(speed).ToString("00") + " KM/H";
            promptText.text = mirrorActive ? L("F  VOLTAR    Q  CONTAGEM", "F  BACK    Q  COUNT") : L("A/D  DIRECAO    F  ESPELHO    E  PORTAS", "A/D  STEER    F  MIRROR    E  DOORS");
            bool showPrompt = state.TutorialActive || state.Phase == GamePhase.Stopped || mirrorActive || doorsOpen;
            promptText.enabled = showPrompt;
            if (promptBacking != null)
            {
                promptBacking.enabled = showPrompt;
            }

            if (doorButtonText != null)
            {
                doorButtonText.text = doorsOpen ? L("FECHAR PORTAS", "CLOSE DOORS") : L("ABRIR PORTAS", "OPEN DOORS");
            }

            SetSanityBar(state.Sanity);
            SetRouteProgress(state);
            SetMissPips(state.MissedStops);
            SetStatusReadouts(state, timeToNextStop, mirrorActive, doorsOpen, activeEpisode);
            string eventMessage = GetPriorityHudMessage(state, activeEpisode, roadDanger, routeAlert, mirrorAlert, doorsOpen);
            SetEventMessage(eventMessage, roadDanger, mirrorAlert, routeAlert, doorsOpen);
            SetSteering(steering);
        }

        private void SetRouteProgress(GameState state)
        {
            if (routeProgressFill == null)
            {
                return;
            }

            float normalized = state.TutorialActive ? 0f : Mathf.Clamp01(state.StopsReached / Mathf.Max(1f, state.TargetStopCount));
            var rect = routeProgressFill.rectTransform;
            rect.sizeDelta = new Vector2(360f * normalized, rect.sizeDelta.y);
            routeProgressFill.color = state.IsNightmare
                ? new Color(0.95f, 0.14f, 0.12f, 0.90f)
                : new Color(0.22f, 0.95f, 0.26f, 0.90f);
        }

        private void SetMissPips(int missedStops)
        {
            SetMissPip(missedPipA, missedStops >= 1);
            SetMissPip(missedPipB, missedStops >= 2);
        }

        private void SetMissPip(Image pip, bool missed)
        {
            if (pip == null)
            {
                return;
            }

            pip.enabled = missed;
            pip.color = new Color(0.95f, 0.12f, 0.08f, 0.96f);
        }

        private void SetStatusReadouts(GameState state, float timeToNextStop, bool mirrorActive, bool doorsOpen, EpisodeType activeEpisode)
        {
            if (phaseStatusText != null)
            {
                phaseStatusText.text = "";
                phaseStatusText.enabled = false;
            }

            bool showDoorStatus = doorsOpen || state.Phase == GamePhase.Stopped;
            bool showMirrorStatus = mirrorActive;
            if (cabinStatusBacking != null)
            {
                cabinStatusBacking.enabled = showDoorStatus || showMirrorStatus;
            }

            if (doorStatusText != null)
            {
                doorStatusText.text = doorsOpen ? L("PORTAS: ABERTAS", "DOORS: OPEN") : L("PORTAS: FECHADAS", "DOORS: CLOSED");
                doorStatusText.enabled = showDoorStatus;
                doorStatusText.color = doorsOpen && state.Phase == GamePhase.Driving
                    ? new Color(1.00f, 0.24f, 0.16f)
                    : new Color(0.54f, 1.00f, 0.54f);
            }

            if (mirrorStatusText != null)
            {
                mirrorStatusText.text = mirrorActive ? L("ESPELHO: ATIVO", "MIRROR: ACTIVE") : L("ESPELHO: PRONTO", "MIRROR: READY");
                mirrorStatusText.enabled = showMirrorStatus;
                mirrorStatusText.color = mirrorActive
                    ? new Color(1.00f, 0.62f, 0.18f)
                    : new Color(0.54f, 1.00f, 0.54f);
            }

            if (objectiveText != null)
            {
                objectiveText.text = "";
                objectiveText.enabled = false;
            }
        }

        private string ObjectiveLabel(GameState state, EpisodeType activeEpisode, bool mirrorActive, bool doorsOpen)
        {
            if (state.TutorialActive)
            {
                return L("OBJETIVO: ENCOSTA A DIREITA, PARA NA ZONA VERDE E FECHA AS PORTAS", "OBJECTIVE: MOVE RIGHT, STOP IN THE GREEN ZONE, CLOSE THE DOORS");
            }

            if (state.Phase == GamePhase.Stopped)
            {
                return doorsOpen
                    ? L("OBJETIVO: AGUARDA PASSAGEIROS, DEPOIS FECHA AS PORTAS", "OBJECTIVE: WAIT FOR PASSENGERS, THEN CLOSE THE DOORS")
                    : L("OBJETIVO: ABRE AS PORTAS NA PARAGEM", "OBJECTIVE: OPEN THE DOORS AT THE STOP");
            }

            if (doorsOpen)
            {
                return L("OBJETIVO: FECHA AS PORTAS ANTES QUE ALGUEM CAIA", "OBJECTIVE: CLOSE THE DOORS BEFORE SOMEONE FALLS");
            }

            if (mirrorActive)
            {
                return L("OBJETIVO: VERIFICA RAPIDO E VOLTA A ESTRADA", "OBJECTIVE: CHECK QUICKLY AND RETURN TO THE ROAD");
            }

            switch (activeEpisode)
            {
                case EpisodeType.InvertedControls:
                    return L("OBJETIVO: DIRECAO INVERTIDA, CORRIGE COM CALMA", "OBJECTIVE: STEERING INVERTED, CORRECT CALMLY");
                case EpisodeType.LightsOut:
                    return L("OBJETIVO: USA AS MARCAS DA ESTRADA", "OBJECTIVE: USE THE ROAD MARKINGS");
                case EpisodeType.Ball:
                    return L("OBJETIVO: NAO TE DISTRAIAS COM O CORREDOR", "OBJECTIVE: DO NOT GET DISTRACTED BY THE AISLE");
                case EpisodeType.Monkey:
                    return L("OBJETIVO: SE OUVIRES MOVIMENTO, ESPELHO CURTO", "OBJECTIVE: IF YOU HEAR MOVEMENT, QUICK MIRROR CHECK");
                case EpisodeType.Silence:
                    return L("OBJETIVO: MANTEM O AUTOCARRO NA FAIXA", "OBJECTIVE: KEEP THE BUS IN LANE");
                default:
                    return "";
            }
        }

        private void SetEventMessage(string message, float roadDanger, string mirrorAlert, string routeAlert, bool doorsOpen)
        {
            bool visible = !string.IsNullOrEmpty(message);
            eventText.text = message;
            eventText.enabled = visible;
            eventBacking.enabled = visible;
            eventAccentLeft.enabled = visible;
            eventAccentRight.enabled = visible;
            if (!visible)
            {
                return;
            }

            bool stopMessage = message.Contains("PARAGEM") || message.Contains("STOP");
            bool danger = !string.IsNullOrEmpty(mirrorAlert) ||
                roadDanger > 0.15f ||
                (doorsOpen && !stopMessage) ||
                (!string.IsNullOrEmpty(routeAlert) && (routeAlert.Contains("FALHOU") || routeAlert.Contains("PERDIDA") || routeAlert.Contains("ABERTAS") || routeAlert.Contains("NAO")));
            float pulse = 0.72f + Mathf.PingPong(Time.time * (danger ? 4.8f : 2.2f), 0.22f);
            Color accent = danger
                ? new Color(1.00f, 0.12f, 0.08f, pulse)
                : new Color(1.00f, 0.60f, 0.12f, pulse);
            eventBacking.color = danger
                ? new Color(0.14f, 0.01f, 0.005f, 0.68f)
                : new Color(0.045f, 0.020f, 0.006f, 0.50f);
            eventText.color = danger
                ? new Color(1.00f, 0.26f, 0.18f)
                : new Color(1.00f, 0.68f, 0.18f);
            eventAccentLeft.color = accent;
            eventAccentRight.color = accent;
        }

        private void SetSanityBar(float sanity)
        {
            if (sanityBarFill == null || sanityBarText == null)
            {
                return;
            }

            float normalized = Mathf.Clamp01(sanity / 100f);
            var rect = sanityBarFill.rectTransform;
            rect.sizeDelta = new Vector2(420f * normalized, rect.sizeDelta.y);
            sanityBarFill.color = normalized > 0.55f
                ? new Color(0.22f, 0.92f, 0.26f, 0.94f)
                : normalized > 0.28f
                    ? new Color(0.95f, 0.62f, 0.14f, 0.94f)
                    : new Color(0.95f, 0.12f, 0.10f, 0.94f);
            sanityBarText.text = L("SAN ", "SAN ") + Mathf.RoundToInt(sanity).ToString("00") + "%";
        }

        private void SetSteering(float steering)
        {
            float angle = Mathf.Clamp(steering, -1f, 1f) * -58f;
            wheelSpokeA.localRotation = Quaternion.Euler(0f, 0f, angle);
            wheelSpokeB.localRotation = Quaternion.Euler(0f, 0f, angle + 90f);
        }

        private string GetCabinMessage(GameState state, EpisodeType activeEpisode, bool doorsOpen)
        {
            if (state.TutorialActive && state.Phase == GamePhase.Driving)
            {
                return L("TUTORIAL  //  PARA NA ZONA VERDE", "TUTORIAL  //  STOP IN THE GREEN ZONE");
            }

            if (state.Phase == GamePhase.Stopped)
            {
                return doorsOpen ? L("PARAGEM  //  PORTAS ABERTAS", "STOP  //  DOORS OPEN") : L("PARAGEM  //  PORTAS FECHADAS", "STOP  //  DOORS CLOSED");
            }

            switch (activeEpisode)
            {
                case EpisodeType.Silence:
                    return L("CABINE  //  SEM RUIDO", "CABIN  //  NO SOUND");
                case EpisodeType.Monkey:
                    return L("CABINE  //  MOVIMENTO ATRAS", "CABIN  //  MOVEMENT BEHIND");
                case EpisodeType.Ball:
                    return L("CORREDOR  //  OBJETO SOLTO", "AISLE  //  LOOSE OBJECT");
                case EpisodeType.InvertedControls:
                    return L("CABINE  //  DIRECAO INVERTIDA", "CABIN  //  STEERING INVERTED");
                case EpisodeType.LightsOut:
                    return L("LUZES  //  FALHA ELETRICA", "LIGHTS  //  ELECTRICAL FAULT");
                default:
                    return "";
            }
        }

        private string GetPriorityHudMessage(GameState state, EpisodeType activeEpisode, float roadDanger, string routeAlert, string mirrorAlert, bool doorsOpen)
        {
            if (!string.IsNullOrEmpty(mirrorAlert))
            {
                return L(mirrorAlert, "LOOK FRONT");
            }

            if (!string.IsNullOrEmpty(routeAlert))
            {
                return LocalizeAlert(routeAlert);
            }

            if (roadDanger > 0.15f)
            {
                return L("ALERTA  //  VOLTA PARA A FAIXA", "ALERT  //  RETURN TO LANE");
            }

            return GetCabinMessage(state, activeEpisode, doorsOpen);
        }

        private string LocalizeAlert(string alert)
        {
            if (!IsEnglish)
            {
                return alert;
            }

            switch (alert)
            {
                case "APROXIMA-TE DA DIREITA":
                    return "MOVE TO THE RIGHT";
                case "APROXIMA-TE DA PARAGEM":
                    return "APPROACH THE STOP";
                case "ENCOSTA A DIREITA PARA PARAR":
                    return "MOVE RIGHT TO STOP";
                case "PARAGEM PERDIDA  //  APROXIMA-TE DA DIREITA":
                    return "STOP MISSED  //  MOVE TO THE RIGHT";
                case "OBJETO NA ESTRADA":
                    return "OBJECT ON ROAD";
                case "PESADELO  //  20 PARAGENS":
                    return "NIGHTMARE  //  20 STOPS";
                case "RUIDO ATRAS  //  VERIFICA O ESPELHO":
                    return "NOISE BEHIND  //  CHECK MIRROR";
                case "ESPELHO CONFIRMADO":
                    return "MIRROR CONFIRMED";
                case "NAO VERIFICASTE O ESPELHO":
                    return "YOU DID NOT CHECK THE MIRROR";
                case "TUTORIAL  //  ENCOSTA A DIREITA":
                    return "TUTORIAL  //  MOVE RIGHT";
                case "TUTORIAL  //  ENTRA NA ZONA VERDE":
                    return "TUTORIAL  //  ENTER THE GREEN ZONE";
                case "TUTORIAL  //  PARAGEM ASSISTIDA":
                    return "TUTORIAL  //  ASSISTED STOP";
                case "TUTORIAL COMPLETO  //  ROTA 04":
                    return "TUTORIAL COMPLETE  //  ROUTE 04";
                case "TUTORIAL COMPLETO  //  PESADELO":
                    return "TUTORIAL COMPLETE  //  NIGHTMARE";
                case "CONTA OS PASSAGEIROS  //  Q NO ESPELHO":
                    return "COUNT PASSENGERS  //  Q IN MIRROR";
                case "CONTAGEM CONFIRMADA":
                    return "COUNT CONFIRMED";
                case "OLHA PELO ESPELHO PARA CONTAR":
                    return "LOOK IN MIRROR TO COUNT";
                case "CONTAGEM FALHOU":
                    return "COUNT FAILED";
                case "PORTAS ABERTAS  //  FECHA AS PORTAS":
                    return "DOORS OPEN  //  CLOSE DOORS";
                case "ALGUEM SAIU DO AUTOCARRO":
                    return "SOMEONE LEFT THE BUS";
                default:
                    return alert;
            }
        }

        private string PassengerDisplayText(GameState state, EpisodeType activeEpisode, bool mirrorActive)
        {
            int count = state.PassengerCount;
            bool wrongCount = activeEpisode == EpisodeType.LightsOut || (activeEpisode == EpisodeType.Silence && mirrorActive) || (activeEpisode == EpisodeType.Monkey && mirrorActive);
            if (wrongCount)
            {
                count = Mathf.Max(0, count + 2 + (state.Loop % 4));
                return count + " PASS.?";
            }

            return count + " PASS.";
        }

        private string CauseLabel(EndingCause cause)
        {
            switch (cause)
            {
                case EndingCause.CompletedRoute:
                    return L("ROTA COMPLETA", "ROUTE COMPLETE");
                case EndingCause.SanityZero:
                    return L("SANIDADE ZERO", "SANITY ZERO");
                case EndingCause.RoadCrash:
                    return L("SAISTE DA ESTRADA", "LEFT THE ROAD");
                case EndingCause.MissedStops:
                    return L("DUAS PARAGENS PERDIDAS", "TWO MISSED STOPS");
                case EndingCause.OncomingBusCrash:
                    return L("COLISAO COM AUTOCARRO", "BUS COLLISION");
                case EndingCause.CloseNpcStare:
                    return L("OLHASTE DEMASIADO", "LOOKED TOO LONG");
                case EndingCause.ManualExit:
                    return L("VIAGEM INTERROMPIDA", "TRIP INTERRUPTED");
                default:
                    return L("DESCONHECIDA", "UNKNOWN");
            }
        }

        private string EndingFailureLine(EndingCause cause)
        {
            switch (cause)
            {
                case EndingCause.SanityZero:
                    return L("O motorista fechou os olhos antes da ultima paragem.", "The driver closed his eyes before the last stop.");
                case EndingCause.RoadCrash:
                    return L("A estrada acabou antes da rota.", "The road ended before the route did.");
                case EndingCause.MissedStops:
                    return L("Duas paragens ficaram vazias. A rota nao perdoou.", "Two stops were left empty. The route did not forgive it.");
                case EndingCause.OncomingBusCrash:
                    return L("As luzes vinham na direcao errada.", "The lights came from the wrong direction.");
                case EndingCause.CloseNpcStare:
                    return L("Algumas caras so aparecem quando e tarde demais.", "Some faces only appear when it is already too late.");
                case EndingCause.ManualExit:
                    return L("A viagem foi interrompida antes do terminal.", "The trip ended before the terminal.");
                default:
                    return L("O terminal desapareceu no nevoeiro.", "The terminal disappeared into the fog.");
            }
        }

        private static string FormatTime(float seconds)
        {
            int wholeSeconds = Mathf.Max(0, Mathf.RoundToInt(seconds));
            return (wholeSeconds / 60).ToString("00") + ":" + (wholeSeconds % 60).ToString("00");
        }

        private string RouteEndFlashLabel(EndingId ending, EndingCause cause)
        {
            if (ending == EndingId.GoodTrip)
            {
                return L("CHEGASTE AO FIM DA ROTA", "YOU REACHED THE END OF THE ROUTE");
            }

            switch (cause)
            {
                case EndingCause.SanityZero:
                    return L("FECHA OS OLHOS", "CLOSE YOUR EYES");
                case EndingCause.CloseNpcStare:
                    return L("NAO OLHES", "DO NOT LOOK");
                case EndingCause.OncomingBusCrash:
                    return L("IMPACTO", "IMPACT");
                case EndingCause.MissedStops:
                    return L("A ROTA SEGUIU", "THE ROUTE MOVED ON");
                default:
                    return L("PERDESTE", "YOU LOST");
            }
        }

        private string RouteEndFlashSubtitle(EndingId ending, EndingCause cause)
        {
            if (ending == EndingId.GoodTrip)
            {
                return L("A ultima paragem ficou para tras.", "The last stop is behind you.");
            }

            switch (cause)
            {
                case EndingCause.MissedStops:
                    return L("Perdeste duas paragens.", "You missed two stops.");
                case EndingCause.CloseNpcStare:
                    return L("Olhaste tempo demais.", "You looked too long.");
                case EndingCause.OncomingBusCrash:
                    return L("Colisao na faixa contraria.", "Collision in the wrong lane.");
                case EndingCause.RoadCrash:
                    return L("Saiste da estrada.", "You left the road.");
                case EndingCause.SanityZero:
                    return L("A rota entrou em ti.", "The route got inside you.");
                default:
                    return L("A viagem terminou.", "The trip ended.");
            }
        }

        private void BuildMenuTitle(Transform parent, Action logoClick)
        {
            var texture = Resources.Load<Texture2D>("TerminalRoute/Art/MenuLogo");
            if (texture != null)
            {
                var logoObject = new GameObject("Terminal Route Logo");
                logoObject.transform.SetParent(parent, false);
                var rect = logoObject.AddComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.34f, 0.785f);
                rect.anchorMax = rect.anchorMin;
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(570f, 128f);
                rect.anchoredPosition = Vector2.zero;

                var image = logoObject.AddComponent<RawImage>();
                image.texture = texture;
                image.color = Color.white;
                image.raycastTarget = logoClick != null;
                if (uiPixelMaterial != null)
                {
                    image.material = uiPixelMaterial;
                }

                if (logoClick != null)
                {
                    var button = logoObject.AddComponent<Button>();
                    button.targetGraphic = image;
                    var colors = button.colors;
                    colors.normalColor = Color.white;
                    colors.highlightedColor = new Color(0.85f, 1f, 0.85f, 1f);
                    colors.pressedColor = new Color(0.62f, 0.82f, 0.62f, 1f);
                    colors.selectedColor = Color.white;
                    colors.disabledColor = new Color(1f, 1f, 1f, 0.65f);
                    button.colors = colors;
                    button.onClick.AddListener(() => logoClick());
                }

                return;
            }

            Text("TERMINAL", parent, new Vector2(0.34f, 0.810f), new Vector2(460f, 54f), 46, TextAnchor.MiddleCenter, new Color(0.30f, 0.95f, 0.30f));
            Text("ROUTE", parent, new Vector2(0.34f, 0.755f), new Vector2(320f, 54f), 46, TextAnchor.MiddleCenter, new Color(0.82f, 0.05f, 0.04f));
            Text("ROUTE", parent, new Vector2(0.342f, 0.751f), new Vector2(320f, 54f), 46, TextAnchor.MiddleCenter, new Color(0.28f, 0.01f, 0.01f, 0.72f));
            BloodDrip(parent, new Vector2(0.295f, 0.720f), 5f, 22f);
            BloodDrip(parent, new Vector2(0.340f, 0.713f), 6f, 30f);
            BloodDrip(parent, new Vector2(0.387f, 0.722f), 5f, 19f);
        }

        private void BloodDrip(Transform parent, Vector2 anchor, float width, float height)
        {
            var dripObject = new GameObject("Route Blood Drip");
            dripObject.transform.SetParent(parent, false);
            var rect = dripObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 1f);
            rect.sizeDelta = new Vector2(width, height);
            rect.anchoredPosition = Vector2.zero;
            var image = dripObject.AddComponent<Image>();
            image.color = new Color(0.74f, 0.02f, 0.025f, 0.88f);
            image.raycastTarget = false;
        }

        private GameObject Panel(string name, Transform parent, Color color)
        {
            var panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            var rect = panel.AddComponent<RectTransform>();
            Stretch(rect);
            var image = panel.AddComponent<Image>();
            image.color = color;
            return panel;
        }

        private RawImage TextureLayer(string name, Transform parent, string resourcePath)
        {
            var textureObject = new GameObject(name);
            textureObject.transform.SetParent(parent, false);
            var rect = textureObject.AddComponent<RectTransform>();
            Stretch(rect);

            var image = textureObject.AddComponent<RawImage>();
            image.texture = Resources.Load<Texture2D>(resourcePath);
            image.color = Color.white;
            if (uiPixelMaterial != null)
            {
                image.material = uiPixelMaterial;
            }

            image.raycastTarget = false;
            return image;
        }

        private void BuildVhsOverlay(Transform parent)
        {
            var shader = TerminalRouteShaderLibrary.VhsOverlay();
            if (shader == null)
            {
                return;
            }

            vhsOverlayMaterial = new Material(shader);
            vhsOverlayMaterial.SetFloat("_Intensity", 0.52f);
            vhsOverlayMaterial.SetFloat("_ScanlineAlpha", 0.055f);
            vhsOverlayMaterial.SetFloat("_NoiseAlpha", 0.010f);
            vhsOverlayMaterial.SetFloat("_VignetteAlpha", 0.14f);
            vhsOverlayMaterial.SetFloat("_LineCount", 430f);
            vhsOverlayMaterial.SetFloat("_TrackingSpeed", 0.10f);

            var overlayObject = new GameObject("VHS Filter Overlay");
            overlayObject.transform.SetParent(parent, false);
            var rect = overlayObject.AddComponent<RectTransform>();
            Stretch(rect);

            var overlay = overlayObject.AddComponent<RawImage>();
            overlay.texture = Texture2D.whiteTexture;
            overlay.color = Color.white;
            overlay.material = vhsOverlayMaterial;
            overlay.raycastTarget = false;
            overlayObject.transform.SetAsLastSibling();
        }

        private void BuildSanityBar(Transform parent)
        {
            HudBox("Sanity Bar Backing", parent, new Vector2(0.585f, 0.158f), new Vector2(450f, 18f), new Color(0.005f, 0.018f, 0.012f, 0.48f));

            var fillObject = new GameObject("Sanity Bar Fill");
            fillObject.transform.SetParent(parent, false);
            var rect = fillObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.585f, 0.158f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = new Vector2(0f, 0.5f);
            rect.sizeDelta = new Vector2(420f, 7f);
            rect.anchoredPosition = new Vector2(-210f, -3f);
            sanityBarFill = fillObject.AddComponent<Image>();
            sanityBarFill.color = new Color(0.22f, 0.92f, 0.26f, 0.94f);
            sanityBarFill.raycastTarget = false;

            sanityBarText = Text("", parent, new Vector2(0.405f, 0.159f), new Vector2(92f, 15f), 11, TextAnchor.MiddleRight, new Color(0.58f, 1.00f, 0.58f, 0.92f));
        }

        private void BuildIntroPanel(Transform parent)
        {
            introPanel = Panel("Dispatcher Intro", parent, new Color(0f, 0f, 0f, 0.97f));
            introBackdrop = introPanel.GetComponent<Image>();
            introTopLid = HudBox("Intro Top Eyelid", introPanel.transform, new Vector2(0.5f, 1.0f), new Vector2(1280f, 390f), Color.black);
            introTopLid.rectTransform.pivot = new Vector2(0.5f, 1f);
            introBottomLid = HudBox("Intro Bottom Eyelid", introPanel.transform, new Vector2(0.5f, 0.0f), new Vector2(1280f, 390f), Color.black);
            introBottomLid.rectTransform.pivot = new Vector2(0.5f, 0f);
            introBacking = HudBox("Dispatcher Backing", introPanel.transform, new Vector2(0.5f, 0.52f), new Vector2(900f, 320f), new Color(0.01f, 0.03f, 0.02f, 0.74f));
            introTitleText = Text("", introPanel.transform, new Vector2(0.5f, 0.68f), new Vector2(790f, 46f), 28, TextAnchor.MiddleCenter, new Color(0.34f, 1.00f, 0.40f));
            introBodyText = Text("", introPanel.transform, new Vector2(0.5f, 0.52f), new Vector2(820f, 178f), 21, TextAnchor.MiddleCenter, new Color(0.82f, 0.90f, 0.80f));
            introContinueText = Text("", introPanel.transform, new Vector2(0.5f, 0.34f), new Vector2(680f, 40f), 18, TextAnchor.MiddleCenter, new Color(0.54f, 1.00f, 0.54f));
            introBodyRect = introBodyText.rectTransform;
            introBodyBasePosition = introBodyRect.anchoredPosition;
            SetIntroEyeOpen(0f);
            introPanel.SetActive(false);
        }

        private void BuildPausePanel(Transform parent, Action resumeRun, Action returnToMenu)
        {
            pausePanel = Panel("Pause", parent, new Color(0f, 0f, 0f, 0.48f));
            HudBox("Pause Backing", pausePanel.transform, new Vector2(0.5f, 0.52f), new Vector2(620f, 360f), new Color(0.01f, 0.03f, 0.02f, 0.88f));
            HudBox("Pause Header Line", pausePanel.transform, new Vector2(0.5f, 0.685f), new Vector2(500f, 4f), new Color(0.20f, 0.95f, 0.20f, 0.52f));
            HudBox("Pause Footer Line", pausePanel.transform, new Vector2(0.5f, 0.335f), new Vector2(500f, 4f), new Color(0.20f, 0.95f, 0.20f, 0.32f));
            pauseTitleText = Text("", pausePanel.transform, new Vector2(0.5f, 0.65f), new Vector2(540f, 58f), 38, TextAnchor.MiddleCenter, new Color(0.34f, 1.00f, 0.40f));
            pauseBodyText = Text("", pausePanel.transform, new Vector2(0.5f, 0.565f), new Vector2(540f, 70f), 18, TextAnchor.MiddleCenter, new Color(0.72f, 0.86f, 0.72f));
            pauseResumeText = Button("", pausePanel.transform, new Vector2(0.5f, 0.455f), new Vector2(270f, 56f), resumeRun ?? (() => { }));
            pauseMenuText = Button("", pausePanel.transform, new Vector2(0.5f, 0.365f), new Vector2(330f, 54f), returnToMenu ?? (() => { }));
            pauseShortcutText = Text("", pausePanel.transform, new Vector2(0.5f, 0.285f), new Vector2(420f, 34f), 17, TextAnchor.MiddleCenter, new Color(0.55f, 0.62f, 0.58f));
            pausePanel.SetActive(false);
        }

        private Image HudBox(string name, Transform parent, Vector2 anchor, Vector2 size, Color color)
        {
            var boxObject = new GameObject(name);
            boxObject.transform.SetParent(parent, false);
            var rect = boxObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
            var image = boxObject.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static void ConfigureLeftFill(RectTransform rect, float width)
        {
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(-width * 0.5f, 0f);
        }

        private Text Text(string value, Transform parent, Vector2 anchor, Vector2 size, int fontSize, TextAnchor alignment, Color color)
        {
            var textObject = new GameObject("Text");
            textObject.transform.SetParent(parent, false);
            var rect = textObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;

            var text = textObject.AddComponent<Text>();
            text.text = value;
            text.font = font;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 12;
            text.resizeTextMaxSize = fontSize;
            return text;
        }

        private RectTransform SteeringSpoke(string name, Transform parent, float angle)
        {
            var spokeObject = new GameObject(name);
            spokeObject.transform.SetParent(parent, false);
            var rect = spokeObject.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.235f, 0.145f);
            rect.anchorMax = rect.anchorMin;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(138f, 10f);
            rect.anchoredPosition = Vector2.zero;
            rect.localRotation = Quaternion.Euler(0f, 0f, angle);

            var image = spokeObject.AddComponent<Image>();
            image.color = new Color(0.09f, 0.12f, 0.10f, 0.78f);
            if (cockpitPixelMaterial != null)
            {
                image.material = cockpitPixelMaterial;
            }

            image.raycastTarget = false;
            return rect;
        }

        private void UpdateNightmareButtonVisual()
        {
            if (menuNightmareText == null)
            {
                return;
            }

            var button = menuNightmareText.transform.parent.GetComponent<Button>();
            if (button != null)
            {
                button.interactable = true;
            }

            menuNightmareText.color = nightmareUnlocked
                ? new Color(0.30f, 0.95f, 0.30f)
                : new Color(0.36f, 0.50f, 0.38f);

            if (menuNightmareButtonImage != null)
            {
                menuNightmareButtonImage.color = nightmareUnlocked
                    ? new Color(0.015f, 0.055f, 0.025f, 0.94f)
                    : new Color(0.01f, 0.025f, 0.018f, 0.78f);
            }
        }

        private Text Button(string label, Transform parent, Vector2 anchor, Vector2 size, Action onClick)
        {
            return Button(label, parent, anchor, size, 24, onClick);
        }

        private Text SmallButton(string label, Transform parent, Vector2 anchor, Vector2 size, Action onClick)
        {
            return Button(label, parent, anchor, size, 16, onClick);
        }

        private Text Button(string label, Transform parent, Vector2 anchor, Vector2 size, int fontSize, Action onClick)
        {
            var buttonObject = new GameObject(label);
            buttonObject.transform.SetParent(parent, false);
            var rect = buttonObject.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
            var image = buttonObject.AddComponent<Image>();
            image.color = new Color(0.004f, 0.030f, 0.016f, 0.88f);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.70f, 1f, 0.74f, 1f);
            colors.pressedColor = new Color(0.42f, 0.70f, 0.44f, 1f);
            colors.selectedColor = new Color(0.76f, 1f, 0.78f, 1f);
            colors.disabledColor = new Color(0.55f, 0.62f, 0.56f, 0.70f);
            button.colors = colors;
            button.onClick.AddListener(() => onClick());
            return Text(label, buttonObject.transform, new Vector2(0.5f, 0.5f), size, fontSize, TextAnchor.MiddleCenter, new Color(0.38f, 1.00f, 0.42f));
        }

        private void AddOutline(Transform parent)
        {
            CubeLine(parent, new Vector2(0.5f, 0.885f), new Vector2(520f, 4f));
            CubeLine(parent, new Vector2(0.5f, 0.705f), new Vector2(520f, 4f));
            CubeLine(parent, new Vector2(0.32f, 0.795f), new Vector2(4f, 195f));
            CubeLine(parent, new Vector2(0.68f, 0.795f), new Vector2(4f, 195f));
        }

        private void CubeLine(Transform parent, Vector2 anchor, Vector2 size)
        {
            var line = new GameObject("Mirror Line");
            line.transform.SetParent(parent, false);
            var rect = line.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = Vector2.zero;
            var image = line.AddComponent<Image>();
            image.color = new Color(0.20f, 0.95f, 0.20f, 0.38f);
            image.raycastTarget = false;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void EnsureEventSystem()
        {
            var eventSystems = FindEventSystems();
            EventSystem primary = eventSystems.Length > 0 ? eventSystems[0] : null;

            if (primary == null)
            {
                var eventSystemObject = new GameObject("Terminal Route EventSystem");
                primary = eventSystemObject.AddComponent<EventSystem>();
                ConfigureInputModule(primary);
                UnityEngine.Object.DontDestroyOnLoad(eventSystemObject);
                return;
            }

            for (int i = 0; i < eventSystems.Length; i++)
            {
                ConfigureInputModule(eventSystems[i]);
                eventSystems[i].enabled = i == 0;
            }

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            DisableAllLegacyInputModules();
#endif
        }

        private static void ConfigureInputModule(EventSystem eventSystem)
        {
            if (eventSystem == null)
            {
                return;
            }

#if ENABLE_LEGACY_INPUT_MANAGER
            var legacyModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (legacyModule == null)
            {
                legacyModule = eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            }

            legacyModule.enabled = true;

#if ENABLE_INPUT_SYSTEM
            var inputSystemModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputSystemModule != null)
            {
                inputSystemModule.enabled = false;
            }
#endif
#elif ENABLE_INPUT_SYSTEM
            DisableLegacyInputModule(eventSystem.gameObject);

            var inputSystemModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputSystemModule == null)
            {
                inputSystemModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }

            inputSystemModule.enabled = true;
            if (inputSystemModule.actionsAsset == null)
            {
                inputSystemModule.AssignDefaultActions();
            }
#else
            if (eventSystem.GetComponent<StandaloneInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            }
#endif
        }

#if ENABLE_INPUT_SYSTEM
        private static void DisableAllLegacyInputModules()
        {
            var legacyModules = FindLegacyInputModules();
            for (int i = 0; i < legacyModules.Length; i++)
            {
                if (legacyModules[i] != null)
                {
                    legacyModules[i].enabled = false;
                }
            }
        }

        private static void DisableLegacyInputModule(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            var legacyModules = gameObject.GetComponents<StandaloneInputModule>();
            for (int i = 0; i < legacyModules.Length; i++)
            {
                legacyModules[i].enabled = false;
            }
        }

        private static StandaloneInputModule[] FindLegacyInputModules()
        {
#if UNITY_2023_1_OR_NEWER
            return UnityEngine.Object.FindObjectsByType<StandaloneInputModule>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
            return UnityEngine.Object.FindObjectsOfType<StandaloneInputModule>(true);
#endif
        }
#endif

        private static EventSystem[] FindEventSystems()
        {
#if UNITY_2023_1_OR_NEWER
            return UnityEngine.Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
#else
            return UnityEngine.Object.FindObjectsOfType<EventSystem>();
#endif
        }

        private static AudioListener FindAudioListener()
        {
#if UNITY_2023_1_OR_NEWER
            return UnityEngine.Object.FindFirstObjectByType<AudioListener>();
#else
            return UnityEngine.Object.FindObjectOfType<AudioListener>();
#endif
        }

        private static Material PixelMaterial(float resolution, float scanlines, float vignette, float aberration)
        {
            Shader shader = TerminalRouteShaderLibrary.PixelatedCamera();
            if (shader == null)
            {
                return null;
            }

            var material = new Material(shader);
            material.SetFloat("_PixelResolution", resolution);
            material.SetFloat("_ScanlineStrength", scanlines);
            material.SetFloat("_VignetteStrength", vignette);
            material.SetFloat("_Aberration", aberration);
            return material;
        }

    }
}
