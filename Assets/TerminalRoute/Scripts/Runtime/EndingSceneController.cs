using TerminalRoute.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TerminalRoute.Runtime
{
    public sealed class EndingSceneController : MonoBehaviour
    {
        private void Awake()
        {
            var ui = new TerminalRouteUi();
            ui.Build(ReturnToMenu, QuitGame);
            var ending = TerminalRouteSession.Ending == EndingId.None ? EndingId.LongRoute : TerminalRouteSession.Ending;
            ui.ShowEnding(ending);

            var audioController = new TerminalRouteAudio(gameObject);
            audioController.PlayEndingTone();
        }

        private void Update()
        {
            if (TerminalRouteInput.FullscreenPressed())
            {
                TerminalRouteUi.ToggleFullscreen();
                return;
            }

            if (TerminalRouteInput.StartPressed())
            {
                ReturnToMenu();
            }
        }

        private void ReturnToMenu()
        {
            SceneManager.LoadScene("Menu");
        }

        private void QuitGame()
        {
            Application.Quit();
        }
    }
}
