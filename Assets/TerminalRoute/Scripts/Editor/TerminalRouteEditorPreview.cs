using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TerminalRoute.EditorTools
{
    [InitializeOnLoad]
    public static class TerminalRouteEditorPreview
    {
        private const string PreviewRootName = "Terminal Route Editor Preview";
        private const string BusPath = "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Bus.prefab";
        private const string BusStopPath = "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Bus_stop.prefab";
        private const string BusStopAltPath = "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Stop_Alt/Bus_stop_001.prefab";
        private const string BusStopSignPath = "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Bus_stop_sign.prefab";
        private const string HousePath = "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/House.prefab";
        private const string LamppostPath = "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Lamppost.prefab";
        private const string PreviewVersionMarkerName = "Terminal Route Preview v6";

        static TerminalRouteEditorPreview()
        {
            EditorApplication.delayCall += RefreshActiveScenePreview;
            EditorSceneManager.sceneOpened += (_, __) => RefreshActiveScenePreview();
            EditorSceneManager.activeSceneChangedInEditMode += (_, __) => RefreshActiveScenePreview();
        }

        [MenuItem("Terminal Route/Setup/Rebuild Route Editor Preview")]
        public static void RebuildActiveScenePreview()
        {
            BuildPreview(true);
        }

        public static void RefreshActiveScenePreview()
        {
            if (Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            BuildPreview(false);
        }

        private static void BuildPreview(bool force)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (!scene.IsValid() || (scene.name != "Route" && scene.name != "Menu" && scene.name != "Ending"))
            {
                return;
            }

            GameObject existing = GameObject.Find(PreviewRootName);
            if (existing != null)
            {
                if (!force && existing.transform.Find(PreviewVersionMarkerName) != null)
                {
                    return;
                }

                Object.DestroyImmediate(existing);
            }

            var root = new GameObject(PreviewRootName);
            TrySetEditorOnly(root);
            var versionMarker = new GameObject(PreviewVersionMarkerName);
            versionMarker.transform.SetParent(root.transform, false);
            TrySetEditorOnly(versionMarker);

            if (scene.name == "Menu")
            {
                BuildMenuPreview(root.transform);
                EditorSceneManager.MarkSceneDirty(scene);
                return;
            }

            if (scene.name == "Ending")
            {
                BuildEndingPreview(root.transform);
                EditorSceneManager.MarkSceneDirty(scene);
                return;
            }

            Material road = Material("Preview Road", new Color(0.08f, 0.085f, 0.095f));
            Material line = Material("Preview Lane", new Color(0.72f, 0.62f, 0.38f));
            Material grass = Material("Preview Grass", new Color(0.05f, 0.10f, 0.06f));
            Material metal = Material("Preview Metal", new Color(0.12f, 0.13f, 0.14f));
            Material glass = Material("Preview Glass", new Color(0.15f, 0.27f, 0.32f, 0.72f));
            Material bus = Material("Preview Bus", new Color(0.05f, 0.07f, 0.13f));
            Material green = Material("Preview Green Light", new Color(0.20f, 0.95f, 0.22f));
            Material amber = Material("Preview Amber Light", new Color(0.95f, 0.50f, 0.12f));
            Material treeBillboard = AssetDatabase.LoadAssetAtPath<Material>("Assets/TerminalRoute/Resources/TerminalRoute/Materials/TreeBillboard.mat");
            if (treeBillboard == null)
            {
                treeBillboard = Material("Preview Tree Billboard", new Color(0.22f, 0.35f, 0.20f));
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/TerminalRoute/AssetPacks/Bus_stop/Texture/Tree_01.png");
                treeBillboard.mainTexture = texture;
                EnableAlphaClip(treeBillboard, 0.22f);
            }

            for (int i = 0; i < 14; i++)
            {
                float z = i * 38f;
                Cube("Preview Road " + i, root.transform, new Vector3(0f, -0.08f, z), new Vector3(8f, 0.12f, 38f), road);
                Cube("Preview Left Shoulder " + i, root.transform, new Vector3(-7.2f, -0.12f, z), new Vector3(6.2f, 0.08f, 38f), grass);
                Cube("Preview Right Shoulder " + i, root.transform, new Vector3(7.2f, -0.12f, z), new Vector3(6.2f, 0.08f, 38f), grass);
                Cube("Preview Left Edge " + i, root.transform, new Vector3(-3.85f, 0.02f, z), new Vector3(0.08f, 0.035f, 33f), line);
                Cube("Preview Right Edge " + i, root.transform, new Vector3(3.85f, 0.02f, z), new Vector3(0.08f, 0.035f, 33f), line);
                for (int d = 0; d < 4; d++)
                {
                    Cube("Preview Center Dash " + i + "-" + d, root.transform, new Vector3(0f, 0.025f, z - 14f + d * 9f), new Vector3(0.08f, 0.035f, 3.6f), line);
                }

                BuildTree(root.transform, new Vector3(-8.8f, 0f, z - 9f), grass, metal, treeBillboard);
                BuildTree(root.transform, new Vector3(8.5f, 0f, z + 6f), grass, metal, treeBillboard);
            }

            BuildPreviewBus(root.transform, new Vector3(-9.4f, 0f, 18f), bus, glass, metal, green, amber);
            for (int i = 0; i < 8; i++)
            {
                BuildPreviewStop(root.transform, 75f + i * 82f, metal, glass, bus, green, amber);
            }
            PlaceExtracted("Preview Roadside House L", HousePath, root.transform, new Vector3(-17.5f, 0f, 74f), new Vector3(0f, 90f, 0f), 0.28f);
            PlaceExtracted("Preview Roadside House R", HousePath, root.transform, new Vector3(17.5f, 0f, 118f), new Vector3(0f, -90f, 0f), 0.28f);

            EditorSceneManager.MarkSceneDirty(scene);
        }

        private static void BuildMenuPreview(Transform parent)
        {
            Material black = Material("Preview Menu Black", new Color(0.006f, 0.008f, 0.010f));
            Material sign = Material("Preview Menu Sign", new Color(0.025f, 0.035f, 0.030f));
            Material green = Material("Preview Menu Green", new Color(0.20f, 0.95f, 0.22f));
            Material red = Material("Preview Menu Blood", new Color(0.72f, 0.02f, 0.02f));
            Material image = TextureMaterial("Preview Menu Generated Art", "Assets/Resources/TerminalRoute/Art/MenuBackground.png", new Color(0.13f, 0.17f, 0.18f));

            Cube("Menu Preview Backdrop", parent, new Vector3(0f, 3.0f, 6f), new Vector3(16f, 9f, 0.08f), image);
            Cube("Menu Preview Title Sign", parent, new Vector3(0f, 5.25f, 5.82f), new Vector3(6.8f, 1.05f, 0.12f), sign);
            TextMesh("TERMINAL", parent, new Vector3(-1.92f, 5.50f, 5.68f), 0.46f, green);
            TextMesh("ROUTE", parent, new Vector3(-1.05f, 5.08f, 5.67f), 0.48f, red);
            Cube("Preview Blood Drip A", parent, new Vector3(-0.56f, 4.78f, 5.60f), new Vector3(0.045f, 0.22f, 0.06f), red);
            Cube("Preview Blood Drip B", parent, new Vector3(0.22f, 4.72f, 5.60f), new Vector3(0.05f, 0.31f, 0.06f), red);
            TextMesh("NOVA VIAGEM", parent, new Vector3(-1.55f, 3.62f, 5.66f), 0.30f, green);
            TextMesh("CREDITOS", parent, new Vector3(-1.12f, 3.08f, 5.66f), 0.26f, green);
            TextMesh("SAIR", parent, new Vector3(-0.50f, 2.58f, 5.66f), 0.26f, green);
            BuildPreviewMonkey(parent, new Vector3(-4.2f, 2.1f, 5.60f), red, green);
        }

        private static void BuildEndingPreview(Transform parent)
        {
            Material black = Material("Preview Ending Black", new Color(0.006f, 0.008f, 0.010f));
            Material panel = Material("Preview Ending Panel", new Color(0.015f, 0.045f, 0.025f));
            Material green = Material("Preview Ending Green", new Color(0.20f, 0.95f, 0.22f));
            Material amber = Material("Preview Ending Amber", new Color(0.95f, 0.50f, 0.12f));

            Cube("Ending Preview Backdrop", parent, new Vector3(0f, 3.0f, 6f), new Vector3(16f, 9f, 0.08f), black);
            Cube("Ending Preview Result Panel", parent, new Vector3(0f, 3.35f, 5.82f), new Vector3(8.2f, 4.2f, 0.12f), panel);
            TextMesh("RESULTADO", parent, new Vector3(-1.85f, 4.80f, 5.65f), 0.46f, green);
            TextMesh("A LONGA ROTA", parent, new Vector3(-2.10f, 4.25f, 5.65f), 0.42f, amber);
            TextMesh("PARAGENS: 06/08", parent, new Vector3(-2.15f, 3.55f, 5.65f), 0.28f, green);
            TextMesh("FALHAS: 02/2", parent, new Vector3(-2.15f, 3.15f, 5.65f), 0.28f, green);
            TextMesh("SANIDADE: 18%", parent, new Vector3(-2.15f, 2.75f, 5.65f), 0.28f, green);
            TextMesh("CAUSA: DUAS PARAGENS PERDIDAS", parent, new Vector3(-2.15f, 2.35f, 5.65f), 0.24f, amber);
        }

        private static void BuildPreviewMonkey(Transform parent, Vector3 position, Material body, Material eyes)
        {
            Sphere("Preview Monkey Head", parent, position + new Vector3(0f, 0.36f, 0f), new Vector3(0.34f, 0.32f, 0.12f), body);
            Cube("Preview Monkey Body", parent, position, new Vector3(0.38f, 0.46f, 0.10f), body);
            Cube("Preview Monkey Eye L", parent, position + new Vector3(-0.09f, 0.42f, -0.08f), new Vector3(0.04f, 0.04f, 0.04f), eyes);
            Cube("Preview Monkey Eye R", parent, position + new Vector3(0.09f, 0.42f, -0.08f), new Vector3(0.04f, 0.04f, 0.04f), eyes);
        }

        private static void BuildPreviewBus(Transform parent, Vector3 position, Material bus, Material glass, Material metal, Material green, Material amber)
        {
            if (PlaceExtracted("Static Preview Bus", BusPath, parent, position, new Vector3(0f, 180f, 0f), 0.68f) != null)
            {
                return;
            }

            var root = new GameObject("Static Preview Bus");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = position;

            Cube("Bus Body", root.transform, new Vector3(0f, 1.05f, 0f), new Vector3(2.8f, 1.7f, 6.4f), bus);
            Cube("Bus Windshield", root.transform, new Vector3(0f, 1.48f, 3.24f), new Vector3(2.25f, 0.72f, 0.08f), glass);
            Cube("Bus Left Windows", root.transform, new Vector3(-1.44f, 1.45f, 0.2f), new Vector3(0.08f, 0.65f, 4.6f), glass);
            Cube("Bus Right Windows", root.transform, new Vector3(1.44f, 1.45f, 0.2f), new Vector3(0.08f, 0.65f, 4.6f), glass);
            Cube("Bus Front Sign", root.transform, new Vector3(0f, 2.05f, 3.30f), new Vector3(1.15f, 0.22f, 0.08f), green);
            Cube("Bus Headlight L", root.transform, new Vector3(-0.75f, 0.62f, 3.32f), new Vector3(0.35f, 0.18f, 0.08f), amber);
            Cube("Bus Headlight R", root.transform, new Vector3(0.75f, 0.62f, 3.32f), new Vector3(0.35f, 0.18f, 0.08f), amber);

            Cylinder("Wheel FL", root.transform, new Vector3(-1.48f, 0.32f, 2.2f), new Vector3(0.52f, 0.18f, 0.52f), Quaternion.Euler(90f, 0f, 0f), metal);
            Cylinder("Wheel FR", root.transform, new Vector3(1.48f, 0.32f, 2.2f), new Vector3(0.52f, 0.18f, 0.52f), Quaternion.Euler(90f, 0f, 0f), metal);
            Cylinder("Wheel RL", root.transform, new Vector3(-1.48f, 0.32f, -2.3f), new Vector3(0.52f, 0.18f, 0.52f), Quaternion.Euler(90f, 0f, 0f), metal);
            Cylinder("Wheel RR", root.transform, new Vector3(1.48f, 0.32f, -2.3f), new Vector3(0.52f, 0.18f, 0.52f), Quaternion.Euler(90f, 0f, 0f), metal);
        }

        private static void BuildPreviewStop(Transform parent, float z, Material metal, Material glass, Material roof, Material green, Material amber)
        {
            string stopPath = z < 100f ? BusStopPath : BusStopAltPath;
            if (PlaceExtracted("Preview Bus Stop Asset", stopPath, parent, new Vector3(6.7f, 0f, z), new Vector3(0f, -90f, 0f), 1f) != null)
            {
                PlaceExtracted("Preview Stop Sign Asset", BusStopSignPath, parent, new Vector3(4.8f, 0f, z - 2.25f), new Vector3(0f, -90f, 0f), 0.95f);
                PlaceExtracted("Preview Stop Lamp Asset", LamppostPath, parent, new Vector3(6.15f, 0f, z + 2.25f), new Vector3(0f, 90f, 0f), 1f);
                return;
            }

            Cube("Preview Stop Platform", parent, new Vector3(6.35f, 0.02f, z), new Vector3(3.4f, 0.12f, 5.2f), metal);
            Cube("Preview Stop Roof", parent, new Vector3(6.65f, 2.05f, z), new Vector3(3.1f, 0.18f, 3.7f), roof);
            Cube("Preview Stop Glass", parent, new Vector3(7.95f, 1.12f, z), new Vector3(0.10f, 1.75f, 3.1f), glass);
            Cube("Preview Stop Bench", parent, new Vector3(6.55f, 0.55f, z + 0.72f), new Vector3(1.65f, 0.18f, 0.42f), roof);
            Cube("Preview Stop Sign Pole", parent, new Vector3(4.9f, 1.05f, z - 1.8f), new Vector3(0.12f, 2.1f, 0.12f), amber);
            Cube("Preview Stop Sign", parent, new Vector3(4.9f, 2.2f, z - 1.8f), new Vector3(0.65f, 0.65f, 0.08f), green);
        }

        private static void BuildTree(Transform parent, Vector3 position, Material crown, Material trunk, Material treeBillboard)
        {
            float height = 2.6f;
            float width = 2.55f;
            float angle = Mathf.Abs(position.z * 19f) % 360f;
            TreeCard("Preview Tree Card A", parent, position + new Vector3(0f, height * 0.5f, 0f), new Vector2(width, height), angle, treeBillboard);
            TreeCard("Preview Tree Card B", parent, position + new Vector3(0f, height * 0.5f, 0f), new Vector2(width * 0.92f, height * 0.98f), angle + 88f, treeBillboard);
        }

        private static GameObject PlaceExtracted(string name, string path, Transform parent, Vector3 position, Vector3 eulerAngles, float uniformScale, Material overrideMaterial = null)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                return null;
            }

            GameObject gameObject = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
            if (gameObject == null)
            {
                gameObject = Object.Instantiate(prefab, parent);
            }

            gameObject.name = name;
            gameObject.transform.localPosition = position;
            gameObject.transform.localRotation = Quaternion.Euler(eulerAngles);
            gameObject.transform.localScale = Vector3.one * uniformScale;
            TrySetEditorOnly(gameObject);

            foreach (var collider in gameObject.GetComponentsInChildren<Collider>(true))
            {
                Object.DestroyImmediate(collider);
            }

            if (overrideMaterial != null)
            {
                foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>(true))
                {
                    renderer.sharedMaterial = overrideMaterial;
                }
            }

            return gameObject;
        }

        private static GameObject Cube(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            return Primitive(PrimitiveType.Cube, name, parent, position, scale, Quaternion.identity, material);
        }

        private static GameObject Sphere(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
        {
            return Primitive(PrimitiveType.Sphere, name, parent, position, scale, Quaternion.identity, material);
        }

        private static GameObject Cylinder(string name, Transform parent, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
        {
            return Primitive(PrimitiveType.Cylinder, name, parent, position, scale, rotation, material);
        }

        private static GameObject TreeCard(string name, Transform parent, Vector3 position, Vector2 size, float yaw, Material material)
        {
            return Primitive(PrimitiveType.Quad, name, parent, position, new Vector3(size.x, size.y, 1f), Quaternion.Euler(0f, yaw, 0f), material);
        }

        private static GameObject Primitive(PrimitiveType type, string name, Transform parent, Vector3 position, Vector3 scale, Quaternion rotation, Material material)
        {
            var gameObject = GameObject.CreatePrimitive(type);
            gameObject.name = name;
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localPosition = position;
            gameObject.transform.localRotation = rotation;
            gameObject.transform.localScale = scale;
            gameObject.GetComponent<Renderer>().sharedMaterial = material;

            var collider = gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                Object.DestroyImmediate(collider);
            }

            return gameObject;
        }

        private static Material Material(string name, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            var material = new Material(shader);
            material.name = name;
            material.color = color;
            return material;
        }

        private static Material TextureMaterial(string name, string texturePath, Color fallbackColor)
        {
            Shader shader = Shader.Find("Unlit/Texture");
            if (shader == null)
            {
                shader = Shader.Find("Universal Render Pipeline/Unlit");
            }

            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            var material = new Material(shader);
            material.name = name;
            material.color = fallbackColor;
            material.mainTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            return material;
        }

        private static TextMesh TextMesh(string value, Transform parent, Vector3 position, float characterSize, Material material)
        {
            var textObject = new GameObject("Preview Text");
            textObject.transform.SetParent(parent, false);
            textObject.transform.localPosition = position;
            textObject.transform.localRotation = Quaternion.identity;
            var text = textObject.AddComponent<TextMesh>();
            text.text = value;
            text.anchor = TextAnchor.MiddleLeft;
            text.alignment = TextAlignment.Left;
            text.characterSize = characterSize;
            text.fontSize = 72;
            text.color = material.color;
            TrySetEditorOnly(textObject);
            return text;
        }

        private static void EnableAlphaClip(Material material, float cutoff)
        {
            if (material.HasProperty("_AlphaClip"))
            {
                material.SetFloat("_AlphaClip", 1f);
            }

            if (material.HasProperty("_Cutoff"))
            {
                material.SetFloat("_Cutoff", cutoff);
            }

            material.EnableKeyword("_ALPHATEST_ON");
            material.renderQueue = 2450;
        }

        private static void TrySetEditorOnly(GameObject gameObject)
        {
            try
            {
                gameObject.tag = "EditorOnly";
            }
            catch
            {
                // Built-in in normal Unity projects; harmless if unavailable.
            }
        }
    }
}
