using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TerminalRoute.EditorTools
{
    public static class TerminalRouteAssetExtractor
    {
        private const string OutputRoot = "Assets/TerminalRoute/ExtractedPrefabs";

        private static readonly SourceModel[] Sources =
        {
            new SourceModel("BusStop_Stop", "Assets/TerminalRoute/AssetPacks/Bus_stop/Models/Stops/Stop.fbx"),
            new SourceModel("BusStop_Stop_Alt", "Assets/TerminalRoute/AssetPacks/Bus_stop/Models/Stops/Stop_01.fbx"),
            new SourceModel("BusStop_Props", "Assets/TerminalRoute/AssetPacks/Bus_stop/Models/Props/Props.fbx"),
            new SourceModel("Roads", "Assets/TerminalRoute/AssetPacks/Roads/Models/Roads.dae")
        };

        [MenuItem("Terminal Route/Assets/Extract Pack Children To Prefabs")]
        public static void ExtractAll()
        {
            EnsureFolder(OutputRoot);

            int prefabCount = 0;
            foreach (var source in Sources)
            {
                prefabCount += ExtractChildren(source);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Terminal Route extracted " + prefabCount + " child prefabs to " + OutputRoot);
        }

        private static int ExtractChildren(SourceModel source)
        {
            GameObject sourceAsset = AssetDatabase.LoadAssetAtPath<GameObject>(source.AssetPath);
            if (sourceAsset == null)
            {
                Debug.LogWarning("Terminal Route extractor could not find " + source.AssetPath);
                return 0;
            }

            string targetFolder = OutputRoot + "/" + source.Name;
            EnsureFolder(targetFolder);

            GameObject instance = PrefabUtility.InstantiatePrefab(sourceAsset) as GameObject;
            if (instance == null)
            {
                instance = Object.Instantiate(sourceAsset);
            }

            instance.name = source.Name + "_SourceInstance";
            instance.hideFlags = HideFlags.HideAndDontSave;

            var children = new List<Transform>();
            foreach (Transform child in instance.transform)
            {
                if (HasRenderer(child.gameObject))
                {
                    children.Add(child);
                }
            }

            int count = 0;
            if (children.Count == 0 && HasRenderer(instance))
            {
                if (SaveExtractedPrefab(source.Name, instance, targetFolder))
                {
                    count++;
                }
            }
            else
            {
                foreach (Transform child in children)
                {
                    if (SaveExtractedPrefab(child.name, child.gameObject, targetFolder))
                    {
                        count++;
                    }
                }
            }

            Object.DestroyImmediate(instance);
            return count;
        }

        private static bool SaveExtractedPrefab(string objectName, GameObject sourceObject, string targetFolder)
        {
            var wrapper = new GameObject(SanitizeName(objectName));
            GameObject clone = Object.Instantiate(sourceObject, wrapper.transform);
            clone.name = objectName;

            StripRuntimeComponents(wrapper);
            NormalizePivotToBottomCenter(wrapper);

            string path = AssetDatabase.GenerateUniqueAssetPath(targetFolder + "/" + SanitizeName(objectName) + ".prefab");
            PrefabUtility.SaveAsPrefabAsset(wrapper, path);
            Object.DestroyImmediate(wrapper);
            return true;
        }

        private static void NormalizePivotToBottomCenter(GameObject root)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                return;
            }

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            Vector3 offset = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
            foreach (Transform child in root.transform)
            {
                child.position -= offset;
            }
        }

        private static void StripRuntimeComponents(GameObject root)
        {
            foreach (var collider in root.GetComponentsInChildren<Collider>(true))
            {
                Object.DestroyImmediate(collider);
            }

            foreach (var body in root.GetComponentsInChildren<Rigidbody>(true))
            {
                Object.DestroyImmediate(body);
            }

            foreach (var animator in root.GetComponentsInChildren<Animator>(true))
            {
                animator.enabled = false;
            }

            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }
        }

        private static bool HasRenderer(GameObject gameObject)
        {
            return gameObject.GetComponentInChildren<Renderer>(true) != null;
        }

        private static string SanitizeName(string value)
        {
            var builder = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '_' || c == '-')
                {
                    builder.Append(c);
                }
                else if (c == ' ' || c == '.')
                {
                    builder.Append('_');
                }
            }

            return builder.Length == 0 ? "ExtractedAsset" : builder.ToString();
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[i]);
                }

                current = next;
            }
        }

        private readonly struct SourceModel
        {
            public SourceModel(string name, string assetPath)
            {
                Name = name;
                AssetPath = assetPath;
            }

            public string Name { get; }
            public string AssetPath { get; }
        }
    }
}
