using TerminalRoute.Core;
using UnityEngine;

namespace TerminalRoute.Runtime
{
    public sealed class RoadHazardManager
    {
        private readonly Vector3[] hazardPositions;
        private readonly bool[] hitHazards;
        private float alertTimer;

        public RoadHazardManager()
        {
            hazardPositions = new Vector3[22];
            hitHazards = new bool[hazardPositions.Length];

            for (int i = 0; i < hazardPositions.Length; i++)
            {
                float z = 55f + i * 48f + (i % 3) * 7f;
                float side = i % 2 == 0 ? -1f : 1f;
                float x = i % 5 == 4 ? side * 1.35f : side * (2.35f + (i % 4) * 0.22f);
                hazardPositions[i] = new Vector3(x, 0f, z);
            }
        }

        public string WarningText
        {
            get { return alertTimer > 0f ? "OBJETO NA ESTRADA" : ""; }
        }

        public void Reset()
        {
            for (int i = 0; i < hitHazards.Length; i++)
            {
                hitHazards[i] = false;
            }

            alertTimer = 0f;
        }

        public void Tick(float deltaTime, GameState state, BusController bus, TerminalRouteAudio audio)
        {
            alertTimer = Mathf.Max(0f, alertTimer - deltaTime);
            if (state.Phase != GamePhase.Driving)
            {
                return;
            }

            for (int i = 0; i < hazardPositions.Length; i++)
            {
                if (hitHazards[i])
                {
                    continue;
                }

                float forwardDistance = Mathf.Abs(bus.WorldZ - hazardPositions[i].z);
                if (forwardDistance > 2.1f)
                {
                    continue;
                }

                float lateralDistance = Mathf.Abs(bus.LateralPosition - hazardPositions[i].x);
                if (lateralDistance > 0.72f)
                {
                    continue;
                }

                hitHazards[i] = true;
                alertTimer = 1.4f;
                state.ApplySanityDelta(-7f);
                audio.PlayHazardHit();
                return;
            }
        }
    }
}
