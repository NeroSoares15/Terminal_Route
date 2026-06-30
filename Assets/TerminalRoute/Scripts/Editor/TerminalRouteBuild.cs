using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace TerminalRoute.EditorTools
{
    public static class TerminalRouteBuild
    {
        private static readonly string[] ScenePaths =
        {
            "Assets/Scenes/Menu.unity",
            "Assets/Scenes/Route.unity",
            "Assets/Scenes/Ending.unity"
        };

        [MenuItem("Terminal Route/Build/Windows MVP")]
        public static void BuildWindows()
        {
            TerminalRouteAssetBridge.EnsureAssetLibrary();
            EnsureBuildSettings();
            Directory.CreateDirectory("Builds/Windows");

            var options = new BuildPlayerOptions
            {
                scenes = ScenePaths,
                locationPathName = "Builds/Windows/TerminalRoute.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            Report(BuildPipeline.BuildPlayer(options));
        }

        [MenuItem("Terminal Route/Build/All MVP Builds")]
        public static void BuildAll()
        {
            BuildWindows();
            BuildWebGL();
        }

        [MenuItem("Terminal Route/Build/WebGL MVP")]
        public static void BuildWebGL()
        {
            TerminalRouteAssetBridge.EnsureAssetLibrary();
            EnsureBuildSettings();
            ApplyWebGLSettings();
            Directory.CreateDirectory("Builds/WebGL");

            var options = new BuildPlayerOptions
            {
                scenes = ScenePaths,
                locationPathName = "Builds/WebGL",
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            Report(BuildPipeline.BuildPlayer(options));
        }

        [MenuItem("Terminal Route/Setup/Apply MVP Player Settings")]
        public static void ApplyPlayerSettings()
        {
            TerminalRouteAssetBridge.EnsureAssetLibrary();
            PlayerSettings.productName = "Terminal Route";
            PlayerSettings.companyName = "Nero Soares & Paulo Monteiro";
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            ApplyWebGLSettings();
            EnsureBuildSettings();
            Debug.Log("Terminal Route MVP settings applied.");
        }

        private static void ApplyWebGLSettings()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.allowDebugging = false;
            EditorUserBuildSettings.connectProfiler = false;

            PlayerSettings.defaultWebScreenWidth = 1280;
            PlayerSettings.defaultWebScreenHeight = 720;
            PlayerSettings.WebGL.template = "PROJECT:TerminalRoute";
            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.dataCaching = true;
            PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.ExplicitlyThrownExceptionsOnly;
            PlayerSettings.WebGL.memorySize = 512;
            PlayerSettings.WebGL.threadsSupport = false;
            PlayerSettings.SetManagedStrippingLevel(NamedBuildTarget.WebGL, ManagedStrippingLevel.Low);
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.WebGL, ScriptingImplementation.IL2CPP);
        }

        private static void EnsureBuildSettings()
        {
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(ScenePaths[0], true),
                new EditorBuildSettingsScene(ScenePaths[1], true),
                new EditorBuildSettingsScene(ScenePaths[2], true)
            };
        }

        private static void Report(BuildReport report)
        {
            if (report.summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Terminal Route build succeeded: " + report.summary.outputPath);
            }
            else
            {
                Debug.LogError("Terminal Route build failed: " + report.summary.result);
                if (Application.isBatchMode)
                {
                    EditorApplication.Exit(1);
                }
            }
        }
    }
}
