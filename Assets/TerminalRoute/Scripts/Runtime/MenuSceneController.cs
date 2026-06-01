using UnityEngine;
using UnityEngine.SceneManagement;

namespace TerminalRoute.Runtime
{
    public sealed class MenuSceneController : MonoBehaviour
    {
        private TerminalRouteUi ui;
        private bool loadingRoute;

        private void Awake()
        {
            ui = new TerminalRouteUi();
            ui.Build(BeginRoute, QuitGame);
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

            if (Input.GetKeyDown(KeyCode.C))
            {
                ui.ShowCredits();
                return;
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                ui.ShowControls();
                return;
            }

            if (TerminalRouteInput.StartPressed())
            {
                BeginRoute();
            }
        }

        private void BeginRoute()
        {
            if (loadingRoute)
            {
                return;
            }

            loadingRoute = true;
            TerminalRouteSession.StartNewRun();
            SceneManager.LoadScene("Route", LoadSceneMode.Single);
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
}
