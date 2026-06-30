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
        private const string GaragePath = "Assets/Resources/TerminalRoute/World/Buildings/Garage/model.fbx";
        private const string Panelak1Path = "Assets/Resources/TerminalRoute/World/Buildings/Panelak1/Panelak 1.fbx";
        private const string Panelak2Path = "Assets/Resources/TerminalRoute/World/Buildings/Panelak2/model.fbx";
        private const string Car01Path = "Assets/Resources/TerminalRoute/World/Cars/Car01/Car.obj";
        private const string Car03Path = "Assets/Resources/TerminalRoute/World/Cars/Car03/Car3.obj";
        private const string Car05Path = "Assets/Resources/TerminalRoute/World/Cars/Car05/Car5.obj";
        private const string Car06Path = "Assets/Resources/TerminalRoute/World/Cars/Car06/Car6.obj";
        private const string Car08Path = "Assets/Resources/TerminalRoute/World/Cars/Car08/Car8.obj";
        private const string Passenger01Path = "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Male/Character_01.fbx";
        private const string Passenger02Path = "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Male/Character_04.fbx";
        private const string Passenger03Path = "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Female/Character_Female_01.fbx";
        private const string PreviewVersionMarkerName = "Terminal Route Preview v8";

        private static readonly string[] BuildingPaths = { GaragePath, Panelak1Path, Panelak2Path };
        private static readonly string[] CarPaths = { Car01Path, Car03Path, Car05Path, Car06Path, Car08Path };
        private static readonly string[] CharacterPaths = { Passenger01Path, Passenger02Path, Passenger03Path };

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
            Material red = Material("Preview Warning Red", new Color(0.85f, 0.08f, 0.06f));
            Material buildingGarage = TextureMaterial("Preview Garage Texture", "Assets/Resources/TerminalRoute/World/Buildings/Garage/texture.png", new Color(0.35f, 0.32f, 0.26f));
            Material buildingPanel1 = TextureMaterial("Preview Panelak 1 Texture", "Assets/Resources/TerminalRoute/World/Buildings/Panelak1/texture.png", new Color(0.35f, 0.25f, 0.24f));
            Material buildingPanel2 = TextureMaterial("Preview Panelak 2 Texture", "Assets/Resources/TerminalRoute/World/Buildings/Panelak2/texture (2).png", new Color(0.30f, 0.24f, 0.20f));
            Material car01 = TextureMaterial("Preview Car 01 Texture", "Assets/Resources/TerminalRoute/World/Cars/Car01/car.png", new Color(0.15f, 0.16f, 0.18f));
            Material car03 = TextureMaterial("Preview Car 03 Texture", "Assets/Resources/TerminalRoute/World/Cars/Car03/car3.png", new Color(0.35f, 0.10f, 0.08f));
            Material car05 = TextureMaterial("Preview Car 05 Texture", "Assets/Resources/TerminalRoute/World/Cars/Car05/car5.png", new Color(0.20f, 0.24f, 0.18f));
            Material car06 = TextureMaterial("Preview Burned Car Texture", "Assets/Resources/TerminalRoute/World/Cars/Car06/car6.png", new Color(0.08f, 0.07f, 0.06f));
            Material car08 = TextureMaterial("Preview Van Texture", "Assets/Resources/TerminalRoute/World/Cars/Car08/Car8.png", new Color(0.18f, 0.24f, 0.32f));
            Material monkey = TextureMaterialTransparent("Preview Monkey Cutout", "Assets/Resources/TerminalRoute/Art/ScaryMonkeyCutout.png", Color.white);
            Material treeBillboard = AssetDatabase.LoadAssetAtPath<Material>("Assets/TerminalRoute/Resources/TerminalRoute/Materials/TreeBillboard.mat");
            if (treeBillboard == null)
            {
                treeBillboard = Material("Preview Tree Billboard", new Color(0.22f, 0.35f, 0.20f));
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/TerminalRoute/AssetPacks/Bus_stop/Texture/Tree_01.png");
                treeBillboard.mainTexture = texture;
                EnableAlphaClip(treeBillboard, 0.22f);
            }

            Material[] buildingMaterials = { buildingGarage, buildingPanel1, buildingPanel2 };
            Material[] carMaterials = { car01, car03, car05, car06, car08 };

            for (int i = 0; i < 28; i++)
            {
                float z = i * 38f;
                Cube("Preview Road " + i, root.transform, new Vector3(0f, -0.08f, z), new Vector3(8f, 0.12f, 38f), road);
                Cube("Preview Left Shoulder " + i, root.transform, new Vector3(-9.0f, -0.12f, z), new Vector3(9.8f, 0.08f, 38f), grass);
                Cube("Preview Right Shoulder " + i, root.transform, new Vector3(9.0f, -0.12f, z), new Vector3(9.8f, 0.08f, 38f), grass);
                Cube("Preview Left Edge " + i, root.transform, new Vector3(-3.85f, 0.02f, z), new Vector3(0.08f, 0.035f, 33f), line);
                Cube("Preview Right Edge " + i, root.transform, new Vector3(3.85f, 0.02f, z), new Vector3(0.08f, 0.035f, 33f), line);
                for (int d = 0; d < 4; d++)
                {
                    Cube("Preview Center Dash " + i + "-" + d, root.transform, new Vector3(0f, 0.025f, z - 14f + d * 9f), new Vector3(0.08f, 0.035f, 3.6f), line);
                }

                BuildTree(root.transform, new Vector3(-8.8f, 0f, z - 9f), grass, metal, treeBillboard);
                BuildTree(root.transform, new Vector3(8.5f, 0f, z + 6f), grass, metal, treeBillboard);
                BuildTree(root.transform, new Vector3(-13.6f, 0f, z + 4f), grass, metal, treeBillboard);
                BuildTree(root.transform, new Vector3(13.2f, 0f, z - 13f), grass, metal, treeBillboard);

                if (i % 2 == 0)
                {
                    BuildPreviewBuilding(root.transform, i, -1f, z + 11f, buildingMaterials);
                    BuildPreviewBuilding(root.transform, i + 3, 1f, z - 7f, buildingMaterials);
                }
            }

            BuildPreviewBus(root.transform, new Vector3(-9.4f, 0f, 18f), bus, glass, metal, green, amber);
            BuildPreviewBus(root.transform, new Vector3(-1.9f, 0f, 205f), bus, glass, metal, green, amber);
            TreeCard("Preview Monkey In Bus", root.transform, new Vector3(0.7f, 1.55f, 46f), new Vector2(0.92f, 1.35f), 180f, monkey);
            TextMesh("SANITY  87%", root.transform, new Vector3(-3.1f, 2.65f, 14.4f), 0.18f, green);
            TextMesh("STOP  //  DOORS OPEN", root.transform, new Vector3(-1.6f, 2.35f, 14.4f), 0.18f, amber);

            for (int i = 0; i < 8; i++)
            {
                BuildPreviewStop(root.transform, 86f + i * 118f, metal, glass, bus, green, amber);
            }

            for (int i = 0; i < 16; i++)
            {
                BuildPreviewParkedCar(root.transform, i, carMaterials, bus);
            }

            BuildPreviewHazards(root.transform, red, amber, metal);
            PlaceExtracted("Preview Roadside House L", HousePath, root.transform, new Vector3(-17.5f, 0f, 74f), new Vector3(0f, 90f, 0f), 0.28f);
            PlaceExtracted("Preview Roadside House R", HousePath, root.transform, new Vector3(17.5f, 0f, 118f), new Vector3(0f, -90f, 0f), 0.28f);

            EditorSceneManager.MarkSceneDirty(scene);
        }

        private static void BuildMenuPreview(Transform parent)
        {
            Material sign = Material("Preview Menu Sign", new Color(0.025f, 0.035f, 0.030f));
            Material green = Material("Preview Menu Green", new Color(0.20f, 0.95f, 0.22f));
            Material red = Material("Preview Menu Blood", new Color(0.72f, 0.02f, 0.02f));
            Material image = TextureMaterial("Preview Menu Generated Art", "Assets/Resources/TerminalRoute/Art/MenuBackground.png", new Color(0.13f, 0.17f, 0.18f));
            Material logo = TextureMaterialTransparent("Preview Menu Logo", "Assets/Resources/TerminalRoute/Art/MenuLogo.png", Color.white);

            Cube("Menu Preview Backdrop", parent, new Vector3(0f, 3.0f, 6f), new Vector3(16f, 9f, 0.08f), image);
            Cube("Menu Preview Title Sign", parent, new Vector3(0f, 5.25f, 5.82f), new Vector3(6.8f, 1.05f, 0.12f), sign);
            TreeCard("Menu Preview Logo Image", parent, new Vector3(0f, 5.30f, 5.58f), new Vector2(5.4f, 1.22f), 0f, logo);
            TextMesh("TERMINAL ROUTE", parent, new Vector3(-2.36f, 4.68f, 5.65f), 0.18f, green);
            Cube("Preview Blood Drip A", parent, new Vector3(-0.56f, 4.78f, 5.60f), new Vector3(0.045f, 0.22f, 0.06f), red);
            Cube("Preview Blood Drip B", parent, new Vector3(0.22f, 4.72f, 5.60f), new Vector3(0.05f, 0.31f, 0.06f), red);
            TextMesh("NEW TRIP", parent, new Vector3(2.55f, 3.92f, 5.66f), 0.30f, green);
            TextMesh("NIGHTMARE", parent, new Vector3(2.30f, 3.38f, 5.66f), 0.26f, green);
            TextMesh("CONTROLS", parent, new Vector3(2.42f, 2.88f, 5.66f), 0.25f, green);
            TextMesh("CREDITS", parent, new Vector3(2.58f, 2.38f, 5.66f), 0.25f, green);
            BuildPreviewMonkey(parent, new Vector3(-4.2f, 2.1f, 5.60f), red, green);
        }

        private static void BuildEndingPreview(Transform parent)
        {
            Material black = Material("Preview Ending Black", new Color(0.006f, 0.008f, 0.010f));
            Material panel = Material("Preview Ending Panel", new Color(0.015f, 0.045f, 0.025f));
            Material green = Material("Preview Ending Green", new Color(0.20f, 0.95f, 0.22f));
            Material amber = Material("Preview Ending Amber", new Color(0.95f, 0.50f, 0.12f));
            Material red = Material("Preview Ending Red", new Color(0.95f, 0.18f, 0.12f));

            Cube("Ending Preview Backdrop", parent, new Vector3(0f, 3.0f, 6f), new Vector3(16f, 9f, 0.08f), black);
            Cube("Ending Preview Road Ghost", parent, new Vector3(0f, 2.15f, 5.84f), new Vector3(2.7f, 5.0f, 0.08f), Material("Preview Ending Road", new Color(0.04f, 0.06f, 0.06f)));
            Cube("Ending Preview Top Eyelid", parent, new Vector3(0f, 6.15f, 5.70f), new Vector3(16f, 2.2f, 0.11f), black);
            Cube("Ending Preview Bottom Eyelid", parent, new Vector3(0f, -0.15f, 5.70f), new Vector3(16f, 2.2f, 0.11f), black);
            Cube("Ending Preview Result Panel", parent, new Vector3(0f, 3.35f, 5.82f), new Vector3(8.6f, 4.4f, 0.12f), panel);
            Cube("Ending Preview Result Line Top", parent, new Vector3(0f, 5.42f, 5.66f), new Vector3(7.7f, 0.035f, 0.06f), green);
            Cube("Ending Preview Result Line Bottom", parent, new Vector3(0f, 1.28f, 5.66f), new Vector3(7.7f, 0.035f, 0.06f), green);
            Cube("Ending Preview Red Accent L", parent, new Vector3(-4.18f, 3.35f, 5.65f), new Vector3(0.04f, 3.7f, 0.06f), red);
            Cube("Ending Preview Red Accent R", parent, new Vector3(4.18f, 3.35f, 5.65f), new Vector3(0.04f, 3.7f, 0.06f), red);
            TextMesh("YOU LOST", parent, new Vector3(-1.70f, 4.72f, 5.65f), 0.52f, red);
            TextMesh("CAUSE  //  SANITY ZERO", parent, new Vector3(-2.18f, 4.12f, 5.65f), 0.27f, amber);
            TextMesh("MODE  ROUTE 04     TIME  01:28", parent, new Vector3(-2.70f, 3.48f, 5.65f), 0.24f, green);
            TextMesh("STOPS  04/08     MISSES  01/2", parent, new Vector3(-2.70f, 3.08f, 5.65f), 0.24f, green);
            TextMesh("SANITY  00%", parent, new Vector3(-1.00f, 2.68f, 5.65f), 0.24f, green);
            TextMesh("THE DRIVER CLOSED HIS EYES BEFORE THE LAST STOP.", parent, new Vector3(-3.55f, 2.08f, 5.65f), 0.18f, amber);
            TextMesh("RETURN TO TERMINAL", parent, new Vector3(-1.42f, 1.58f, 5.65f), 0.25f, green);
        }

        private static void BuildPreviewBuilding(Transform parent, int index, float side, float z, Material[] materials)
        {
            int buildingIndex = Mathf.Abs(index) % BuildingPaths.Length;
            string path = BuildingPaths[buildingIndex];
            Material material = materials[Mathf.Clamp(buildingIndex, 0, materials.Length - 1)];
            float x = side * (14.5f + (index % 4) * 3.2f);
            float height = 5.8f + (index % 5) * 1.1f;
            float yaw = side < 0f ? 90f : -90f;
            GameObject building = PlacePreviewModel("Preview Urban Building " + index, path, parent, new Vector3(x, 0f, z), new Vector3(0f, yaw, 0f), height, material);
            if (building == null)
            {
                Cube("Preview Block Building " + index, parent, new Vector3(x, height * 0.5f, z), new Vector3(3.3f, height, 4.2f), material);
            }

            if (index % 3 == 0)
            {
                Cube("Preview Building Lit Window A " + index, parent, new Vector3(side * (Mathf.Abs(x) - 1.72f), height * 0.66f, z - 0.9f), new Vector3(0.05f, 0.28f, 0.34f), Material("Preview Warm Window " + index, new Color(0.95f, 0.58f, 0.18f)));
                Cube("Preview Building Lit Window B " + index, parent, new Vector3(side * (Mathf.Abs(x) - 1.72f), height * 0.42f, z + 0.9f), new Vector3(0.05f, 0.24f, 0.32f), Material("Preview Green Window " + index, new Color(0.22f, 0.80f, 0.24f)));
            }
        }

        private static void BuildPreviewParkedCar(Transform parent, int index, Material[] materials, Material fallbackMaterial)
        {
            int carIndex = Mathf.Abs(index) % CarPaths.Length;
            string path = CarPaths[carIndex];
            Material material = materials[Mathf.Clamp(carIndex, 0, materials.Length - 1)];
            float side = index % 2 == 0 ? -1f : 1f;
            float z = 68f + index * 58f + (index % 4) * 7f;
            float x = side * (7.3f + (index % 3) * 0.72f);
            float yaw = side < 0f ? 0f : 180f;
            GameObject car = PlacePreviewModel("Preview Parked PSX Car " + index, path, parent, new Vector3(x, 0f, z), new Vector3(0f, yaw, 0f), 0.92f, material);
            if (car == null)
            {
                Cube("Preview Parked Car Fallback " + index, parent, new Vector3(x, 0.38f, z), new Vector3(1.55f, 0.70f, 3.1f), fallbackMaterial);
            }
        }

        private static void BuildPreviewHazards(Transform parent, Material red, Material amber, Material metal)
        {
            for (int i = 0; i < 8; i++)
            {
                float z = 132f + i * 96f;
                float x = i % 2 == 0 ? -1.55f : 1.65f;
                Cube("Preview Road Object " + i, parent, new Vector3(x, 0.08f, z), new Vector3(0.72f, 0.16f, 0.58f), metal);
                Cube("Preview Warning Cone " + i, parent, new Vector3(-x * 0.62f, 0.22f, z + 3.8f), new Vector3(0.28f, 0.44f, 0.28f), amber);
                Cube("Preview Hazard Red Glint " + i, parent, new Vector3(x, 0.23f, z - 0.32f), new Vector3(0.16f, 0.06f, 0.06f), red);
            }
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
            Cube("Preview Stop Capture Zone", parent, new Vector3(2.55f, 0.025f, z), new Vector3(1.75f, 0.025f, 8.8f), green);
            Cube("Preview Stop Curb Glow", parent, new Vector3(3.58f, 0.06f, z), new Vector3(0.16f, 0.055f, 7.4f), amber);

            if (PlaceExtracted("Preview Bus Stop Asset", stopPath, parent, new Vector3(6.7f, 0f, z), new Vector3(0f, -90f, 0f), 1f) != null)
            {
                PlaceExtracted("Preview Stop Sign Asset", BusStopSignPath, parent, new Vector3(4.8f, 0f, z - 2.25f), new Vector3(0f, -90f, 0f), 0.95f);
                PlaceExtracted("Preview Stop Lamp Asset", LamppostPath, parent, new Vector3(6.15f, 0f, z + 2.25f), new Vector3(0f, 90f, 0f), 1f);
                BuildPreviewWaitingPassengers(parent, z);
                return;
            }

            Cube("Preview Stop Platform", parent, new Vector3(6.35f, 0.02f, z), new Vector3(3.4f, 0.12f, 5.2f), metal);
            Cube("Preview Stop Roof", parent, new Vector3(6.65f, 2.05f, z), new Vector3(3.1f, 0.18f, 3.7f), roof);
            Cube("Preview Stop Glass", parent, new Vector3(7.95f, 1.12f, z), new Vector3(0.10f, 1.75f, 3.1f), glass);
            Cube("Preview Stop Bench", parent, new Vector3(6.55f, 0.55f, z + 0.72f), new Vector3(1.65f, 0.18f, 0.42f), roof);
            Cube("Preview Stop Sign Pole", parent, new Vector3(4.9f, 1.05f, z - 1.8f), new Vector3(0.12f, 2.1f, 0.12f), amber);
            Cube("Preview Stop Sign", parent, new Vector3(4.9f, 2.2f, z - 1.8f), new Vector3(0.65f, 0.65f, 0.08f), green);
            BuildPreviewWaitingPassengers(parent, z);
        }

        private static void BuildPreviewWaitingPassengers(Transform parent, float z)
        {
            for (int i = 0; i < 3; i++)
            {
                float offset = -1.55f + i * 1.05f;
                PlacePreviewCharacter("Preview Waiting Passenger " + Mathf.RoundToInt(z) + "-" + i, parent, new Vector3(5.7f + i * 0.24f, 0f, z + offset), new Vector3(0f, -90f, 0f), i);
            }
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

        private static GameObject PlacePreviewModel(string name, string path, Transform parent, Vector3 position, Vector3 eulerAngles, float targetHeight, Material overrideMaterial = null)
        {
            GameObject gameObject = PlaceExtracted(name, path, parent, position, eulerAngles, 1f, overrideMaterial);
            if (gameObject == null)
            {
                return null;
            }

            FitObjectToHeight(gameObject, targetHeight, position.y);
            return gameObject;
        }

        private static GameObject PlacePreviewCharacter(string name, Transform parent, Vector3 position, Vector3 eulerAngles, int poseVariant)
        {
            string path = CharacterPaths[Mathf.Abs(poseVariant) % CharacterPaths.Length];
            GameObject character = PlaceExtracted(name, path, parent, position, eulerAngles, 1f);
            if (character == null)
            {
                Material cloth = Material(name + " Cloth", new Color(0.16f, 0.18f, 0.18f));
                Material skin = Material(name + " Skin", new Color(0.62f, 0.52f, 0.42f));
                var fallback = new GameObject(name);
                fallback.transform.SetParent(parent, false);
                fallback.transform.localPosition = position;
                fallback.transform.localRotation = Quaternion.Euler(eulerAngles);
                TrySetEditorOnly(fallback);
                Cube("Preview Passenger Body", fallback.transform, new Vector3(0f, 0.78f, 0f), new Vector3(0.34f, 0.80f, 0.22f), cloth);
                Sphere("Preview Passenger Head", fallback.transform, new Vector3(0f, 1.34f, 0f), new Vector3(0.26f, 0.30f, 0.24f), skin);
                Cube("Preview Passenger Arm L", fallback.transform, new Vector3(-0.22f, 0.78f, 0f), new Vector3(0.08f, 0.42f, 0.08f), cloth);
                Cube("Preview Passenger Arm R", fallback.transform, new Vector3(0.22f, 0.78f, 0f), new Vector3(0.08f, 0.42f, 0.08f), cloth);
                return fallback;
            }

            PoseStanding(character.transform, poseVariant);
            FitObjectToHeight(character, 1.58f, position.y);
            return character;
        }

        private static void FitObjectToHeight(GameObject gameObject, float targetHeight, float groundY)
        {
            Bounds bounds;
            if (!TryGetBounds(gameObject, out bounds) || bounds.size.y <= 0.001f)
            {
                return;
            }

            float scale = targetHeight / bounds.size.y;
            gameObject.transform.localScale *= scale;

            if (TryGetBounds(gameObject, out bounds))
            {
                gameObject.transform.position += Vector3.up * (groundY - bounds.min.y);
            }
        }

        private static bool TryGetBounds(GameObject gameObject, out Bounds bounds)
        {
            var renderers = gameObject.GetComponentsInChildren<Renderer>(true);
            bounds = new Bounds(gameObject.transform.position, Vector3.zero);
            bool hasBounds = false;

            foreach (var renderer in renderers)
            {
                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return hasBounds;
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

        private static Material TextureMaterialTransparent(string name, string texturePath, Color fallbackColor)
        {
            Material material = TextureMaterial(name, texturePath, fallbackColor);
            EnableTransparency(material);
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

        private static void EnableTransparency(Material material)
        {
            if (material == null)
            {
                return;
            }

            if (material.HasProperty("_Surface"))
            {
                material.SetFloat("_Surface", 1f);
            }

            if (material.HasProperty("_Blend"))
            {
                material.SetFloat("_Blend", 0f);
            }

            if (material.HasProperty("_SrcBlend"))
            {
                material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            }

            if (material.HasProperty("_DstBlend"))
            {
                material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            }

            if (material.HasProperty("_ZWrite"))
            {
                material.SetFloat("_ZWrite", 0f);
            }

            if (material.HasProperty("_Cull"))
            {
                material.SetInt("_Cull", 0);
            }

            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = 3000;
        }

        private static void PoseStanding(Transform model, int poseVariant)
        {
            int variant = Mathf.Abs(poseVariant) % 5;

            RotateBone(model, "hips", new Vector3(0f, variant % 2 == 0 ? -3f : 3f, variant % 2 == 0 ? -2f : 2f));
            RotateBone(model, "leftupleg", new Vector3(variant == 4 ? -8f : -3f, 0f, -4f));
            RotateBone(model, "rightupleg", new Vector3(variant == 3 ? -8f : -3f, 0f, 4f));
            RotateBone(model, "leftleg", new Vector3(variant == 4 ? 7f : 2f, 0f, 0f));
            RotateBone(model, "rightleg", new Vector3(variant == 3 ? 7f : 2f, 0f, 0f));
            RotateBone(model, "spine", new Vector3(4f, 0f, variant % 2 == 0 ? -4f : 4f));
            RotateBone(model, "head", new Vector3(variant == 1 ? 7f : 1f, variant == 2 ? -10f : 0f, variant % 2 == 0 ? 3f : -3f));

            switch (variant)
            {
                case 1:
                    RotateBone(model, "leftarm", new Vector3(42f, 4f, -78f));
                    RotateBone(model, "leftforearm", new Vector3(22f, 0f, -72f));
                    RotateBone(model, "rightarm", new Vector3(18f, -4f, 72f));
                    RotateBone(model, "rightforearm", new Vector3(6f, 0f, 30f));
                    break;
                case 2:
                    RotateBone(model, "leftarm", new Vector3(36f, 8f, -66f));
                    RotateBone(model, "rightarm", new Vector3(36f, -8f, 66f));
                    RotateBone(model, "leftforearm", new Vector3(8f, 0f, -88f));
                    RotateBone(model, "rightforearm", new Vector3(8f, 0f, 88f));
                    break;
                case 3:
                    RotateBone(model, "leftarm", new Vector3(18f, 8f, -86f));
                    RotateBone(model, "rightarm", new Vector3(46f, -4f, 54f));
                    RotateBone(model, "leftforearm", new Vector3(4f, 0f, -28f));
                    RotateBone(model, "rightforearm", new Vector3(18f, 0f, 82f));
                    break;
                case 4:
                    RotateBone(model, "leftarm", new Vector3(48f, 0f, -54f));
                    RotateBone(model, "rightarm", new Vector3(20f, 0f, 84f));
                    RotateBone(model, "leftforearm", new Vector3(12f, 0f, -74f));
                    RotateBone(model, "rightforearm", new Vector3(5f, 0f, 32f));
                    break;
                default:
                    RotateBone(model, "leftarm", new Vector3(22f, 4f, -82f));
                    RotateBone(model, "rightarm", new Vector3(22f, -4f, 82f));
                    RotateBone(model, "leftforearm", new Vector3(5f, 0f, -36f));
                    RotateBone(model, "rightforearm", new Vector3(5f, 0f, 36f));
                    break;
            }
        }

        private static void RotateBone(Transform root, string token, Vector3 localEuler)
        {
            Transform bone = FindNamedTransform(root, token);
            if (bone != null)
            {
                bone.localRotation *= Quaternion.Euler(localEuler);
            }
        }

        private static Transform FindNamedTransform(Transform root, string token)
        {
            string normalizedToken = token.ToLowerInvariant();
            var children = root.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                if (child.name.ToLowerInvariant().Contains(normalizedToken))
                {
                    return child;
                }
            }

            return null;
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

            foreach (Transform child in gameObject.transform)
            {
                TrySetEditorOnly(child.gameObject);
            }
        }
    }
}
