using UnityEngine;
using UnityEngine.SceneManagement;

namespace TerminalRoute.Runtime
{
    public static class TerminalRouteBootstrap
    {
        private const string EditorPreviewRootName = "Terminal Route Editor Preview";
        private static bool redirectingToMenu;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterSceneHook()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            redirectingToMenu = false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeCurrentScene()
        {
            InitializeScene(SceneManager.GetActiveScene());
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            InitializeScene(scene);
        }

        private static void InitializeScene(Scene scene)
        {
            string sceneName = scene.name;
            redirectingToMenu = false;
            RemoveEditorPreviewRoot();

            if (sceneName == "Menu")
            {
                CreateController<MenuSceneController>("Terminal Route Menu");
                return;
            }

            if (sceneName == "Route")
            {
                CreateController<TerminalRouteGame>("Terminal Route Runtime");
                return;
            }

            if (sceneName == "Ending")
            {
                CreateController<EndingSceneController>("Terminal Route Ending");
                return;
            }

            if (redirectingToMenu)
            {
                return;
            }

            redirectingToMenu = true;
            SceneManager.LoadScene("Menu");
        }

        private static void RemoveEditorPreviewRoot()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            GameObject previewRoot = GameObject.Find(EditorPreviewRootName);
            if (previewRoot != null)
            {
                Object.Destroy(previewRoot);
            }
        }

        private static void CreateController<T>(string objectName) where T : Component
        {
            if (FindController<T>() != null)
            {
                return;
            }

            var gameObject = new GameObject(objectName);
            gameObject.AddComponent<T>();
        }

        private static T FindController<T>() where T : Component
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindFirstObjectByType<T>();
#else
            return Object.FindObjectOfType<T>();
#endif
        }
    }
}
