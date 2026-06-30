using System.Collections.Generic;
using TerminalRoute.Core;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TerminalRoute.Runtime
{
    public sealed class SceneFactory
    {
        private readonly GameMode mode;
        private readonly int targetStopCount;
        private readonly float routeStopInterval;
        private readonly Material roadMaterial = ResourceMaterial("RoadAsphalt", "TR Road", new Color(0.10f, 0.10f, 0.14f));
        private readonly Material groundMaterial = ResourceMaterial("Grass", "TR Grass", new Color(0.06f, 0.11f, 0.07f));
        private readonly Material concreteMaterial = ResourceMaterial("RoadConcrete", "TR Concrete", new Color(0.26f, 0.25f, 0.23f));
        private readonly Material fenceMaterial = ResourceMaterial("Fence", "TR Fence", new Color(0.18f, 0.18f, 0.16f));
        private readonly Material glassMaterial = ResourceMaterial("WindowGlass", "TR Glass", new Color(0.08f, 0.14f, 0.18f, 0.55f));
        private readonly Material busPaintMaterial = ResourceMaterial("BusPaint", "TR Bus Paint", new Color(0.08f, 0.10f, 0.16f));
        private readonly Material laneMaterial = Material("TR Lane", new Color(0.55f, 0.50f, 0.32f));
        private readonly Material busMaterial = ResourceMaterial("BusInterior", "TR Bus Interior", new Color(0.08f, 0.09f, 0.13f));
        private readonly Material dashMaterial = Material("TR Dashboard", new Color(0.04f, 0.045f, 0.055f));
        private readonly Material seatMaterial = ResourceMaterial("SeatVinyl", "TR Seats", new Color(0.19f, 0.16f, 0.10f));
        private readonly Material greenMaterial = Material("TR Phosphor", new Color(0.20f, 0.95f, 0.22f));
        private readonly Material stopZoneMaterial = Material("TR Stop Zone", new Color(0.02f, 0.22f, 0.05f));
        private readonly Material amberMaterial = Material("TR Amber", new Color(0.95f, 0.55f, 0.12f));
        private readonly Material skinMaterial = Material("TR Skin", new Color(0.58f, 0.53f, 0.43f));
        private readonly Material clothMaterial = Material("TR Cloth", new Color(0.23f, 0.18f, 0.19f));
        private readonly Material monkeyMaterial = Material("TR Monkey", new Color(0.10f, 0.075f, 0.055f));
        private readonly Material ballMaterial = Material("TR Ball", new Color(0.70f, 0.70f, 0.62f));
        private readonly Material fogTreeMaterial = Material("TR Tree", new Color(0.05f, 0.08f, 0.07f));
        private readonly Material treeBillboardMaterial = ResourceMaterial("TreeBillboard", "TR Tree Billboard", new Color(0.22f, 0.35f, 0.20f));
        private readonly Material scaryMonkeyMaterial = TexturedBillboardMaterial("TR Scary Monkey", "TerminalRoute/Art/ScaryMonkeyCutout", Color.white);
        private readonly Vector3 ballStart = new Vector3(0f, 0.25f, -9.2f);
        private readonly GameObject busStopPrefab = ExtractedPrefab("BusStop", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Bus_stop.prefab");
        private readonly GameObject busStopAltPrefab = ExtractedPrefab("BusStopAlt", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Stop_Alt/Bus_stop_001.prefab");
        private readonly GameObject busStopSignPrefab = ExtractedPrefab("BusStopSign", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Bus_stop_sign.prefab");
        private readonly GameObject busPrefab = ExtractedPrefab("Bus", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Bus.prefab");
        private readonly GameObject fencePrefab = ExtractedPrefab("Fence", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Stop_Alt/Fence.prefab");
        private readonly GameObject housePrefab = ExtractedPrefab("House", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/House.prefab");
        private readonly GameObject lamppostPrefab = ExtractedPrefab("Lamppost", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Lamppost.prefab");
        private readonly GameObject treePrefab = ExtractedPrefab("Tree", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Tree.prefab");
        private readonly GameObject treeAltPrefab = ExtractedPrefab("TreeAlt", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Props/Tree_01.prefab");
        private readonly GameObject treeTallPrefab = ExtractedPrefab("TreeTall", "Assets/TerminalRoute/ExtractedPrefabs/BusStop_Stop_Alt/Tree_001.prefab");
        private readonly GameObject urbanGaragePrefab = WorldPrefab("TerminalRoute/World/Buildings/Garage/model", "Assets/Resources/TerminalRoute/World/Buildings/Garage/model.fbx");
        private readonly GameObject urbanPanelak1Prefab = WorldPrefab("TerminalRoute/World/Buildings/Panelak1/Panelak 1", "Assets/Resources/TerminalRoute/World/Buildings/Panelak1/Panelak 1.fbx");
        private readonly GameObject urbanPanelak2Prefab = WorldPrefab("TerminalRoute/World/Buildings/Panelak2/model", "Assets/Resources/TerminalRoute/World/Buildings/Panelak2/model.fbx");
        private readonly Material urbanGarageMaterial = TexturedWorldMaterial("TR Urban Garage", "TerminalRoute/World/Buildings/Garage/texture", Color.white, false);
        private readonly Material urbanPanelak1Material = TexturedWorldMaterial("TR Urban Panelak 1", "TerminalRoute/World/Buildings/Panelak1/texture", Color.white, false);
        private readonly Material urbanPanelak2Material = TexturedWorldMaterial("TR Urban Panelak 2", "TerminalRoute/World/Buildings/Panelak2/texture (2)", Color.white, false);
        private readonly GameObject car01Prefab = WorldPrefab("TerminalRoute/World/Cars/Car01/Car", "Assets/Resources/TerminalRoute/World/Cars/Car01/Car.obj");
        private readonly GameObject car03Prefab = WorldPrefab("TerminalRoute/World/Cars/Car03/Car3", "Assets/Resources/TerminalRoute/World/Cars/Car03/Car3.obj");
        private readonly GameObject car05Prefab = WorldPrefab("TerminalRoute/World/Cars/Car05/Car5", "Assets/Resources/TerminalRoute/World/Cars/Car05/Car5.obj");
        private readonly GameObject car05PolicePrefab = WorldPrefab("TerminalRoute/World/Cars/Car05/Car5_Police", "Assets/Resources/TerminalRoute/World/Cars/Car05/Car5_Police.obj");
        private readonly GameObject car06Prefab = WorldPrefab("TerminalRoute/World/Cars/Car06/Car6", "Assets/Resources/TerminalRoute/World/Cars/Car06/Car6.obj");
        private readonly GameObject car08Prefab = WorldPrefab("TerminalRoute/World/Cars/Car08/Car8", "Assets/Resources/TerminalRoute/World/Cars/Car08/Car8.obj");
        private readonly Material car01Material = TexturedWorldMaterial("TR Car 01", "TerminalRoute/World/Cars/Car01/car", Color.white, false);
        private readonly Material car03Material = TexturedWorldMaterial("TR Car 03", "TerminalRoute/World/Cars/Car03/car3", Color.white, false);
        private readonly Material car05Material = TexturedWorldMaterial("TR Car 05", "TerminalRoute/World/Cars/Car05/car5", Color.white, false);
        private readonly Material car05PoliceMaterial = TexturedWorldMaterial("TR Car 05 Police", "TerminalRoute/World/Cars/Car05/car5_police", Color.white, false);
        private readonly Material car06Material = TexturedWorldMaterial("TR Burned Car", "TerminalRoute/World/Cars/Car06/car6", Color.white, false);
        private readonly Material car08Material = TexturedWorldMaterial("TR Car 08", "TerminalRoute/World/Cars/Car08/Car8", Color.white, false);
        private readonly GameObject[] urbanBuildingPrefabs;
        private readonly GameObject[] parkedCarPrefabs;
        private readonly Material[] urbanBuildingMaterials;
        private readonly Material[] parkedCarMaterials;
        private readonly GameObject[] passengerPrefabs =
        {
            CharacterPrefab("Passenger01", "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Male/Character_01.fbx"),
            CharacterPrefab("Passenger02", "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Male/Character_04.fbx"),
            CharacterPrefab("Passenger03", "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Male/Character_10.fbx"),
            CharacterPrefab("Passenger04", "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Female/Character_Female_01.fbx"),
            CharacterPrefab("Passenger05", "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Female/Character_29_Female.fbx"),
            CharacterPrefab("Passenger06", "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Female/Character_31_Female.fbx")
        };
        private readonly GameObject[] closeThreatPrefabs =
        {
            CharacterPrefab("Passenger03", "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Killers/Character_Monster_02.fbx"),
            CharacterPrefab("Passenger05", "Assets/TerminalRoute/AssetPacks/Characters_psx/Models/Killers/Character_Killer_03.fbx")
        };
        private int treeVariant;

        private sealed class StopBoardingPassenger
        {
            public Transform Root;
            public Vector3 Start;
            public Vector3 Door;
            public float Delay;
        }

        private readonly List<List<StopBoardingPassenger>> stopBoardingPassengers = new List<List<StopBoardingPassenger>>();
        private List<StopBoardingPassenger> activeBoardingPassengers;
        private float boardingTimer;
        private float boardingDuration;

        public readonly List<PassengerVisual> Passengers = new List<PassengerVisual>();

        public Transform Rig { get; private set; }
        public Transform CameraTransform { get; private set; }
        public Camera MainCamera { get; private set; }
        public GameObject WorldRoot { get; private set; }
        public GameObject BusRoot { get; private set; }
        public GameObject ProceduralInteriorRoot { get; private set; }
        public GameObject PlayerBusShell { get; private set; }
        public GameObject Monkey { get; private set; }
        public GameObject Ball { get; private set; }
        public Transform SteeringWheel { get; private set; }
        private Transform frontDoorPanel;
        private Transform rearDoorPanel;

        public SceneFactory()
            : this(GameMode.Route04)
        {
        }

        public SceneFactory(GameMode mode)
        {
            this.mode = mode;
            targetStopCount = mode == GameMode.Nightmare ? GameState.NightmareStopCount : GameState.FinalStopCount;
            routeStopInterval = mode == GameMode.Nightmare ? GameState.NightmareStopInterval : GameState.DefaultStopInterval;
            urbanBuildingPrefabs = new[] { urbanGaragePrefab, urbanPanelak1Prefab, urbanPanelak2Prefab };
            parkedCarPrefabs = new[] { car01Prefab, car03Prefab, car05Prefab, car05PolicePrefab, car06Prefab, car08Prefab };
            urbanBuildingMaterials = new[] { urbanGarageMaterial, urbanPanelak1Material, urbanPanelak2Material };
            parkedCarMaterials = new[] { car01Material, car03Material, car05Material, car05PoliceMaterial, car06Material, car08Material };
        }

        public void Build()
        {
            DisableTemplateCameras();
            ConfigureLighting();
            BuildWorld();
            BuildBus();
        }

        public void SetPlayObjectsVisible(bool visible)
        {
            if (MainCamera != null)
            {
                MainCamera.enabled = true;
            }

            if (WorldRoot != null)
            {
                WorldRoot.SetActive(visible);
            }

            if (BusRoot != null)
            {
                BusRoot.SetActive(visible);
            }
        }

        public void SetMirrorMode(bool mirrorActive)
        {
            if (PlayerBusShell != null)
            {
                PlayerBusShell.SetActive(mirrorActive);
            }

            if (ProceduralInteriorRoot != null)
            {
                ProceduralInteriorRoot.SetActive(!mirrorActive);
            }
        }

        public void ResetDynamicObjects(int passengerCount)
        {
            Rig.position = Vector3.zero;
            Rig.rotation = Quaternion.identity;
            CameraTransform.localPosition = new Vector3(0f, 1.35f, 0.35f);
            CameraTransform.localRotation = Quaternion.identity;

            foreach (var passenger in Passengers)
            {
                passenger.SetStaring(false);
            }

            SetPassengerCount(passengerCount);
            SetMonkeyRow(10);
            Monkey.SetActive(false);
            Ball.transform.localPosition = ballStart;
            Ball.SetActive(false);
            ResetStopBoardingPassengers();
            SetDoorsOpen(false);
        }

        public void SetPassengerCount(int passengerCount)
        {
            int visiblePassengers = Mathf.Clamp(passengerCount, 0, Passengers.Count);

            for (int i = 0; i < Passengers.Count; i++)
            {
                Passengers[i].SetPresent(i < visiblePassengers);
            }
        }

        public void SetMonkeyRow(int row)
        {
            float z = -2.1f - Mathf.Clamp(row, 0, 10) * 0.72f;
            Monkey.transform.localPosition = new Vector3(0f, 0.98f, z);
        }

        public void SetBallProgress(float progress)
        {
            Ball.transform.localPosition = Vector3.Lerp(ballStart, new Vector3(0f, 0.25f, -1.15f), Mathf.Clamp01(progress));
        }

        public void SetDoorsOpen(bool open)
        {
            if (frontDoorPanel == null || rearDoorPanel == null)
            {
                return;
            }

            frontDoorPanel.localPosition = open ? new Vector3(1.78f, 1.16f, 0.58f) : new Vector3(1.53f, 1.16f, 0.36f);
            rearDoorPanel.localPosition = open ? new Vector3(1.78f, 1.16f, -0.05f) : new Vector3(1.53f, 1.16f, -0.24f);
            frontDoorPanel.localRotation = Quaternion.Euler(0f, open ? -18f : 0f, 0f);
            rearDoorPanel.localRotation = Quaternion.Euler(0f, open ? 18f : 0f, 0f);
        }

        public void BeginStopBoarding(int stopIndex, float duration)
        {
            if (stopIndex < 0 || stopIndex >= stopBoardingPassengers.Count)
            {
                activeBoardingPassengers = null;
                return;
            }

            activeBoardingPassengers = stopBoardingPassengers[stopIndex];
            boardingTimer = 0f;
            boardingDuration = Mathf.Max(0.8f, duration);

            foreach (var passenger in activeBoardingPassengers)
            {
                if (passenger.Root == null)
                {
                    continue;
                }

                passenger.Root.localPosition = passenger.Start;
                passenger.Root.gameObject.SetActive(true);
            }
        }

        public void TickStopBoarding(float deltaTime)
        {
            if (activeBoardingPassengers == null)
            {
                return;
            }

            boardingTimer += deltaTime;
            float travelTime = Mathf.Max(0.35f, boardingDuration - 0.35f);

            foreach (var passenger in activeBoardingPassengers)
            {
                if (passenger.Root == null)
                {
                    continue;
                }

                float t = Mathf.Clamp01((boardingTimer - passenger.Delay) / travelTime);
                float smooth = t * t * (3f - 2f * t);
                passenger.Root.localPosition = Vector3.Lerp(passenger.Start, passenger.Door, smooth);

                if (t >= 0.94f)
                {
                    passenger.Root.gameObject.SetActive(false);
                }
            }

            if (boardingTimer >= boardingDuration)
            {
                activeBoardingPassengers = null;
            }
        }

        public GameObject BuildOncomingBus(Transform parent)
        {
            GameObject bus = PlaceExtracted("Oncoming Bus", busPrefab, parent, Vector3.zero, new Vector3(0f, 180f, 0f), 0.78f);
            if (bus == null)
            {
                bus = Cube("Oncoming Bus Body", parent, new Vector3(0f, 0.72f, 0f), new Vector3(1.8f, 1.35f, 4.8f), busPaintMaterial);
                Cube("Oncoming Bus Windshield", bus.transform, new Vector3(0f, 0.26f, -0.51f), new Vector3(0.82f, 0.32f, 0.04f), glassMaterial);
                Cube("Oncoming Bus Lights", bus.transform, new Vector3(0f, -0.18f, -0.52f), new Vector3(0.92f, 0.08f, 0.04f), amberMaterial);
                return bus;
            }

            AutoAlignLongAxisToRoute(bus);
            bus.transform.localRotation *= Quaternion.Euler(0f, 180f, 0f);
            foreach (var renderer in bus.GetComponentsInChildren<Renderer>(true))
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }

            AddOncomingHeadlights(bus.transform);
            return bus;
        }

        public GameObject BuildCloseMirrorThreat()
        {
            var root = new GameObject("Close Mirror NPC");
            root.transform.SetParent(CameraTransform, false);
            root.transform.localPosition = new Vector3(0f, 0f, 0.54f);
            root.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            root.transform.localScale = Vector3.one;

            GameObject prefab = closeThreatPrefabs.Length == 0 ? null : closeThreatPrefabs[0];
            var modelAnchor = new GameObject("Close Mirror Character Anchor").transform;
            modelAnchor.SetParent(root.transform, false);
            modelAnchor.localPosition = new Vector3(0f, -1.72f, 0f);
            modelAnchor.localRotation = Quaternion.identity;

            if (!BuildWorldCharacterModel("Close Mirror Character", modelAnchor, prefab, 2.05f, true))
            {
                Material faceMaterial = Material("TR Close Face", new Color(0.56f, 0.55f, 0.49f));
                Material mouthMaterial = Material("TR Close Mouth", new Color(0.03f, 0.02f, 0.02f));
                Sphere("Close Head", root.transform, new Vector3(0f, 0.02f, 0f), new Vector3(0.82f, 0.94f, 0.48f), faceMaterial);
                Cube("Close Left Eye", root.transform, new Vector3(-0.20f, 0.11f, 0.39f), new Vector3(0.14f, 0.075f, 0.035f), greenMaterial);
                Cube("Close Right Eye", root.transform, new Vector3(0.20f, 0.11f, 0.39f), new Vector3(0.14f, 0.075f, 0.035f), greenMaterial);
                Cube("Close Mouth", root.transform, new Vector3(0f, -0.16f, 0.41f), new Vector3(0.34f, 0.10f, 0.04f), mouthMaterial);
                Cube("Close Shoulder", root.transform, new Vector3(0f, -0.62f, 0.04f), new Vector3(1.16f, 0.42f, 0.36f), clothMaterial);
            }

            root.SetActive(false);
            return root;
        }

        private void DisableTemplateCameras()
        {
            var cameras = FindSceneCameras();
            foreach (var camera in cameras)
            {
                camera.enabled = false;
                var listener = camera.GetComponent<AudioListener>();
                if (listener != null)
                {
                    listener.enabled = false;
                }
            }
        }

        private static Camera[] FindSceneCameras()
        {
#if UNITY_2023_1_OR_NEWER
            return Object.FindObjectsByType<Camera>(FindObjectsSortMode.None);
#else
            return Object.FindObjectsOfType<Camera>();
#endif
        }

        private void ConfigureLighting()
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color(0.032f, 0.042f, 0.060f);
            RenderSettings.fogDensity = 0.018f;
            RenderSettings.ambientLight = new Color(0.052f, 0.060f, 0.070f);

            var lightObject = new GameObject("Terminal Route Moon Lamp");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(0.42f, 0.50f, 0.63f);
            light.intensity = 0.70f;
            lightObject.transform.rotation = Quaternion.Euler(42f, -25f, 0f);
        }

        private void BuildWorld()
        {
            WorldRoot = new GameObject("Terminal Route World");
            float routeLength = RouteManager.FirstStopZ + routeStopInterval * (targetStopCount + GameState.MaxMissedStops + 1) + 260f;
            int roadSegments = Mathf.CeilToInt(routeLength / 40f) + 2;
            Cube("Green Ground Plane", WorldRoot.transform, new Vector3(0f, -0.22f, routeLength * 0.5f), new Vector3(140f, 0.08f, routeLength + 300f), groundMaterial);

            for (int i = -1; i < roadSegments; i++)
            {
                float z = i * 40f;
                Cube("Road " + i, WorldRoot.transform, new Vector3(0f, -0.08f, z), new Vector3(8f, 0.12f, 40f), roadMaterial);
                Cube("Left Shoulder " + i, WorldRoot.transform, new Vector3(-7.2f, -0.11f, z), new Vector3(6.2f, 0.08f, 40f), groundMaterial);
                Cube("Right Shoulder " + i, WorldRoot.transform, new Vector3(7.2f, -0.11f, z), new Vector3(6.2f, 0.08f, 40f), groundMaterial);
                Cube("Left Edge " + i, WorldRoot.transform, new Vector3(-3.85f, 0.01f, z), new Vector3(0.08f, 0.03f, 35f), laneMaterial);
                Cube("Right Edge " + i, WorldRoot.transform, new Vector3(3.85f, 0.01f, z), new Vector3(0.08f, 0.03f, 35f), laneMaterial);

                for (int d = 0; d < 4; d++)
                {
                    Cube("Center Dash " + i + "-" + d, WorldRoot.transform, new Vector3(0f, 0.01f, z - 16f + d * 10f), new Vector3(0.08f, 0.025f, 4.4f), laneMaterial);
                }

                Cube("Left Guardrail " + i, WorldRoot.transform, new Vector3(-5.1f, 0.62f, z), new Vector3(0.16f, 0.16f, 34f), fenceMaterial);
                Cube("Right Guardrail " + i, WorldRoot.transform, new Vector3(5.1f, 0.62f, z), new Vector3(0.16f, 0.16f, 34f), fenceMaterial);

                for (int p = 0; p < 3; p++)
                {
                    float postZ = z - 15f + p * 12f;
                    Cube("Rail Post L " + i + "-" + p, WorldRoot.transform, new Vector3(-5.1f, 0.28f, postZ), new Vector3(0.18f, 0.95f, 0.18f), fenceMaterial);
                    Cube("Rail Post R " + i + "-" + p, WorldRoot.transform, new Vector3(5.1f, 0.28f, postZ), new Vector3(0.18f, 0.95f, 0.18f), fenceMaterial);
                    BuildTree(new Vector3(-9.6f, 0f, postZ + 4f), 3.4f + p * 0.45f);
                    BuildTree(new Vector3(9.9f, 0f, postZ - 2f), 3.1f + p * 0.35f);
                }

                BuildForestRow(z, i);

                if (i % 2 == 0)
                {
                    BuildLamp(new Vector3(6.4f, 0f, z + 10f));
                }
            }

            BuildStops();
            BuildRouteSetPieces();
        }

        private void BuildStops()
        {
            int stopCount = targetStopCount + GameState.MaxMissedStops + 1;
            for (int i = 0; i < stopCount; i++)
            {
                float z = RouteManager.FirstStopZ + i * routeStopInterval;
                GameObject stopPrefab = i % 2 == 0 ? busStopPrefab : busStopAltPrefab;
                if (PlaceExtracted("Bus Stop Asset " + i, stopPrefab, WorldRoot.transform, new Vector3(6.7f, 0f, z), new Vector3(0f, -90f, 0f), 1f) != null)
                {
                    BuildStopCapturePad(i, z);
                    BuildWaitingPassengers(i, z);
                    PlaceExtracted("Bus Stop Sign Asset " + i, busStopSignPrefab, WorldRoot.transform, new Vector3(4.8f, 0f, z - 2.25f), new Vector3(0f, -90f, 0f), 0.95f);
                    BuildLamp(new Vector3(6.15f, 0f, z + 2.25f));
                    PlaceExtracted("Fence Asset " + i, fencePrefab, WorldRoot.transform, new Vector3(8.45f, 0f, z + 2.7f), new Vector3(0f, 90f, 0f), 1f);
                    BuildTree(new Vector3(9.0f, 0f, z - 2.5f), 2.4f);
                    BuildTree(new Vector3(9.4f, 0f, z + 2.6f), 2.1f);
                    continue;
                }

                BuildStopCapturePad(i, z);
                BuildWaitingPassengers(i, z);
                Cube("Stop Platform " + i, WorldRoot.transform, new Vector3(6.35f, 0.02f, z), new Vector3(3.4f, 0.12f, 5.5f), concreteMaterial);
                Cube("Stop Roof " + i, WorldRoot.transform, new Vector3(6.65f, 2.05f, z), new Vector3(3.1f, 0.18f, 3.7f), busPaintMaterial);
                Cube("Stop Back Glass " + i, WorldRoot.transform, new Vector3(7.95f, 1.12f, z), new Vector3(0.10f, 1.75f, 3.1f), glassMaterial);
                Cube("Stop Front Frame " + i, WorldRoot.transform, new Vector3(5.35f, 1.12f, z - 1.48f), new Vector3(0.10f, 1.85f, 0.12f), fenceMaterial);
                Cube("Stop Rear Frame " + i, WorldRoot.transform, new Vector3(5.35f, 1.12f, z + 1.48f), new Vector3(0.10f, 1.85f, 0.12f), fenceMaterial);
                Cube("Stop Bench " + i, WorldRoot.transform, new Vector3(6.55f, 0.55f, z + 0.72f), new Vector3(1.65f, 0.18f, 0.42f), seatMaterial);
                Cube("Stop Bench Back " + i, WorldRoot.transform, new Vector3(6.9f, 0.88f, z + 0.72f), new Vector3(0.16f, 0.62f, 0.45f), seatMaterial);
                Cube("Stop Sign Pole " + i, WorldRoot.transform, new Vector3(4.9f, 1.05f, z - 1.8f), new Vector3(0.12f, 2.1f, 0.12f), amberMaterial);
                Cube("Stop Sign " + i, WorldRoot.transform, new Vector3(4.9f, 2.2f, z - 1.8f), new Vector3(0.65f, 0.65f, 0.08f), greenMaterial);
                Cube("Stop Lamp " + i, WorldRoot.transform, new Vector3(6.0f, 2.35f, z + 1.9f), new Vector3(0.36f, 0.25f, 0.36f), amberMaterial);
                BuildTree(new Vector3(9.0f, 0f, z - 2.5f), 2.4f);
                BuildTree(new Vector3(9.4f, 0f, z + 2.6f), 2.1f);
            }
        }

        private void BuildStopCapturePad(int index, float z)
        {
            Cube("Stop Capture Zone " + index, WorldRoot.transform, new Vector3(2.55f, 0.035f, z), new Vector3(1.75f, 0.025f, 8.8f), stopZoneMaterial);
            Cube("Stop Curb Glow " + index, WorldRoot.transform, new Vector3(3.58f, 0.06f, z), new Vector3(0.16f, 0.055f, 7.4f), amberMaterial);
        }

        private void BuildWaitingPassengers(int stopIndex, float z)
        {
            EnsureStopBoardingList(stopIndex);
            int count = 2 + (stopIndex % 2);
            for (int i = 0; i < count; i++)
            {
                float zOffset = -1.65f + i * 1.25f;
                Vector3 start = new Vector3(5.95f + (i % 2) * 0.42f, 0f, z + zOffset);
                Vector3 door = new Vector3(3.08f, 0f, z - 0.55f + i * 0.22f);
                stopBoardingPassengers[stopIndex].Add(BuildWaitingPassenger(stopIndex, i, start, door));
            }
        }

        private StopBoardingPassenger BuildWaitingPassenger(int stopIndex, int index, Vector3 start, Vector3 door)
        {
            var root = new GameObject("Waiting Passenger " + stopIndex + "-" + index).transform;
            root.SetParent(WorldRoot.transform, false);
            root.localPosition = start;
            root.localRotation = Quaternion.Euler(0f, -90f, 0f);

            GameObject prefab = passengerPrefabs.Length == 0 ? null : passengerPrefabs[(stopIndex + index) % passengerPrefabs.Length];
            if (!BuildWorldCharacterModel("Waiting Character Model", root, prefab, 1.58f, false, stopIndex * 3 + index))
            {
                var cloth = Material("TR Waiting Passenger Cloth " + stopIndex + "-" + index, Color.Lerp(new Color(0.11f, 0.12f, 0.14f), new Color(0.25f, 0.19f, 0.13f), ((stopIndex + index) % 4) / 3f));
                var skin = Material("TR Waiting Passenger Skin " + stopIndex + "-" + index, Color.Lerp(new Color(0.38f, 0.30f, 0.24f), new Color(0.72f, 0.62f, 0.52f), ((stopIndex * 2 + index) % 5) / 4f));

                Cube("Waiting Body", root, new Vector3(0f, 0.75f, 0f), new Vector3(0.32f, 0.78f, 0.20f), cloth);
                Sphere("Waiting Head", root, new Vector3(0f, 1.28f, -0.01f), new Vector3(0.26f, 0.30f, 0.24f), skin);
                Cube("Waiting Arm L", root, new Vector3(-0.24f, 0.76f, 0f), new Vector3(0.09f, 0.48f, 0.09f), cloth);
                Cube("Waiting Arm R", root, new Vector3(0.24f, 0.76f, 0f), new Vector3(0.09f, 0.48f, 0.09f), cloth);
                Cube("Waiting Leg L", root, new Vector3(-0.09f, 0.25f, 0f), new Vector3(0.11f, 0.50f, 0.11f), dashMaterial);
                Cube("Waiting Leg R", root, new Vector3(0.09f, 0.25f, 0f), new Vector3(0.11f, 0.50f, 0.11f), dashMaterial);
            }

            return new StopBoardingPassenger
            {
                Root = root,
                Start = start,
                Door = door,
                Delay = index * 0.28f
            };
        }

        private void EnsureStopBoardingList(int stopIndex)
        {
            while (stopBoardingPassengers.Count <= stopIndex)
            {
                stopBoardingPassengers.Add(new List<StopBoardingPassenger>());
            }
        }

        private void ResetStopBoardingPassengers()
        {
            activeBoardingPassengers = null;
            boardingTimer = 0f;

            foreach (var stopPassengers in stopBoardingPassengers)
            {
                foreach (var passenger in stopPassengers)
                {
                    if (passenger.Root == null)
                    {
                        continue;
                    }

                    passenger.Root.localPosition = passenger.Start;
                    passenger.Root.gameObject.SetActive(true);
                }
            }
        }

        private void BuildTree(Vector3 position, float height)
        {
            height *= 1.58f;
            float angle = (treeVariant * 47f) % 360f;
            float width = height * 1.05f;
            TreeCard("Tree Card A", WorldRoot.transform, position + new Vector3(0f, height * 0.5f, 0f), new Vector2(width, height), angle, treeBillboardMaterial);
            TreeCard("Tree Card B", WorldRoot.transform, position + new Vector3(0f, height * 0.5f, 0f), new Vector2(width * 0.92f, height * 0.98f), angle + 88f, treeBillboardMaterial);
            treeVariant++;
        }

        private void BuildForestRow(float segmentZ, int segmentIndex)
        {
            for (int sideIndex = 0; sideIndex < 2; sideIndex++)
            {
                float side = sideIndex == 0 ? -1f : 1f;
                for (int t = 0; t < 8; t++)
                {
                    float laneOffset = 10.4f + t * 2.55f + ((segmentIndex + t) % 2) * 0.9f;
                    float zOffset = -19f + t * 5.8f + ((segmentIndex * 5 + t * 3) % 5);
                    float height = 4.8f + ((segmentIndex + t) % 4) * 0.82f;
                    BuildTree(new Vector3(side * laneOffset, 0f, segmentZ + zOffset), height);
                }
            }
        }

        private void BuildRouteSetPieces()
        {
            BuildUrbanCorridor();
            PlaceExtracted("Abandoned Bus Left", busPrefab, WorldRoot.transform, new Vector3(-13.8f, 0f, 142f), new Vector3(0f, 180f, 0f), 0.78f);
            PlaceExtracted("Abandoned Bus Right", busPrefab, WorldRoot.transform, new Vector3(13.8f, 0f, 408f), new Vector3(0f, 0f, 0f), 0.72f);
            BuildParkedCars();
            BuildRoadClutter();
            BuildIntersections();
            BuildRoadsideSilhouettes();
            BuildTerminalGate(RouteManager.FirstStopZ + routeStopInterval * targetStopCount + 42f);
        }

        private void BuildUrbanCorridor()
        {
            float lastZ = RouteManager.FirstStopZ + routeStopInterval * targetStopCount + 180f;
            int blockCount = Mathf.Clamp(Mathf.CeilToInt(lastZ / 38f), mode == GameMode.Nightmare ? 72 : 34, mode == GameMode.Nightmare ? 124 : 68);
            for (int i = 0; i < blockCount; i++)
            {
                float z = 34f + i * 38f + ((i % 5) - 2) * 3.0f;
                if (z > lastZ)
                {
                    break;
                }

                BuildUrbanSideCluster(i, z, -1f);
                BuildUrbanSideCluster(i, z + 14f + (i % 3) * 3.5f, 1f);
            }
        }

        private void BuildUrbanSideCluster(int index, float z, float side)
        {
            float nearX = side * (11.8f + (index % 3) * 1.8f);
            float midX = side * (18.5f + ((index + 1) % 3) * 2.2f);
            float farX = side * (28.0f + (index % 4) * 3.0f);
            float nearScale = 1.18f + (index % 5) * 0.16f;
            float midScale = 1.42f + ((index + 1) % 4) * 0.18f;
            float farScale = 1.68f + ((index + 2) % 4) * 0.20f;

            BuildDistantBuilding(new Vector3(nearX, 0f, z), nearScale);
            BuildDistantBuilding(new Vector3(midX, 0f, z + 9f), midScale);
            if (index % 3 != 1)
            {
                BuildDistantBuilding(new Vector3(farX, 0f, z + 18f), farScale);
            }

            if (index % 4 == 1)
            {
                BuildDistantBuilding(new Vector3(side * 35.5f, 0f, z - 10f), 1.55f);
            }
        }

        private void BuildIntersections()
        {
            int intersectionCount = mode == GameMode.Nightmare ? 14 : 8;
            for (int i = 0; i < intersectionCount; i++)
            {
                float z = 206f + i * 188f;
                Cube("Side Road Left " + i, WorldRoot.transform, new Vector3(-10.8f, -0.07f, z), new Vector3(14f, 0.10f, 5.2f), roadMaterial);
                Cube("Side Road Right " + i, WorldRoot.transform, new Vector3(10.8f, -0.07f, z), new Vector3(14f, 0.10f, 5.2f), roadMaterial);
                Cube("Crosswalk Left " + i, WorldRoot.transform, new Vector3(-3.15f, 0.018f, z - 1.52f), new Vector3(1.0f, 0.020f, 0.10f), laneMaterial);
                Cube("Crosswalk Right " + i, WorldRoot.transform, new Vector3(3.15f, 0.018f, z + 1.52f), new Vector3(1.0f, 0.020f, 0.10f), laneMaterial);
                BuildTrafficLight(new Vector3(-4.65f, 0f, z + 2.85f), 1f, i);
                BuildTrafficLight(new Vector3(4.65f, 0f, z - 2.85f), -1f, i + 5);
                BuildDistantBuilding(new Vector3(-16.8f, 0f, z - 8f), 1.34f);
                BuildDistantBuilding(new Vector3(16.9f, 0f, z + 8f), 1.42f);
                BuildDistantBuilding(new Vector3(-27.0f, 0f, z + 15f), 1.72f);
                BuildDistantBuilding(new Vector3(27.4f, 0f, z - 15f), 1.64f);
            }
        }

        private void BuildTrafficLight(Vector3 position, float side, int index)
        {
            Cube("Traffic Light Pole " + index, WorldRoot.transform, position + new Vector3(0f, 1.18f, 0f), new Vector3(0.10f, 2.35f, 0.10f), fenceMaterial);
            Cube("Traffic Light Arm " + index, WorldRoot.transform, position + new Vector3(side * 0.48f, 2.18f, 0f), new Vector3(1.0f, 0.08f, 0.08f), fenceMaterial);
            Cube("Traffic Light Box " + index, WorldRoot.transform, position + new Vector3(side * 0.96f, 2.08f, 0f), new Vector3(0.24f, 0.56f, 0.18f), dashMaterial);
            Cube("Traffic Red " + index, WorldRoot.transform, position + new Vector3(side * 1.08f, 2.24f, -0.095f), new Vector3(0.08f, 0.08f, 0.025f), amberMaterial);
            Cube("Traffic Green " + index, WorldRoot.transform, position + new Vector3(side * 1.08f, 1.92f, -0.095f), new Vector3(0.08f, 0.08f, 0.025f), greenMaterial);
        }

        private void BuildRoadClutter()
        {
            int clutterCount = mode == GameMode.Nightmare ? 58 : 22;
            for (int i = 0; i < clutterCount; i++)
            {
                float z = SafeRoadClutterZ(55f + i * 48f + (i % 3) * 7f, i);
                float side = i % 2 == 0 ? -1f : 1f;
                float x = side * (2.35f + (i % 4) * 0.22f);

                switch (i % 5)
                {
                    case 0:
                        BuildTrafficCone(new Vector3(x, 0.03f, z));
                        BuildTrafficCone(new Vector3(x + side * 0.42f, 0.03f, z + 1.1f));
                        break;
                    case 1:
                        BuildFallenSign(new Vector3(x, 0.06f, z), side);
                        break;
                    case 2:
                        BuildRoadCrate(new Vector3(x, 0.15f, z));
                        break;
                    case 3:
                        BuildRoadTire(new Vector3(x, 0.18f, z), i * 19f);
                        break;
                    default:
                        BuildPotholePatch(new Vector3(side * 1.35f, 0.012f, z));
                        break;
                }
            }
        }

        private void BuildTrafficCone(Vector3 position)
        {
            Cylinder("Road Cone Base", WorldRoot.transform, position + new Vector3(0f, 0.04f, 0f), new Vector3(0.26f, 0.04f, 0.26f), Quaternion.identity, dashMaterial);
            Cylinder("Road Cone", WorldRoot.transform, position + new Vector3(0f, 0.29f, 0f), new Vector3(0.18f, 0.42f, 0.18f), Quaternion.identity, amberMaterial);
            Cube("Cone Stripe", WorldRoot.transform, position + new Vector3(0f, 0.34f, -0.02f), new Vector3(0.34f, 0.055f, 0.03f), laneMaterial);
        }

        private void BuildFallenSign(Vector3 position, float side)
        {
            Cube("Fallen Sign Pole", WorldRoot.transform, position + new Vector3(0f, 0.08f, 0f), new Vector3(0.08f, 0.08f, 1.55f), fenceMaterial);
            Cube("Fallen Reflector", WorldRoot.transform, position + new Vector3(side * 0.28f, 0.18f, 0.60f), new Vector3(0.72f, 0.34f, 0.06f), amberMaterial);
        }

        private float SafeRoadClutterZ(float z, int index)
        {
            int stopCount = targetStopCount + GameState.MaxMissedStops + 1;
            for (int i = 0; i < stopCount; i++)
            {
                float stopZ = RouteManager.FirstStopZ + i * routeStopInterval;
                if (Mathf.Abs(z - stopZ) < 18f)
                {
                    return z + (index % 2 == 0 ? 24f : -24f);
                }
            }

            return z;
        }

        private void BuildRoadCrate(Vector3 position)
        {
            Cube("Loose Road Crate", WorldRoot.transform, position, new Vector3(0.52f, 0.30f, 0.42f), seatMaterial);
            Cube("Crate Lid", WorldRoot.transform, position + new Vector3(0f, 0.18f, 0f), new Vector3(0.58f, 0.05f, 0.48f), fenceMaterial);
        }

        private void BuildRoadTire(Vector3 position, float yaw)
        {
            Cylinder("Discarded Tire", WorldRoot.transform, position, new Vector3(0.38f, 0.18f, 0.38f), Quaternion.Euler(90f, yaw, 0f), dashMaterial);
            Cylinder("Tire Hole", WorldRoot.transform, position, new Vector3(0.20f, 0.19f, 0.20f), Quaternion.Euler(90f, yaw, 0f), roadMaterial);
        }

        private void BuildPotholePatch(Vector3 position)
        {
            Cube("Dark Road Patch", WorldRoot.transform, position, new Vector3(1.25f, 0.018f, 0.72f), dashMaterial);
            Cube("Patch Edge Glow", WorldRoot.transform, position + new Vector3(0.42f, 0.012f, 0f), new Vector3(0.06f, 0.020f, 0.62f), laneMaterial);
        }

        private void BuildParkedCars()
        {
            int carCount = mode == GameMode.Nightmare ? 44 : 28;
            for (int i = 0; i < carCount; i++)
            {
                GameObject prefab = PickAvailablePrefab(parkedCarPrefabs, i);
                if (prefab == null)
                {
                    return;
                }

                float side = i % 2 == 0 ? -1f : 1f;
                float z = SafeRoadClutterZ(118f + i * 56f + (i % 3) * 11f, i + 73);
                float shoulderOffset = 7.4f + (i % 3) * 0.70f;
                float yaw = side < 0f ? 0f : 180f;
                Vector3 position = new Vector3(side * shoulderOffset, 0f, z);
                Material material = PickWorldMaterial(parkedCarMaterials, PrefabIndex(parkedCarPrefabs, prefab));
                GameObject car = PlaceWorldModel("Parked PSX Car " + i, prefab, WorldRoot.transform, position, new Vector3(0f, yaw, 0f), 1.04f, 4.0f, material);
                if (car != null)
                {
                    AutoAlignLongAxisToRoute(car);
                    Cube("Parked Car Shadow " + i, WorldRoot.transform, position + new Vector3(0f, 0.014f, 0f), new Vector3(3.25f, 0.020f, 1.55f), dashMaterial);
                }
            }
        }

        private void BuildRoadsideSilhouettes()
        {
            for (int i = 0; i < 9; i++)
            {
                float z = 118f + i * 92f;
                float side = i % 2 == 0 ? -1f : 1f;
                BuildRoadsideFigure(new Vector3(side * (5.85f + (i % 3) * 0.45f), 0f, z), side, i);
            }
        }

        private void BuildRoadsideFigure(Vector3 position, float side, int index)
        {
            var root = new GameObject("Roadside Figure " + index).transform;
            root.SetParent(WorldRoot.transform, false);
            root.localPosition = position;
            root.localRotation = Quaternion.Euler(0f, side < 0f ? 88f : -88f, 0f);
            Material silhouette = Material("TR Roadside Figure " + index, new Color(0.018f, 0.020f, 0.024f));
            Cube("Figure Body", root, new Vector3(0f, 0.78f, 0f), new Vector3(0.28f, 0.88f, 0.18f), silhouette);
            Sphere("Figure Head", root, new Vector3(0f, 1.34f, 0.02f), new Vector3(0.25f, 0.28f, 0.23f), silhouette);
            Cube("Figure Left Arm", root, new Vector3(-0.20f, 0.78f, 0f), new Vector3(0.08f, 0.62f, 0.08f), silhouette);
            Cube("Figure Right Arm", root, new Vector3(0.20f, 0.78f, 0f), new Vector3(0.08f, 0.62f, 0.08f), silhouette);
            Cube("Figure Eye Glint", root, new Vector3(side * -0.02f, 1.38f, 0.13f), new Vector3(0.16f, 0.035f, 0.02f), greenMaterial);
        }

        private void BuildDistantBuilding(Vector3 position, float scale)
        {
            if (TryPlaceUrbanBuilding(position, scale))
            {
                return;
            }

            if (PlaceExtracted("Roadside House", housePrefab, WorldRoot.transform, position, new Vector3(0f, position.x < 0f ? 90f : -90f, 0f), 0.28f * scale) != null)
            {
                return;
            }

            Cube("Roadside Silhouette", WorldRoot.transform, position + new Vector3(0f, 1.2f * scale, 0f), new Vector3(2.4f * scale, 2.4f * scale, 1.4f * scale), dashMaterial);
            Cube("Roadside Roof", WorldRoot.transform, position + new Vector3(0f, 2.62f * scale, 0f), new Vector3(2.8f * scale, 0.22f * scale, 1.7f * scale), busPaintMaterial);
            for (int w = 0; w < 3; w++)
            {
                Cube("Dead Window", WorldRoot.transform, position + new Vector3((-0.7f + w * 0.7f) * scale, 1.45f * scale, -0.72f * scale), new Vector3(0.22f * scale, 0.28f * scale, 0.04f * scale), greenMaterial);
            }
        }

        private bool TryPlaceUrbanBuilding(Vector3 position, float scale)
        {
            int seed = Mathf.Abs(Mathf.RoundToInt(position.z * 0.11f + position.x * 2.7f));
            GameObject prefab = PickAvailablePrefab(urbanBuildingPrefabs, seed);
            if (prefab == null)
            {
                return false;
            }

            int variant = PrefabIndex(urbanBuildingPrefabs, prefab);
            float targetHeight = variant == 0 ? 3.4f : 9.0f + (variant * 0.95f);
            float yaw = position.x < 0f ? 90f : -90f;
            Vector3 setback = new Vector3(position.x < 0f ? -1.8f : 1.8f, 0f, 0f);
            Material material = PickWorldMaterial(urbanBuildingMaterials, variant);
            return PlaceWorldModel("Urban Decay Building", prefab, WorldRoot.transform, position + setback, new Vector3(0f, yaw, 0f), targetHeight * scale, 22.0f * scale, material) != null;
        }

        private void BuildTerminalGate(float z)
        {
            Cube("Terminal Gate Left", WorldRoot.transform, new Vector3(-4.85f, 1.7f, z), new Vector3(0.36f, 3.4f, 0.55f), fenceMaterial);
            Cube("Terminal Gate Right", WorldRoot.transform, new Vector3(4.85f, 1.7f, z), new Vector3(0.36f, 3.4f, 0.55f), fenceMaterial);
            Cube("Terminal Gate Header", WorldRoot.transform, new Vector3(0f, 3.25f, z), new Vector3(9.9f, 0.36f, 0.55f), busPaintMaterial);
            Cube("Terminal Gate Glow", WorldRoot.transform, new Vector3(0f, 2.82f, z - 0.08f), new Vector3(2.8f, 0.12f, 0.08f), greenMaterial);
            Cube("Terminal Gate Stop Light", WorldRoot.transform, new Vector3(0f, 1.85f, z - 0.15f), new Vector3(0.32f, 0.32f, 0.08f), amberMaterial);
        }

        private void BuildBus()
        {
            Rig = new GameObject("Bus Driver Rig").transform;
            BusRoot = new GameObject("Bus Interior");
            BusRoot.transform.SetParent(Rig, false);

            MainCamera = new GameObject("Driver Camera").AddComponent<Camera>();
            MainCamera.transform.SetParent(Rig, false);
            MainCamera.transform.localPosition = new Vector3(0f, 1.35f, 0.35f);
            MainCamera.transform.localRotation = Quaternion.identity;
            MainCamera.fieldOfView = 62f;
            MainCamera.nearClipPlane = 0.03f;
            MainCamera.farClipPlane = 160f;
            MainCamera.clearFlags = CameraClearFlags.SolidColor;
            MainCamera.backgroundColor = new Color(0.025f, 0.032f, 0.045f);
            MainCamera.gameObject.AddComponent<AudioListener>();
            MainCamera.gameObject.AddComponent<PixelatedCameraOutput>();
            CameraTransform = MainCamera.transform;

            BuildHeadlights();
            BuildPlayerBusShell();

            ProceduralInteriorRoot = new GameObject("Procedural Bus Interior");
            ProceduralInteriorRoot.transform.SetParent(BusRoot.transform, false);

            Cube("Bus Floor", ProceduralInteriorRoot.transform, new Vector3(0f, 0f, -4.6f), new Vector3(2.9f, 0.14f, 10.8f), busMaterial);
            Cube("Bus Ceiling", ProceduralInteriorRoot.transform, new Vector3(0f, 2.45f, -4.6f), new Vector3(2.95f, 0.12f, 10.8f), busMaterial);
            Cube("Left Wall", ProceduralInteriorRoot.transform, new Vector3(-1.48f, 1.2f, -4.6f), new Vector3(0.12f, 2.35f, 10.8f), busMaterial);
            Cube("Right Wall", ProceduralInteriorRoot.transform, new Vector3(1.48f, 1.2f, -4.6f), new Vector3(0.12f, 2.35f, 10.8f), busMaterial);
            Cube("Left Window Strip", ProceduralInteriorRoot.transform, new Vector3(-1.55f, 1.55f, -4.8f), new Vector3(0.035f, 0.72f, 8.8f), glassMaterial);
            Cube("Right Window Strip", ProceduralInteriorRoot.transform, new Vector3(1.55f, 1.55f, -4.8f), new Vector3(0.035f, 0.72f, 8.8f), glassMaterial);
            Cube("Left Yellow Rail", ProceduralInteriorRoot.transform, new Vector3(-1.17f, 1.65f, -4.5f), new Vector3(0.07f, 0.07f, 8.8f), amberMaterial);
            Cube("Right Yellow Rail", ProceduralInteriorRoot.transform, new Vector3(1.17f, 1.65f, -4.5f), new Vector3(0.07f, 0.07f, 8.8f), amberMaterial);
            for (int r = 0; r < 5; r++)
            {
                float z = -1.8f - r * 1.55f;
                Cube("Left Rail Hanger " + r, ProceduralInteriorRoot.transform, new Vector3(-1.17f, 1.15f, z), new Vector3(0.06f, 1.0f, 0.06f), amberMaterial);
                Cube("Right Rail Hanger " + r, ProceduralInteriorRoot.transform, new Vector3(1.17f, 1.15f, z), new Vector3(0.06f, 1.0f, 0.06f), amberMaterial);
                Cube("Ceiling Light " + r, ProceduralInteriorRoot.transform, new Vector3(0f, 2.36f, z), new Vector3(0.75f, 0.035f, 0.18f), greenMaterial);
            }
            Cube("Windshield Top", ProceduralInteriorRoot.transform, new Vector3(0f, 2.15f, 1.25f), new Vector3(3.1f, 0.24f, 0.18f), busMaterial);
            Cube("Windshield Bottom", ProceduralInteriorRoot.transform, new Vector3(0f, 0.75f, 1.25f), new Vector3(3.1f, 0.20f, 0.18f), busMaterial);
            Cube("Windshield Left Pillar", ProceduralInteriorRoot.transform, new Vector3(-1.45f, 1.45f, 1.24f), new Vector3(0.16f, 1.55f, 0.16f), busMaterial);
            Cube("Windshield Right Pillar", ProceduralInteriorRoot.transform, new Vector3(1.45f, 1.45f, 1.24f), new Vector3(0.16f, 1.55f, 0.16f), busMaterial);
            Cube("Mirror Frame", ProceduralInteriorRoot.transform, new Vector3(0f, 1.98f, 0.92f), new Vector3(1.35f, 0.28f, 0.08f), dashMaterial);
            frontDoorPanel = Cube("Front Door Panel", ProceduralInteriorRoot.transform, new Vector3(1.53f, 1.16f, 0.36f), new Vector3(0.055f, 1.38f, 0.56f), glassMaterial).transform;
            rearDoorPanel = Cube("Rear Door Panel", ProceduralInteriorRoot.transform, new Vector3(1.53f, 1.16f, -0.24f), new Vector3(0.055f, 1.38f, 0.56f), glassMaterial).transform;
            SetDoorsOpen(false);

            Cube("Dashboard", ProceduralInteriorRoot.transform, new Vector3(0f, 0.72f, 0.72f), new Vector3(2.8f, 0.35f, 0.75f), dashMaterial);
            Cube("Driver Console Face", ProceduralInteriorRoot.transform, new Vector3(0.58f, 0.92f, 0.72f), new Vector3(0.7f, 0.12f, 0.52f), busPaintMaterial);
            Cylinder("Steering Column", ProceduralInteriorRoot.transform, new Vector3(0f, 0.84f, 0.35f), new Vector3(0.12f, 0.5f, 0.12f), Quaternion.Euler(65f, 0f, 0f), dashMaterial);
            SteeringWheel = Cylinder("Steering Wheel", ProceduralInteriorRoot.transform, new Vector3(0f, 1.03f, 0.14f), new Vector3(0.72f, 0.06f, 0.72f), Quaternion.Euler(75f, 0f, 0f), dashMaterial).transform;

            BuildSeatsAndPassengers();
            BuildMonkey();
            Ball = Sphere("Football", BusRoot.transform, ballStart, new Vector3(0.36f, 0.36f, 0.36f), ballMaterial);
            Ball.SetActive(false);
        }

        private void BuildSeatsAndPassengers()
        {
            for (int row = 0; row < 11; row++)
            {
                float z = -2.1f - row * 0.72f;
                BuildSeat(new Vector3(-0.72f, 0.45f, z));
                BuildSeat(new Vector3(0.72f, 0.45f, z));
                Passengers.Add(BuildPassenger(new Vector3(-0.72f, 0.95f, z - 0.04f), row, row * 2));
                Passengers.Add(BuildPassenger(new Vector3(0.72f, 0.95f, z - 0.04f), row, row * 2 + 1));
            }
        }

        private void BuildSeat(Vector3 position)
        {
            Cube("Seat Base", ProceduralInteriorRoot.transform, position, new Vector3(0.55f, 0.18f, 0.45f), seatMaterial);
            Cube("Seat Back", ProceduralInteriorRoot.transform, position + new Vector3(0f, 0.38f, -0.16f), new Vector3(0.55f, 0.65f, 0.12f), seatMaterial);
        }

        private PassengerVisual BuildPassenger(Vector3 position, int row, int passengerIndex)
        {
            PassengerVisual assetPassenger = BuildAssetPassenger(position, row, passengerIndex);
            if (assetPassenger != null)
            {
                return assetPassenger;
            }

            var root = new GameObject("Passenger " + row).transform;
            root.SetParent(BusRoot.transform, false);
            root.localPosition = position;

            var bodyMat = Material("TR Passenger Cloth " + passengerIndex, Color.Lerp(new Color(0.10f, 0.11f, 0.14f), new Color(0.24f, 0.20f, 0.14f), (passengerIndex % 5) / 4f));
            Cube("Passenger Body", root, new Vector3(0f, -0.10f, -0.02f), new Vector3(0.34f, 0.62f, 0.24f), bodyMat);
            Cube("Passenger Left Arm", root, new Vector3(-0.24f, -0.08f, 0.02f), new Vector3(0.10f, 0.48f, 0.10f), bodyMat);
            Cube("Passenger Right Arm", root, new Vector3(0.24f, -0.08f, 0.02f), new Vector3(0.10f, 0.48f, 0.10f), bodyMat);
            Cube("Passenger Left Leg", root, new Vector3(-0.10f, -0.48f, 0.15f), new Vector3(0.12f, 0.42f, 0.13f), dashMaterial);
            Cube("Passenger Right Leg", root, new Vector3(0.10f, -0.48f, 0.15f), new Vector3(0.12f, 0.42f, 0.13f), dashMaterial);
            var headMat = new Material(skinMaterial);
            var head = Sphere("Passenger Head", root, new Vector3(0f, 0.42f, 0.03f), new Vector3(0.32f, 0.36f, 0.30f), headMat).transform;
            var fallbackLeftEye = Cube("Left Eye", head, new Vector3(-0.065f, 0.02f, 0.145f), new Vector3(0.035f, 0.035f, 0.012f), greenMaterial);
            var fallbackRightEye = Cube("Right Eye", head, new Vector3(0.065f, 0.02f, 0.145f), new Vector3(0.035f, 0.035f, 0.012f), greenMaterial);

            return new PassengerVisual(root, head, fallbackLeftEye, fallbackRightEye, headMat);
        }

        private PassengerVisual BuildAssetPassenger(Vector3 position, int row, int passengerIndex)
        {
            GameObject prefab = passengerPrefabs.Length == 0 ? null : passengerPrefabs[passengerIndex % passengerPrefabs.Length];
            if (prefab == null)
            {
                return null;
            }

            var root = new GameObject("Passenger " + row + " Asset").transform;
            root.SetParent(BusRoot.transform, false);
            root.localPosition = position + new Vector3(0f, -0.86f, 0.16f);
            root.localRotation = Quaternion.identity;

            var model = Object.Instantiate(prefab, root);
            model.name = "Passenger Model";
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
            RemoveColliders(model);
            DisableAnimationComponents(model);
            PoseSeated(model.transform, passengerIndex);
            FitModelToHeight(model.transform, root, 1.16f);

            Transform head = FindNamedTransform(model.transform, "head") ?? model.transform;
            var eyeAnchor = new GameObject("Stare Eye Anchor").transform;
            eyeAnchor.SetParent(root, false);
            eyeAnchor.localPosition = new Vector3(0f, 1.06f, 0.15f);
            var leftEye = Cube("Left Eye", eyeAnchor, new Vector3(-0.055f, 0f, 0f), new Vector3(0.035f, 0.035f, 0.012f), greenMaterial);
            var rightEye = Cube("Right Eye", eyeAnchor, new Vector3(0.055f, 0f, 0f), new Vector3(0.035f, 0.035f, 0.012f), greenMaterial);

            return new PassengerVisual(root, head, leftEye, rightEye, null, false);
        }

        private bool BuildWorldCharacterModel(string name, Transform parent, GameObject prefab, float targetHeight, bool closeThreatPose, int poseVariant = 0)
        {
            if (prefab == null)
            {
                return false;
            }

            var model = Object.Instantiate(prefab, parent);
            model.name = name;
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one;
            RemoveColliders(model);
            DisableAnimationComponents(model);

            if (closeThreatPose)
            {
                PoseCloseThreat(model.transform);
            }
            else
            {
                PoseStanding(model.transform, poseVariant);
            }

            FitModelToHeight(model.transform, parent, targetHeight);
            foreach (var renderer in model.GetComponentsInChildren<Renderer>(true))
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }

            return true;
        }

        private void BuildMonkey()
        {
            Monkey = new GameObject("O Macaco");
            Monkey.transform.SetParent(BusRoot.transform, false);
            if (scaryMonkeyMaterial.mainTexture != null)
            {
                TreeCard("Scary Monkey Cutout", Monkey.transform, new Vector3(0f, 0.08f, 0f), new Vector2(1.04f, 1.56f), 0f, scaryMonkeyMaterial);
            }
            else
            {
                Cube("Monkey Body", Monkey.transform, new Vector3(0f, -0.12f, 0f), new Vector3(0.38f, 0.62f, 0.24f), monkeyMaterial);
                Sphere("Monkey Head", Monkey.transform, new Vector3(0f, 0.36f, 0.05f), new Vector3(0.42f, 0.38f, 0.32f), monkeyMaterial);
                Cube("Monkey Face", Monkey.transform, new Vector3(0f, 0.35f, 0.23f), new Vector3(0.24f, 0.14f, 0.035f), skinMaterial);
                Cube("Monkey Left Eye", Monkey.transform, new Vector3(-0.07f, 0.42f, 0.255f), new Vector3(0.04f, 0.04f, 0.015f), greenMaterial);
                Cube("Monkey Right Eye", Monkey.transform, new Vector3(0.07f, 0.42f, 0.255f), new Vector3(0.04f, 0.04f, 0.015f), greenMaterial);
            }

            SetMonkeyRow(10);
            Monkey.SetActive(false);
        }

        private void BuildPlayerBusShell()
        {
            PlayerBusShell = PlaceExtracted("Player Bus Shell", busPrefab, Rig, new Vector3(0f, -0.32f, -5.2f), new Vector3(0f, 90f, 0f), 1.18f);
            if (PlayerBusShell == null)
            {
                return;
            }

            AutoAlignLongAxisToRoute(PlayerBusShell);
            foreach (var renderer in PlayerBusShell.GetComponentsInChildren<Renderer>(true))
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
            }

            PlayerBusShell.SetActive(false);
        }

        private void BuildHeadlights()
        {
            BuildHeadlight("Left Headlight", new Vector3(-0.82f, 0.62f, 1.35f));
            BuildHeadlight("Right Headlight", new Vector3(0.82f, 0.62f, 1.35f));
            BuildHeadlight("Windshield Fill", new Vector3(0f, 1.15f, 1.1f), 2.0f, 52f, 44f);
        }

        private void AddOncomingHeadlights(Transform bus)
        {
            AddOncomingHeadlight(bus, new Vector3(-0.62f, 0.56f, -2.25f));
            AddOncomingHeadlight(bus, new Vector3(0.62f, 0.56f, -2.25f));
        }

        private void AddOncomingHeadlight(Transform bus, Vector3 localPosition)
        {
            var lightObject = new GameObject("Oncoming Headlight");
            lightObject.transform.SetParent(bus, false);
            lightObject.transform.localPosition = localPosition;
            lightObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Spot;
            light.color = new Color(1f, 0.88f, 0.66f);
            light.intensity = 3.2f;
            light.range = 48f;
            light.spotAngle = 42f;
            light.shadows = LightShadows.None;
        }

        private void BuildHeadlight(string name, Vector3 localPosition)
        {
            BuildHeadlight(name, localPosition, 7.4f, 62f, 145f);
        }

        private void BuildHeadlight(string name, Vector3 localPosition, float intensity, float spotAngle, float range)
        {
            var lightObject = new GameObject(name);
            lightObject.transform.SetParent(Rig, false);
            lightObject.transform.localPosition = localPosition;
            lightObject.transform.localRotation = Quaternion.identity;

            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Spot;
            light.color = new Color(0.80f, 0.92f, 1.00f);
            light.intensity = intensity;
            light.range = range;
            light.spotAngle = spotAngle;
            light.shadows = LightShadows.None;
        }

        private void BuildLamp(Vector3 position)
        {
            if (PlaceExtracted("Road Lamp Asset", lamppostPrefab, WorldRoot.transform, position, new Vector3(0f, 90f, 0f), 1f) != null)
            {
                AddRoadLampLight(position + new Vector3(-0.86f, 2.7f, 0f));
                return;
            }

            Cube("Road Lamp Pole", WorldRoot.transform, position + new Vector3(0f, 1.6f, 0f), new Vector3(0.11f, 3.2f, 0.11f), fenceMaterial);
            Cube("Road Lamp Arm", WorldRoot.transform, position + new Vector3(-0.42f, 3.12f, 0f), new Vector3(0.9f, 0.08f, 0.08f), fenceMaterial);
            Cube("Road Lamp Glow", WorldRoot.transform, position + new Vector3(-0.86f, 2.95f, 0f), new Vector3(0.22f, 0.16f, 0.22f), amberMaterial);

            AddRoadLampLight(position + new Vector3(-0.86f, 2.7f, 0f));
        }

        private void AddRoadLampLight(Vector3 localPosition)
        {
            var lightObject = new GameObject("Road Lamp Light");
            lightObject.transform.SetParent(WorldRoot.transform, false);
            lightObject.transform.localPosition = localPosition;
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(0.95f, 0.48f, 0.14f);
            light.intensity = 0.55f;
            light.range = 7f;
        }

        private static void PoseSeated(Transform model, int poseVariant)
        {
            int variant = Mathf.Abs(poseVariant) % 4;
            bool leanLeft = variant == 0 || variant == 2;

            RotateBone(model, "hips", new Vector3(0f, leanLeft ? -3f : 3f, leanLeft ? -3f : 3f));
            RotateBone(model, "leftupleg", new Vector3(-82f, 2f, -8f));
            RotateBone(model, "rightupleg", new Vector3(-82f, -2f, 8f));
            RotateBone(model, "leftleg", new Vector3(86f, 0f, 0f));
            RotateBone(model, "rightleg", new Vector3(86f, 0f, 0f));
            RotateBone(model, "spine", new Vector3(variant == 3 ? 9f : 4f, 0f, leanLeft ? 4f : -4f));
            RotateBone(model, "neck", new Vector3(leanLeft ? -2f : 2f, 0f, leanLeft ? -3f : 3f));
            RotateBone(model, "head", new Vector3(leanLeft ? -3f : 2f, 0f, leanLeft ? -4f : 4f));

            switch (variant)
            {
                case 1:
                    RotateBone(model, "leftarm", new Vector3(24f, -4f, -82f));
                    RotateBone(model, "rightarm", new Vector3(44f, -6f, 64f));
                    RotateBone(model, "leftforearm", new Vector3(8f, 0f, -48f));
                    RotateBone(model, "rightforearm", new Vector3(8f, 0f, 78f));
                    break;
                case 2:
                    RotateBone(model, "leftarm", new Vector3(48f, 8f, -66f));
                    RotateBone(model, "rightarm", new Vector3(28f, -6f, 82f));
                    RotateBone(model, "leftforearm", new Vector3(10f, 0f, -76f));
                    RotateBone(model, "rightforearm", new Vector3(4f, 0f, 42f));
                    break;
                case 3:
                    RotateBone(model, "leftarm", new Vector3(32f, 0f, -74f));
                    RotateBone(model, "rightarm", new Vector3(32f, 0f, 74f));
                    RotateBone(model, "leftforearm", new Vector3(8f, 0f, -64f));
                    RotateBone(model, "rightforearm", new Vector3(8f, 0f, 64f));
                    break;
                default:
                    RotateBone(model, "leftarm", new Vector3(28f, 6f, -78f));
                    RotateBone(model, "rightarm", new Vector3(28f, -6f, 78f));
                    RotateBone(model, "leftforearm", new Vector3(6f, 0f, -54f));
                    RotateBone(model, "rightforearm", new Vector3(6f, 0f, 54f));
                    break;
            }
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

        private static void PoseCloseThreat(Transform model)
        {
            RotateBone(model, "leftarm", new Vector3(58f, 0f, -72f));
            RotateBone(model, "rightarm", new Vector3(58f, 0f, 72f));
            RotateBone(model, "leftforearm", new Vector3(20f, 0f, -58f));
            RotateBone(model, "rightforearm", new Vector3(20f, 0f, 58f));
            RotateBone(model, "spine", new Vector3(-7f, 0f, 0f));
            RotateBone(model, "head", new Vector3(-4f, 0f, 0f));
        }

        private static void RotateBone(Transform root, string token, Vector3 localEuler)
        {
            Transform bone = FindNamedTransform(root, token);
            if (bone != null)
            {
                bone.localRotation *= Quaternion.Euler(localEuler);
            }
        }

        private static void FitModelToHeight(Transform model, Transform root, float targetHeight)
        {
            Bounds bounds;
            if (!TryGetBounds(model.gameObject, out bounds) || bounds.size.y <= 0.001f)
            {
                model.localScale = Vector3.one * 0.72f;
                return;
            }

            float scale = targetHeight / bounds.size.y;
            model.localScale *= scale;

            if (TryGetBounds(model.gameObject, out bounds))
            {
                Vector3 desiredBottom = root.TransformPoint(Vector3.zero);
                model.position += desiredBottom - new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
            }
        }

        private static bool TryGetBounds(GameObject root, out Bounds bounds)
        {
            Renderer[] renderers = root.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                bounds = default;
                return false;
            }

            bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return true;
        }

        private static void AutoAlignLongAxisToRoute(GameObject root)
        {
            Bounds bounds;
            if (!TryGetBounds(root, out bounds))
            {
                return;
            }

            if (bounds.size.x > bounds.size.z * 1.18f)
            {
                root.transform.localRotation *= Quaternion.Euler(0f, 90f, 0f);
            }
        }

        private static Transform FindNamedTransform(Transform root, string token)
        {
            token = token.ToLowerInvariant();
            foreach (var transform in root.GetComponentsInChildren<Transform>(true))
            {
                string name = transform.name.ToLowerInvariant().Replace("_", "").Replace(" ", "").Replace(":", "");
                if (name.Contains(token))
                {
                    return transform;
                }
            }

            return null;
        }

        private static GameObject ExtractedPrefab(string resourceName, string editorPath)
        {
            var prefab = Resources.Load<GameObject>("TerminalRoute/Extracted/" + resourceName);
#if UNITY_EDITOR
            if (prefab == null)
            {
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(editorPath);
            }
#endif

            return prefab;
        }

        private static GameObject CharacterPrefab(string resourceName, string editorPath)
        {
            var prefab = Resources.Load<GameObject>("TerminalRoute/Characters/" + resourceName);
#if UNITY_EDITOR
            if (prefab == null)
            {
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(editorPath);
            }
#endif

            return prefab;
        }

        private static GameObject WorldPrefab(string resourcePath, string editorPath)
        {
            var prefab = Resources.Load<GameObject>(resourcePath);
#if UNITY_EDITOR
            if (prefab == null)
            {
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(editorPath);
            }
#endif

            return prefab;
        }

        private static GameObject PickAvailablePrefab(GameObject[] prefabs, int seed)
        {
            if (prefabs == null || prefabs.Length == 0)
            {
                return null;
            }

            int start = Mathf.Abs(seed) % prefabs.Length;
            for (int offset = 0; offset < prefabs.Length; offset++)
            {
                GameObject prefab = prefabs[(start + offset) % prefabs.Length];
                if (prefab != null)
                {
                    return prefab;
                }
            }

            return null;
        }

        private static int PrefabIndex(GameObject[] prefabs, GameObject prefab)
        {
            if (prefabs == null)
            {
                return 0;
            }

            for (int i = 0; i < prefabs.Length; i++)
            {
                if (prefabs[i] == prefab)
                {
                    return i;
                }
            }

            return 0;
        }

        private static Material PickWorldMaterial(Material[] materials, int index)
        {
            if (materials == null || materials.Length == 0)
            {
                return null;
            }

            index = Mathf.Clamp(index, 0, materials.Length - 1);
            return materials[index];
        }

        private static GameObject PlaceExtracted(string name, GameObject prefab, Transform parent, Vector3 localPosition, Vector3 localEulerAngles, float uniformScale, Material overrideMaterial = null)
        {
            if (prefab == null)
            {
                return null;
            }

            var gameObject = Object.Instantiate(prefab, parent);
            gameObject.name = name;
            gameObject.transform.localPosition = localPosition;
            gameObject.transform.localRotation = Quaternion.Euler(localEulerAngles);
            gameObject.transform.localScale = Vector3.one * uniformScale;
            RemoveColliders(gameObject);
            if (overrideMaterial != null)
            {
                foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>(true))
                {
                    renderer.sharedMaterial = overrideMaterial;
                }
            }

            return gameObject;
        }

        private static GameObject PlaceWorldModel(string name, GameObject prefab, Transform parent, Vector3 localPosition, Vector3 localEulerAngles, float targetHeight, float maxFootprint, Material overrideMaterial = null)
        {
            if (prefab == null)
            {
                return null;
            }

            var gameObject = Object.Instantiate(prefab, parent);
            gameObject.name = name;
            gameObject.transform.localPosition = localPosition;
            gameObject.transform.localRotation = Quaternion.Euler(localEulerAngles);
            gameObject.transform.localScale = Vector3.one;
            RemoveColliders(gameObject);
            DisableAnimationComponents(gameObject);
            ApplyMaterialOverride(gameObject, overrideMaterial);
            NormalizeModelBounds(gameObject, parent, localPosition, targetHeight, maxFootprint);
            return gameObject;
        }

        private static void ApplyMaterialOverride(GameObject gameObject, Material material)
        {
            if (material == null)
            {
                return;
            }

            foreach (var renderer in gameObject.GetComponentsInChildren<Renderer>(true))
            {
                renderer.sharedMaterial = material;
            }
        }

        private static void NormalizeModelBounds(GameObject gameObject, Transform parent, Vector3 localPosition, float targetHeight, float maxFootprint)
        {
            if (!TryGetRendererBounds(gameObject, out Bounds bounds))
            {
                return;
            }

            if (bounds.size.y > 0.001f)
            {
                float heightScale = Mathf.Clamp(targetHeight / bounds.size.y, 0.001f, 100f);
                gameObject.transform.localScale *= heightScale;
            }

            if (maxFootprint > 0f && TryGetRendererBounds(gameObject, out bounds))
            {
                float footprint = Mathf.Max(bounds.size.x, bounds.size.z);
                if (footprint > maxFootprint && footprint > 0.001f)
                {
                    gameObject.transform.localScale *= Mathf.Clamp(maxFootprint / footprint, 0.001f, 1f);
                }
            }

            if (TryGetRendererBounds(gameObject, out bounds))
            {
                float targetWorldY = parent != null ? parent.TransformPoint(localPosition).y : localPosition.y;
                gameObject.transform.position += Vector3.up * (targetWorldY - bounds.min.y);
            }
        }

        private static bool TryGetRendererBounds(GameObject root, out Bounds bounds)
        {
            var renderers = root.GetComponentsInChildren<Renderer>(true);
            bounds = new Bounds(root.transform.position, Vector3.zero);
            bool found = false;

            foreach (var renderer in renderers)
            {
                if (!renderer.enabled)
                {
                    continue;
                }

                if (!found)
                {
                    bounds = renderer.bounds;
                    found = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return found;
        }

        private static void RemoveColliders(GameObject root)
        {
            foreach (var collider in root.GetComponentsInChildren<Collider>(true))
            {
                Object.Destroy(collider);
            }
        }

        private static void DisableAnimationComponents(GameObject root)
        {
            foreach (var animator in root.GetComponentsInChildren<Animator>(true))
            {
                animator.enabled = false;
            }

            foreach (var animation in root.GetComponentsInChildren<Animation>(true))
            {
                animation.enabled = false;
            }
        }

        private static Material ResourceMaterial(string resourceName, string fallbackName, Color fallbackColor)
        {
            var material = Resources.Load<Material>("TerminalRoute/Materials/" + resourceName);
#if UNITY_EDITOR
            if (material == null)
            {
                var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorTexturePath(resourceName));
                if (texture != null)
                {
                    material = Material(fallbackName, fallbackColor);
                    material.mainTexture = texture;
                }
            }
#endif

            material = material != null ? material : Material(fallbackName, fallbackColor);
            if (resourceName == "TreeBillboard")
            {
                EnableAlphaClip(material, 0.22f);
            }

            return material;
        }

#if UNITY_EDITOR
        private static string EditorTexturePath(string resourceName)
        {
            switch (resourceName)
            {
                case "RoadAsphalt":
                    return "Assets/TerminalRoute/AssetPacks/Bus_stop/Texture/Asphalt.jpg";
                case "RoadConcrete":
                    return "Assets/TerminalRoute/AssetPacks/Bus_stop/Texture/Concrete.png";
                case "Grass":
                    return "Assets/TerminalRoute/AssetPacks/Bus_stop/Texture/Grass.jpg";
                case "Fence":
                    return "Assets/TerminalRoute/AssetPacks/Bus_stop/Texture/Fence.png";
                case "TreeBillboard":
                    return "Assets/TerminalRoute/AssetPacks/Bus_stop/Texture/Tree_01.png";
                case "BusInterior":
                    return "Assets/TerminalRoute/AssetPacks/Bus_stop/Texture/Floor.jpg";
                case "SeatVinyl":
                    return "Assets/TerminalRoute/AssetPacks/Bus_stop/Texture/Seating.png";
                case "WindowGlass":
                    return "Assets/TerminalRoute/AssetPacks/Bus_stop/Texture/Glass.png";
                case "BusPaint":
                    return "Assets/TerminalRoute/AssetPacks/Bus_stop/Texture/Bus.png";
                default:
                    return "";
            }
        }
#endif

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

        private static Material TexturedWorldMaterial(string name, string resourcePath, Color color, bool alphaClip)
        {
            Material material = Material(name, color);
            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (texture != null)
            {
                texture.filterMode = FilterMode.Point;
                texture.wrapMode = TextureWrapMode.Clamp;
                material.mainTexture = texture;
                if (material.HasProperty("_BaseMap"))
                {
                    material.SetTexture("_BaseMap", texture);
                }
            }

            if (alphaClip)
            {
                EnableAlphaClip(material, 0.32f);
            }

            return material;
        }

        private static Material TexturedBillboardMaterial(string name, string resourcePath, Color color)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
            {
                shader = Shader.Find("Unlit/Texture");
            }

            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            var material = new Material(shader);
            material.name = name;
            material.color = color;

            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (texture != null)
            {
                texture.filterMode = FilterMode.Point;
                texture.wrapMode = TextureWrapMode.Clamp;
                material.mainTexture = texture;
                if (material.HasProperty("_BaseMap"))
                {
                    material.SetTexture("_BaseMap", texture);
                }
            }

            if (material.HasProperty("_Cull"))
            {
                material.SetInt("_Cull", 0);
            }

            EnableTransparentMaterial(material);

            return material;
        }

        private static void EnableTransparentMaterial(Material material)
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

            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = 3000;
        }

        private static void EnableAlphaClip(Material material, float cutoff)
        {
            if (material == null)
            {
                return;
            }

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

        private static GameObject Cube(string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
        {
            return Primitive(PrimitiveType.Cube, name, parent, localPosition, localScale, Quaternion.identity, material);
        }

        private static GameObject Sphere(string name, Transform parent, Vector3 localPosition, Vector3 localScale, Material material)
        {
            return Primitive(PrimitiveType.Sphere, name, parent, localPosition, localScale, Quaternion.identity, material);
        }

        private static GameObject Cylinder(string name, Transform parent, Vector3 localPosition, Vector3 localScale, Quaternion localRotation, Material material)
        {
            return Primitive(PrimitiveType.Cylinder, name, parent, localPosition, localScale, localRotation, material);
        }

        private static GameObject TreeCard(string name, Transform parent, Vector3 localPosition, Vector2 size, float yaw, Material material)
        {
            return Primitive(PrimitiveType.Quad, name, parent, localPosition, new Vector3(size.x, size.y, 1f), Quaternion.Euler(0f, yaw, 0f), material);
        }

        private static GameObject Primitive(PrimitiveType type, string name, Transform parent, Vector3 localPosition, Vector3 localScale, Quaternion localRotation, Material material)
        {
            var gameObject = GameObject.CreatePrimitive(type);
            gameObject.name = name;
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localPosition = localPosition;
            gameObject.transform.localRotation = localRotation;
            gameObject.transform.localScale = localScale;

            var renderer = gameObject.GetComponent<Renderer>();
            renderer.sharedMaterial = material;

            var collider = gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                Object.Destroy(collider);
            }

            return gameObject;
        }
    }
}
