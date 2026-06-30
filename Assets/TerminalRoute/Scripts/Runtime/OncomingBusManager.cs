using System.Collections.Generic;
using TerminalRoute.Core;
using UnityEngine;

namespace TerminalRoute.Runtime
{
    public sealed class OncomingBusManager
    {
        private const float SpawnX = -1.9f;
        private const float SpawnDistance = 105f;
        private const float OncomingSpeed = 18f;
        private const float CollisionLateralDistance = 1.25f;
        private const float CollisionForwardDistance = 3.6f;

        private readonly SceneFactory scene;
        private readonly Transform parent;
        private readonly List<GameObject> activeBuses = new List<GameObject>();
        private float spawnTimer;
        private bool nightmareMode;

        public OncomingBusManager(SceneFactory scene)
        {
            this.scene = scene;
            parent = new GameObject("Oncoming Bus Runtime").transform;
        }

        public void Reset()
        {
            Reset(GameState.CreateNewRun());
        }

        public void Reset(GameState state)
        {
            for (int i = 0; i < activeBuses.Count; i++)
            {
                if (activeBuses[i] != null)
                {
                    Object.Destroy(activeBuses[i]);
                }
            }

            activeBuses.Clear();
            nightmareMode = state.IsNightmare;
            spawnTimer = Random.Range(nightmareMode ? 4f : 10f, nightmareMode ? 8f : 16f);
        }

        public void Tick(float deltaTime, GameState state, BusController playerBus)
        {
            if (state.Phase == GamePhase.Ended)
            {
                return;
            }

            for (int i = activeBuses.Count - 1; i >= 0; i--)
            {
                GameObject bus = activeBuses[i];
                if (bus == null)
                {
                    activeBuses.RemoveAt(i);
                    continue;
                }

                Vector3 position = bus.transform.position;
                position.z -= (nightmareMode ? 25f : OncomingSpeed) * deltaTime;
                bus.transform.position = position;

                if (IsCollision(playerBus.LateralPosition, playerBus.WorldZ, position.x, position.z))
                {
                    state.ResolveEnding(EndingId.LongRoute, EndingCause.OncomingBusCrash);
                    return;
                }

                if (position.z < playerBus.WorldZ - 22f)
                {
                    Object.Destroy(bus);
                    activeBuses.RemoveAt(i);
                }
            }

            if (state.Phase != GamePhase.Driving)
            {
                return;
            }

            spawnTimer -= deltaTime;
            if (spawnTimer > 0f)
            {
                return;
            }

            Spawn(playerBus);
            spawnTimer = Random.Range(nightmareMode ? 7f : 18f, nightmareMode ? 13f : 28f);
        }

        public static bool IsCollision(float playerX, float playerZ, float busX, float busZ)
        {
            return Mathf.Abs(playerX - busX) < CollisionLateralDistance &&
                   Mathf.Abs(playerZ - busZ) < CollisionForwardDistance;
        }

        private void Spawn(BusController playerBus)
        {
            GameObject bus = scene.BuildOncomingBus(parent);
            if (bus == null)
            {
                return;
            }

            float spawnDistance = nightmareMode ? 128f : SpawnDistance;
            bus.transform.position = new Vector3(SpawnX, 0f, playerBus.WorldZ + spawnDistance);
            activeBuses.Add(bus);
        }
    }
}
