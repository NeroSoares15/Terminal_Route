using System;
using TerminalRoute.Core;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private static TerminalRouteLanguage language = TerminalRouteLanguage.Portuguese;

        private Font font;
        private GameObject menuPanel;
        private GameObject hudPanel;
        private GameObject endingPanel;
        private GameObject creditsPanel;
        private GameObject controlsPanel;
        private GameObject mirrorPanel;
        private GameObject routeEndFlashPanel;
        private RawImage cockpitOverlay;
        private RawImage endingArtwork;
        private Material cockpitPixelMaterial;
        private Material uiPixelMaterial;
        private Material vhsOverlayMaterial;
        private Image sanityOverlay;
        private Image lightsOutOverlay;
        private Text clockText;
        private Text routeText;
        private Text passengerText;
        private Text speedText;
        private Text promptText;
        private Text eventText;
        private Image eventBacking;
        private Text endingTitleText;
        private Text endingBodyText;
        private Text menuSubtitleText;
        private Text menuStartText;
        private Text menuControlsText;
        private Text menuCreditsText;
        private Text menuQuitText;
        private Text menuFullscreenText;
        private Text menuLanguageText;
        private Text menuShortcutText;
        private Text doorButtonText;
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
        private Text routeEndFlashText;
        private RectTransform wheelSpokeA;
        private RectTransform wheelSpokeB;
        private bool lastDoorOpen;
        private EndingId lastFlashEnding = EndingId.None;
        private EndingCause lastFlashCause = EndingCause.None;

        public void Build(Action startRun, Action quitGame)
        {
            Build(startRun, quitGame, startRun, null);
        }

        public void Build(Action startRun, Action quitGame, Action endingReturn)
        {
            Build(startRun, quitGame, endingReturn, null);
        }

        public void Build(Action startRun, Action quitGame, Action endingReturn, Action toggleDoors)
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
            Panel("Menu Shade", menuPanel.transform, new Color(0f, 0f, 0f, 0.30f));
            HudBox("Title Sign Backing", menuPanel.transform, new Vector2(0.50f, 0.78f), new Vector2(660f, 118f), new Color(0.025f, 0.035f, 0.03f, 0.72f));
            BuildMenuTitle(menuPanel.transform);
            menuSubtitleText = Text("", menuPanel.transform, new Vector2(0.5f, 0.62f), new Vector2(850f, 45f), 23, TextAnchor.MiddleCenter, new Color(0.83f, 0.88f, 0.82f));
            menuStartText = Button("", menuPanel.transform, new Vector2(0.5f, 0.48f), new Vector2(300f, 58f), startRun);
            menuControlsText = Button("", menuPanel.transform, new Vector2(0.5f, 0.40f), new Vector2(265f, 52f), ShowControls);
            menuCreditsText = Button("", menuPanel.transform, new Vector2(0.5f, 0.32f), new Vector2(250f, 52f), ShowCredits);
            menuQuitText = Button("", menuPanel.transform, new Vector2(0.5f, 0.25f), new Vector2(205f, 50f), quitGame);
            menuLanguageText = SmallButton("", menuPanel.transform, new Vector2(0.12f, 0.08f), new Vector2(180f, 34f), ToggleLanguage);
            menuFullscreenText = SmallButton("", menuPanel.transform, new Vector2(0.88f, 0.08f), new Vector2(170f, 34f), ToggleFullscreen);
            menuShortcutText = Text("", menuPanel.transform, new Vector2(0.5f, 0.16f), new Vector2(780f, 34f), 18, TextAnchor.MiddleCenter, new Color(0.60f, 0.70f, 0.63f));

            hudPanel = Panel("HUD", canvasObject.transform, new Color(0f, 0f, 0f, 0f));
            cockpitOverlay = TextureLayer("Cockpit Artwork", hudPanel.transform, "TerminalRoute/Art/CockpitOverlay");
            cockpitPixelMaterial = PixelMaterial(224, 0.08f, 0.12f, 0.001f);
            if (cockpitPixelMaterial != null)
            {
                cockpitOverlay.material = cockpitPixelMaterial;
            }

            HudBox("Clock Backing", hudPanel.transform, new Vector2(0.082f, 0.94f), new Vector2(170f, 40f), new Color(0.01f, 0.03f, 0.02f, 0.62f));
            HudBox("Route Backing", hudPanel.transform, new Vector2(0.5f, 0.94f), new Vector2(410f, 34f), new Color(0.01f, 0.03f, 0.02f, 0.50f));
            HudBox("Passenger Backing", hudPanel.transform, new Vector2(0.905f, 0.94f), new Vector2(176f, 40f), new Color(0.01f, 0.03f, 0.02f, 0.62f));
            eventBacking = HudBox("Event Backing", hudPanel.transform, new Vector2(0.5f, 0.84f), new Vector2(560f, 38f), new Color(0.03f, 0.02f, 0.01f, 0.54f));
            HudBox("Prompt Backing", hudPanel.transform, new Vector2(0.5f, 0.06f), new Vector2(410f, 30f), new Color(0.01f, 0.03f, 0.02f, 0.56f));
            HudBox("Speed Backing", hudPanel.transform, new Vector2(0.91f, 0.08f), new Vector2(154f, 38f), new Color(0.03f, 0.02f, 0.01f, 0.58f));
            wheelSpokeA = SteeringSpoke("Wheel Spoke A", hudPanel.transform, 0f);
            wheelSpokeB = SteeringSpoke("Wheel Spoke B", hudPanel.transform, 90f);
            clockText = Text("00:55", hudPanel.transform, new Vector2(0.08f, 0.94f), new Vector2(150f, 34f), 24, TextAnchor.MiddleLeft, new Color(0.34f, 1.00f, 0.40f));
            routeText = Text("ROTA 04  //  PARAGEM 00/08  //  FALHAS 0/2", hudPanel.transform, new Vector2(0.5f, 0.94f), new Vector2(390f, 30f), 16, TextAnchor.MiddleCenter, new Color(0.62f, 0.80f, 0.64f));
            passengerText = Text("22", hudPanel.transform, new Vector2(0.91f, 0.94f), new Vector2(150f, 34f), 24, TextAnchor.MiddleRight, new Color(0.34f, 1.00f, 0.40f));
            eventText = Text("", hudPanel.transform, new Vector2(0.5f, 0.84f), new Vector2(535f, 34f), 19, TextAnchor.MiddleCenter, new Color(1.00f, 0.60f, 0.16f));
            speedText = Text("42", hudPanel.transform, new Vector2(0.91f, 0.08f), new Vector2(135f, 34f), 21, TextAnchor.MiddleRight, new Color(0.98f, 0.62f, 0.18f));
            promptText = Text("A/D  DIRECAO    F  ESPELHO", hudPanel.transform, new Vector2(0.5f, 0.06f), new Vector2(390f, 28f), 15, TextAnchor.MiddleCenter, new Color(0.54f, 1.00f, 0.54f));
            if (toggleDoors != null)
            {
                doorButtonText = SmallButton("", hudPanel.transform, new Vector2(0.12f, 0.08f), new Vector2(180f, 34f), toggleDoors);
            }

            mirrorPanel = Panel("Mirror Overlay", canvasObject.transform, new Color(0f, 0f, 0f, 0f));
            AddOutline(mirrorPanel.transform);
            HudBox("Mirror Label Backing", mirrorPanel.transform, new Vector2(0.5f, 0.91f), new Vector2(230f, 28f), new Color(0.01f, 0.04f, 0.02f, 0.46f));
            mirrorLabelText = Text("", mirrorPanel.transform, new Vector2(0.5f, 0.91f), new Vector2(220f, 26f), 15, TextAnchor.MiddleCenter, new Color(0.34f, 1.00f, 0.40f));

            endingPanel = Panel("Ending", canvasObject.transform, Color.black);
            endingArtwork = TextureLayer("Ending Artwork", endingPanel.transform, "TerminalRoute/Art/EndingLong");
            Panel("Ending Shade", endingPanel.transform, new Color(0f, 0f, 0f, 0.68f));
            HudBox("Result Backing", endingPanel.transform, new Vector2(0.5f, 0.50f), new Vector2(780f, 360f), new Color(0.01f, 0.03f, 0.02f, 0.72f));
            endingTitleText = Text("", endingPanel.transform, new Vector2(0.5f, 0.64f), new Vector2(850f, 88f), 52, TextAnchor.MiddleCenter, new Color(0.30f, 0.95f, 0.30f));
            endingBodyText = Text("", endingPanel.transform, new Vector2(0.5f, 0.48f), new Vector2(900f, 210f), 24, TextAnchor.MiddleCenter, new Color(0.84f, 0.84f, 0.76f));
            endingReturnText = Button("", endingPanel.transform, new Vector2(0.5f, 0.34f), new Vector2(350f, 60f), endingReturn);
            endingShortcutText = Text("", endingPanel.transform, new Vector2(0.5f, 0.25f), new Vector2(620f, 42f), 18, TextAnchor.MiddleCenter, new Color(0.55f, 0.62f, 0.58f));

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
            routeEndFlashPanel = Panel("Route End Flash", canvasObject.transform, new Color(0f, 0f, 0f, 0.88f));
            routeEndFlashText = Text("", routeEndFlashPanel.transform, new Vector2(0.5f, 0.52f), new Vector2(980f, 220f), 46, TextAnchor.MiddleCenter, new Color(0.95f, 0.20f, 0.16f));
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
            routeEndFlashPanel.SetActive(false);
            mirrorPanel.SetActive(false);
            SetVisualDistortion(0f);
            SetLightsOut(0f);
        }

        public void ShowHud()
        {
            menuPanel.SetActive(false);
            hudPanel.SetActive(true);
            endingPanel.SetActive(false);
            creditsPanel.SetActive(false);
            controlsPanel.SetActive(false);
            routeEndFlashPanel.SetActive(false);
            if (cockpitOverlay != null)
            {
                cockpitOverlay.enabled = true;
            }
        }

        public void ShowEnding(EndingId ending)
        {
            ShowEnding(ending, TerminalRouteSession.EndingCause, TerminalRouteSession.StopsReached, TerminalRouteSession.MissedStops, TerminalRouteSession.FinalSanity, TerminalRouteSession.ElapsedRouteTime);
        }

        public void ShowEnding(EndingId ending, EndingCause cause, int stopsReached, int missedStops, float finalSanity, float elapsedTime)
        {
            ApplyLanguage();
            menuPanel.SetActive(false);
            hudPanel.SetActive(false);
            endingPanel.SetActive(true);
            creditsPanel.SetActive(false);
            controlsPanel.SetActive(false);
            routeEndFlashPanel.SetActive(false);
            mirrorPanel.SetActive(false);
            SetVisualDistortion(0f);
            SetLightsOut(0f);

            if (ending == EndingId.GoodTrip)
            {
                endingArtwork.texture = Resources.Load<Texture2D>("TerminalRoute/Art/EndingGood");
                endingTitleText.text = IsEnglish ? "GOOD TRIP" : "BOA VIAGEM";
            }
            else
            {
                endingArtwork.texture = Resources.Load<Texture2D>("TerminalRoute/Art/EndingLong");
                endingTitleText.text = IsEnglish ? "THE LONG ROUTE" : "A LONGA ROTA";
            }

            endingBodyText.text =
                L("RESULTADO", "RESULT") + "\n" +
                L("PARAGENS: ", "STOPS: ") + Mathf.Clamp(stopsReached, 0, GameState.FinalStopCount).ToString("00") + "/" + GameState.FinalStopCount.ToString("00") + "\n" +
                L("FALHAS: ", "MISSES: ") + Mathf.Clamp(missedStops, 0, GameState.MaxMissedStops) + "/" + GameState.MaxMissedStops + "\n" +
                L("SANIDADE: ", "SANITY: ") + Mathf.RoundToInt(finalSanity).ToString("00") + "%\n" +
                L("TEMPO: ", "TIME: ") + FormatTime(elapsedTime) + "\n" +
                L("CAUSA: ", "CAUSE: ") + CauseLabel(cause);
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
            routeEndFlashPanel.SetActive(false);
            mirrorPanel.SetActive(false);
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
            routeEndFlashPanel.SetActive(false);
            mirrorPanel.SetActive(false);
            SetVisualDistortion(0f);
            SetLightsOut(0f);
        }

        public void ShowRouteEndFlash(EndingId ending, EndingCause cause)
        {
            lastFlashEnding = ending;
            lastFlashCause = cause;
            menuPanel.SetActive(false);
            hudPanel.SetActive(true);
            endingPanel.SetActive(false);
            creditsPanel.SetActive(false);
            controlsPanel.SetActive(false);
            mirrorPanel.SetActive(false);
            routeEndFlashPanel.SetActive(true);
            routeEndFlashText.text = RouteEndFlashLabel(ending, cause);
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
                menuSubtitleText.text = L("Mantem o autocarro na estrada. Chega a tempo.", "Keep the bus on the road. Arrive on time.");
                menuStartText.text = L("NOVA VIAGEM", "NEW TRIP");
                menuControlsText.text = L("CONTROLOS", "CONTROLS");
                menuCreditsText.text = L("CREDITOS", "CREDITS");
                menuQuitText.text = L("SAIR", "EXIT");
                menuFullscreenText.text = L("TELA CHEIA", "FULLSCREEN");
                menuLanguageText.text = L("IDIOMA: PT", "LANGUAGE: EN");
                menuShortcutText.text = L("ENTER  //  INICIAR      T  //  CONTROLOS      C  //  CREDITOS", "ENTER  //  START      T  //  CONTROLS      C  //  CREDITS");
            }

            if (doorButtonText != null)
            {
                doorButtonText.text = lastDoorOpen ? L("FECHAR PORTAS", "CLOSE DOORS") : L("ABRIR PORTAS", "OPEN DOORS");
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
                    "Terminal Route\n\nEquipa: Nero Soares & Paulo Monteiro\nDirecao, programacao e design: Grupo 4\nPrototipo tecnico: Unity 2022.3 LTS + URP\n\nAssets externos: Elbolilloduro / itch.io\nCharacters PSX, Bus Stop, Roads Procedural\n\nArte de menu/cockpit/finais: gerada para este prototipo\nAudio: sintetizado em runtime\nURLs e licencas: ASSET_CREDITS.md",
                    "Terminal Route\n\nTeam: Nero Soares & Paulo Monteiro\nDirection, programming and design: Group 4\nTechnical prototype: Unity 2022.3 LTS + URP\n\nExternal assets: Elbolilloduro / itch.io\nCharacters PSX, Bus Stop, Roads Procedural\n\nMenu/cockpit/ending art: generated for this prototype\nAudio: synthesized at runtime\nURLs and licenses: ASSET_CREDITS.md");
                creditsReturnText.text = L("VOLTAR", "BACK");
                creditsShortcutText.text = L("ESC  //  VOLTAR", "ESC  //  BACK");
            }

            if (controlsTitleText != null)
            {
                controlsTitleText.text = L("CONTROLOS", "CONTROLS");
                controlsBodyText.text = L(
                    "A / D ou setas  -  direcao\nF  -  olhar para o espelho\nE  -  abrir / fechar portas\n\nEncosta a direita quando a paragem brilhar.\nPara dentro da zona verde. Falhar duas paragens termina a viagem.\nSe alguem aparecer perto no espelho, olha para a frente.",
                    "A / D or arrows  -  steer\nF  -  look in the mirror\nE  -  open / close doors\n\nMove right when the stop glows.\nStop inside the green zone. Missing two stops ends the trip.\nIf someone appears close in the mirror, look forward.");
                controlsReturnText.text = L("VOLTAR", "BACK");
                controlsShortcutText.text = L("ESC  //  VOLTAR", "ESC  //  BACK");
            }

            if (routeEndFlashPanel != null && routeEndFlashPanel.activeSelf)
            {
                routeEndFlashText.text = RouteEndFlashLabel(lastFlashEnding, lastFlashCause);
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
            routeText.text = L("ROTA 04  //  PARAGEM ", "ROUTE 04  //  STOP ") + Mathf.Clamp(state.StopsReached, 0, GameState.FinalStopCount).ToString("00") + "/08  //  " + L("FALHAS ", "MISSES ") + Mathf.Clamp(state.MissedStops, 0, GameState.MaxMissedStops) + "/2";
            passengerText.text = PassengerDisplayText(state, activeEpisode, mirrorActive);
            speedText.text = Mathf.RoundToInt(speed).ToString("00") + " KM/H";
            promptText.text = mirrorActive ? L("F  VOLTAR", "F  BACK") : L("A/D  DIRECAO    F  ESPELHO    E  PORTAS", "A/D  STEER    F  MIRROR    E  DOORS");
            if (doorButtonText != null)
            {
                doorButtonText.text = doorsOpen ? L("FECHAR PORTAS", "CLOSE DOORS") : L("ABRIR PORTAS", "OPEN DOORS");
            }

            eventText.text = GetPriorityHudMessage(state, activeEpisode, roadDanger, routeAlert, mirrorAlert, doorsOpen);
            eventText.enabled = eventText.text.Length > 0;
            eventBacking.enabled = eventText.enabled;
            SetSteering(steering);
        }

        private void SetSteering(float steering)
        {
            float angle = Mathf.Clamp(steering, -1f, 1f) * -58f;
            wheelSpokeA.localRotation = Quaternion.Euler(0f, 0f, angle);
            wheelSpokeB.localRotation = Quaternion.Euler(0f, 0f, angle + 90f);
        }

        private string GetCabinMessage(GamePhase phase, EpisodeType activeEpisode, bool doorsOpen)
        {
            if (phase == GamePhase.Stopped)
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

            return GetCabinMessage(state.Phase, activeEpisode, doorsOpen);
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
                case EndingCause.MissedStops:
                    return L("PERDESTE DUAS PARAGENS", "YOU MISSED TWO STOPS");
                case EndingCause.CloseNpcStare:
                    return L("OLHASTE TEMPO DEMAIS", "YOU LOOKED TOO LONG");
                case EndingCause.OncomingBusCrash:
                    return L("COLISAO NA FAIXA CONTRARIA", "COLLISION IN THE WRONG LANE");
                case EndingCause.RoadCrash:
                    return L("SAISTE DA ESTRADA", "YOU LEFT THE ROAD");
                case EndingCause.SanityZero:
                    return L("A ROTA ENTROU EM TI", "THE ROUTE GOT INSIDE YOU");
                default:
                    return L("A VIAGEM TERMINOU", "THE TRIP ENDED");
            }
        }

        private void BuildMenuTitle(Transform parent)
        {
            Text("TERMINAL", parent, new Vector2(0.5f, 0.810f), new Vector2(600f, 54f), 50, TextAnchor.MiddleCenter, new Color(0.30f, 0.95f, 0.30f));
            Text("ROUTE", parent, new Vector2(0.5f, 0.755f), new Vector2(360f, 54f), 50, TextAnchor.MiddleCenter, new Color(0.82f, 0.05f, 0.04f));
            Text("ROUTE", parent, new Vector2(0.502f, 0.751f), new Vector2(360f, 54f), 50, TextAnchor.MiddleCenter, new Color(0.28f, 0.01f, 0.01f, 0.72f));
            BloodDrip(parent, new Vector2(0.445f, 0.720f), 5f, 22f);
            BloodDrip(parent, new Vector2(0.505f, 0.713f), 6f, 30f);
            BloodDrip(parent, new Vector2(0.562f, 0.722f), 5f, 19f);
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
            image.raycastTarget = false;
            return rect;
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
            image.color = new Color(0.015f, 0.055f, 0.025f, 0.94f);
            var button = buttonObject.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = new Color(0.92f, 1f, 0.92f, 1f);
            colors.highlightedColor = new Color(0.24f, 0.72f, 0.29f, 1f);
            colors.pressedColor = new Color(0.14f, 0.48f, 0.18f, 1f);
            button.colors = colors;
            button.onClick.AddListener(() => onClick());
            return Text(label, buttonObject.transform, new Vector2(0.5f, 0.5f), size, fontSize, TextAnchor.MiddleCenter, new Color(0.30f, 0.95f, 0.30f));
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
                eventSystemObject.AddComponent<EventSystem>();
                eventSystemObject.AddComponent<StandaloneInputModule>();
                UnityEngine.Object.DontDestroyOnLoad(eventSystemObject);
                return;
            }

            if (primary.GetComponent<StandaloneInputModule>() == null)
            {
                primary.gameObject.AddComponent<StandaloneInputModule>();
            }
        }

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
