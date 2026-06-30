using UnityEngine;
using UnityEngine.SceneManagement;
using TerminalRoute.Core;

namespace TerminalRoute.Runtime
{
    public sealed class MenuSceneController : MonoBehaviour
    {
        private TerminalRouteUi ui;
        private bool loadingRoute;
        private int logoClickCount;

        private void Awake()
        {
            ui = new TerminalRouteUi();
            ui.BuildMenu(BeginRoute, BeginNightmareRoute, QuitGame, HandleLogoClick);
            ui.SetNightmareUnlocked(TerminalRouteSession.IsNightmareUnlocked);
            ui.ShowMenu();

            var audioController = new TerminalRouteAudio(gameObject);
            audioController.PlayMenuDrone();
        }

        private void Update()
        {
            if (TerminalRouteInput.FullscreenPressed())
            {
                TerminalRouteUi.ToggleFullscreen();
                return;
            }

            if (ui.IsCreditsVisible || ui.IsControlsVisible)
            {
                if (TerminalRouteInput.CancelPressed())
                {
                    ui.ShowMenu();
                }

                return;
            }

            if (TerminalRouteInput.CreditsPressed())
            {
                ui.ShowCredits();
                return;
            }

            if (TerminalRouteInput.ControlsPressed())
            {
                ui.ShowControls();
                return;
            }

            if (TerminalRouteInput.NightmarePressed())
            {
                BeginNightmareRoute();
                return;
            }

            if (TerminalRouteInput.StartPressed())
            {
                BeginRoute();
            }
        }

        private void BeginRoute()
        {
            BeginRoute(GameMode.Route04);
        }

        private void BeginNightmareRoute()
        {
            if (!TerminalRouteSession.IsNightmareUnlocked)
            {
                ui.ShowNightmareLockedMessage();
                return;
            }

            BeginRoute(GameMode.Nightmare);
        }

        private void BeginRoute(GameMode mode)
        {
            if (loadingRoute)
            {
                return;
            }

            loadingRoute = true;
            TerminalRouteSession.StartNewRun(mode);
            SceneManager.LoadScene("Route", LoadSceneMode.Single);
        }

        private void HandleLogoClick()
        {
            if (TerminalRouteSession.IsNightmareUnlocked)
            {
                return;
            }

            logoClickCount++;
            if (logoClickCount >= 5)
            {
                TerminalRouteSession.UnlockNightmare();
                ui.SetNightmareUnlocked(true);
                ui.ShowNightmareUnlockedMessage();
            }
            else
            {
                ui.ShowNightmareUnlockProgress(logoClickCount);
            }
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
}
