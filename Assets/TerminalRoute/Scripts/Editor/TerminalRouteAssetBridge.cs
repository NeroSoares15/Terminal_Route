using UnityEditor;
using UnityEngine;

namespace TerminalRoute.EditorTools
{
    public static class TerminalRouteAssetBridge
    {
        private const string AssetPackRoot = "Assets/TerminalRoute/AssetPacks";
        private const string ResourceRoot = "Assets/TerminalRoute/Resources/TerminalRoute";

        [InitializeOnLoadMethod]
        private static void QueueEnsureAssetLibrary()
        {
            EditorApplication.delayCall += EnsureAssetLibrary;
        }

        [MenuItem("Terminal Route/Assets/Rebuild Demo Asset Library")]
        public static void RebuildAssetLibrary()
        {
            BuildAssetLibrary(true);
        }

        public static void EnsureAssetLibrary()
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                return;
            }

            bool hasRoadMaterial = AssetDatabase.LoadAssetAtPath<Material>(ResourcePath("Materials/RoadAsphalt.mat")) != null;
            bool hasRuntimeBus = AssetDatabase.LoadAssetAtPath<GameObject>(ResourcePath("Extracted/Bus.prefab")) != null;
            bool hasPassenger = AssetDatabase.LoadAssetAtPath<GameObject>(ResourcePath("Characters/Passenger01.fbx")) != null;

