using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TerminalRoute.EditorTools
{
    [InitializeOnLoad]
    public static class TerminalRouteRendererRepair
    {
        private static readonly string[] RendererPaths =
        {
            "Assets/Settings/URP-HighFidelity-Renderer.asset",
            "Assets/Settings/URP-Balanced-Renderer.asset",
            "Assets/Settings/URP-Performant-Renderer.asset"
        };

        static TerminalRouteRendererRepair()
        {
            EditorApplication.delayCall += CleanRendererFeatureReferences;
        }

        [MenuItem("Terminal Route/Repair/Clean URP Renderer Features")]
        public static void CleanRendererFeatureReferences()
        {
            var changedPaths = new List<string>();

            foreach (string path in RendererPaths)
            {
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (asset == null)
                {
                    continue;
                }

                if (CleanRenderer(asset))
                {
                    EditorUtility.SetDirty(asset);
                    changedPaths.Add(path);
                }
            }

            if (changedPaths.Count == 0)
            {
                return;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.ForceReserializeAssets(changedPaths);
            foreach (string path in changedPaths)
            {
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }

            Debug.Log("Terminal Route cleaned stale URP renderer feature references.");
        }

        private static bool CleanRenderer(Object asset)
        {
            var serializedObject = new SerializedObject(asset);
            SerializedProperty features = serializedObject.FindProperty("m_RendererFeatures");
            SerializedProperty featureMap = serializedObject.FindProperty("m_RendererFeatureMap");
            if (features == null || !features.isArray)
            {
                return false;
            }

            bool changed = false;
            for (int i = features.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty feature = features.GetArrayElementAtIndex(i);
                if (feature.objectReferenceValue != null)
                {
                    continue;
                }

                features.DeleteArrayElementAtIndex(i);
                if (featureMap != null && featureMap.isArray && i < featureMap.arraySize)
                {
                    featureMap.DeleteArrayElementAtIndex(i);
                }

                changed = true;
            }

            if (featureMap != null && featureMap.isArray && featureMap.arraySize > features.arraySize)
            {
                while (featureMap.arraySize > features.arraySize)
                {
                    featureMap.DeleteArrayElementAtIndex(featureMap.arraySize - 1);
                }

                changed = true;
            }

            if (changed)
            {
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            return changed;
        }
    }
}