            if (!hasRoadMaterial || !hasRuntimeBus || !hasPassenger)
            {
                BuildAssetLibrary(false);
            }
        }

        private static void BuildAssetLibrary(bool force)
        {
            if (!AssetDatabase.IsValidFolder(AssetPackRoot))
            {
                Debug.LogWarning("Terminal Route asset bridge skipped. AssetPacks folder was not found.");
                return;
            }

            EnsureFolder(ResourceRoot);
            EnsureFolder(ResourcePath("Materials"));
            EnsureFolder(ResourcePath("Extracted"));
            EnsureFolder(ResourcePath("Characters"));
            DeleteGeneratedFolder(ResourcePath("Prefabs"));

            CreateMaterial("RoadAsphalt", "Bus_stop/Texture/Asphalt.jpg", new Color(0.22f, 0.22f, 0.24f), false);
            CreateMaterial("RoadConcrete", "Bus_stop/Texture/Concrete.png", new Color(0.38f, 0.37f, 0.34f), false);
            CreateMaterial("Grass", "Bus_stop/Texture/Grass.jpg", new Color(0.10f, 0.20f, 0.12f), false);
            CreateMaterial("Fence", "Bus_stop/Texture/Fence.png", new Color(0.20f, 0.20f, 0.18f), true);
            CreateMaterial("TreeBillboard", "Bus_stop/Texture/Tree_01.png", new Color(0.22f, 0.35f, 0.20f), true);
            CreateMaterial("BusInterior", "Bus_stop/Texture/Floor.jpg", new Color(0.08f, 0.09f, 0.13f), false);
            CreateMaterial("SeatVinyl", "Bus_stop/Texture/Seating.png", new Color(0.16f, 0.13f, 0.11f), false);
            CreateMaterial("WindowGlass", "Bus_stop/Texture/Glass.png", new Color(0.25f, 0.38f, 0.44f, 0.46f), true);
            CreateMaterial("BusPaint", "Bus_stop/Texture/Bus.png", new Color(0.08f, 0.10f, 0.16f), false);
            ConfigureTransparentTexture("Bus_stop/Texture/Tree_01.png");
            ConfigureTransparentTexture("Bus_stop/Texture/Tree_02.png");
            ConfigureTransparentTexture("Bus_stop/Texture/Tree_03.png");
            ConfigureTransparentTexture("Bus_stop/Texture/Tree.png");

            CopyRuntimePrefab("Bus", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Bus.prefab", force);
            CopyRuntimePrefab("BusStop", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Bus_stop.prefab", force);
            CopyRuntimePrefab("BusStopAlt", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Stop_Alt/Bus_stop_001.prefab", force);
            CopyRuntimePrefab("BusStopSign", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Bus_stop_sign.prefab", force);
            CopyRuntimePrefab("Fence", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Stop_Alt/Fence.prefab", force);
            CopyRuntimePrefab("House", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/House.prefab", force);
            CopyRuntimePrefab("Lamppost", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Lamppost.prefab", force);
            CopyRuntimePrefab("RoadSign", "Assets/TerminalRoute/ExtractedPrefabs/Roads/sign.prefab", force);
            CopyRuntimePrefab("Tree", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Tree.prefab", force);
            CopyRuntimePrefab("TreeAlt", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Tree_01.prefab", force);
            CopyRuntimePrefab("TreeTall", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Stop_Alt/Tree_001.prefab", force);
            CopyRuntimeModel("Passenger01", "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Male/Character_01.fbx", force);
            CopyRuntimeModel("Passenger02", "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Male/Character_04.fbx", force);
            CopyRuntimeModel("Passenger03", "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Male/Character_10.fbx", force);
            CopyRuntimeModel("Passenger04", "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Female/Character_Female_01.fbx", force);
            CopyRuntimeModel("Passenger05", "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Female/Character_29_Female.fbx", force);
            CopyRuntimeModel("Passenger06", "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Female/Character_31_Female.fbx", force);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Terminal Route demo asset library is ready.");
        }

        private static void CreateMaterial(string name, string textureRelativePath, Color color, bool transparent)
        {
            string materialPath = ResourcePath("Materials/" + name + ".mat");
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

            if (material == null)
            {
                material = new Material(FindRuntimeShader());
                AssetDatabase.CreateAsset(material, materialPath);
            }

            material.name = name;
            ApplyColor(material, color);

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(PackPath(textureRelativePath));
            if (texture != null)
            {
                material.mainTexture = texture;
                if (material.HasProperty("_BaseMap"))
                {
                    material.SetTexture("_BaseMap", texture);
                }
            }

            if (transparent)
            {
                material.SetFloat("_AlphaClip", 1f);
                material.SetFloat("_Cutoff", 0.22f);
                material.EnableKeyword("_ALPHATEST_ON");
                material.renderQueue = 2450;
            }

            EditorUtility.SetDirty(material);
        }

        private static void ConfigureTransparentTexture(string textureRelativePath)
        {
            var importer = AssetImporter.GetAtPath(PackPath(textureRelativePath)) as TextureImporter;
            if (importer == null)
            {
                return;
            }

            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
        }

        private static void CopyRuntimePrefab(string resourceName, string sourcePath, bool force)
        {
            string targetPath = ResourcePath("Extracted/" + resourceName + ".prefab");
            bool exists = AssetDatabase.LoadAssetAtPath<GameObject>(targetPath) != null;
            if (exists && !force)
            {
                return;
            }

            if (exists)
            {
                AssetDatabase.DeleteAsset(targetPath);
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath) == null)
            {
                return;
            }

            AssetDatabase.CopyAsset(sourcePath, targetPath);
        }

        private static void CopyRuntimeModel(string resourceName, string sourcePath, bool force)
        {
            string targetPath = ResourcePath("Characters/" + resourceName + ".fbx");
            bool exists = AssetDatabase.LoadAssetAtPath<GameObject>(targetPath) != null;
            if (exists && !force)
            {
                return;
            }

            if (exists)
            {
                AssetDatabase.DeleteAsset(targetPath);
            }

            if (AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath) == null)
            {
                return;
            }

            AssetDatabase.CopyAsset(sourcePath, targetPath);
        }

        private static Shader FindRuntimeShader()
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            return shader != null ? shader : Shader.Find("Standard");
        }

        private static void ApplyColor(Material material, Color color)
        {
            material.color = color;
            if (material.HasProperty("_BaseColor"))
            {
                material.SetColor("_BaseColor", color);
            }
        }

        private static string PackPath(string relativePath)
        {
            return AssetPackRoot + "/" + relativePath;
        }

        private static string ResourcePath(string relativePath)
        {
            return ResourceRoot + "/" + relativePath;
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

        private static void DeleteGeneratedFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.DeleteAsset(path);
            }
        }
    }
}
