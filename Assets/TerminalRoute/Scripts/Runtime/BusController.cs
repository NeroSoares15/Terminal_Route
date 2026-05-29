using TerminalRoute.Core;
using UnityEngine;

namespace TerminalRoute.Runtime
{
    public sealed class BusController
    {
        private const float SafeHalfWidth = 2.75f;
        private const float GuardrailHalfWidth = 3.55f;
        private const float GuardrailFailureSeconds = 1.15f;

        private readonly Transform rig;
        private readonly Transform steeringWheel;
        private readonly Quaternion steeringWheelBaseRotation;
        private float guardrailTimer;
        private float wheelAngle;
        private float currentSpeed;
        private bool paused;

        public BusController(Transform rig, Transform steeringWheel)
        {
            this.rig = rig;
            this.steeringWheel = steeringWheel;
            steeringWheelBaseRotation = steeringWheel != null ? steeringWheel.localRotation : Quaternion.identity;
        }

        public float WorldZ
        {
            get { return rig.position.z; }
        }

        public float LateralPosition
        {
            get { return rig.position.x; }
        }

        public float CurrentRouteSpeed
        {
            get { return paused ? 0f : currentSpeed; }
        }

        public float DisplaySpeed
        {
            get { return paused ? 0f : currentSpeed * 3.55f; }
        }

        public float RoadDanger
        {
            get
            {
                float edgeDanger = Mathf.InverseLerp(SafeHalfWidth, GuardrailHalfWidth, Mathf.Abs(rig.position.x));
                return Mathf.Clamp01(Mathf.Max(edgeDanger, guardrailTimer / GuardrailFailureSeconds));
            }
        }

        public float SteeringVisual
        {
            get { return Mathf.Clamp(wheelAngle / -68f, -1f, 1f); }
        }

        public void Reset()
        {
            rig.position = Vector3.zero;
            rig.rotation = Quaternion.identity;
            guardrailTimer = 0f;
            wheelAngle = 0f;
            currentSpeed = 12f;
            paused = false;
            UpdateSteeringWheel(0f, 1f);
        }

        public void SetPaused(bool paused)
        {
            this.paused = paused;
        }

        public void SnapToStop(float lateralPosition, float worldZ)
        {
            Vector3 position = rig.position;
            position.x = Mathf.Clamp(lateralPosition, -GuardrailHalfWidth, GuardrailHalfWidth);
            position.z = worldZ;
            rig.position = position;
        }

        public void Tick(float deltaTime, GameState state, bool invertSteering)
        {
            if (state.Phase != GamePhase.Driving || paused)
            {
                return;
            }

            var profile = SanitySystem.Evaluate(state.Sanity);
            float input = TerminalRouteInput.Steering();
            if (invertSteering)
            {
                input *= -1f;
            }
            UpdateSteeringWheel(input, deltaTime);
            float difficulty = Mathf.Clamp01((state.Loop - 1) / 4f);
            currentSpeed = 11.6f + difficulty * 3.2f + profile.VisualDistortion * 1.8f;
            float drift = Mathf.Sin(Time.time * (1.35f + difficulty * 0.65f)) * (profile.DriftStrength * 1.65f + difficulty * 0.34f);
            float roadCrown = -rig.position.x * (0.22f + difficulty * 0.08f);

            Vector3 position = rig.position;
            float desiredX = position.x + ((input * 6.2f * profile.ControlMultiplier) + drift + roadCrown) * deltaTime;
            bool pressingIntoGuardrail = Mathf.Abs(desiredX) > GuardrailHalfWidth && Mathf.Sign(desiredX) == Mathf.Sign(input == 0f ? desiredX : input);
            position.x = Mathf.Clamp(desiredX, -GuardrailHalfWidth, GuardrailHalfWidth);
            position.z += currentSpeed * deltaTime;
            rig.position = position;

            float absX = Mathf.Abs(position.x);
            if (absX > SafeHalfWidth)
            {
                float danger = Mathf.InverseLerp(SafeHalfWidth, GuardrailHalfWidth, absX);
                state.ApplySanityDelta(-deltaTime * Mathf.Lerp(4f, 22f, danger));

                if (pressingIntoGuardrail || absX >= GuardrailHalfWidth - 0.02f)
                {
                    guardrailTimer += deltaTime * Mathf.Lerp(0.65f, 1.45f, danger);
                    position.x = Mathf.Sign(position.x) * (GuardrailHalfWidth - 0.06f);
                    rig.position = position;
                }
                else
                {
                    guardrailTimer = Mathf.Max(0f, guardrailTimer - deltaTime * 1.2f);
                }

                if (guardrailTimer >= GuardrailFailureSeconds)
                {
                    state.ResolveEnding(EndingId.LongRoute, EndingCause.RoadCrash);
                }
            }
            else
            {
                guardrailTimer = Mathf.Max(0f, guardrailTimer - deltaTime * 2f);
            }
        }

        private void UpdateSteeringWheel(float input, float deltaTime)
        {
            if (steeringWheel == null)
            {
                return;
            }

            wheelAngle = Mathf.Lerp(wheelAngle, input * -68f, Mathf.Clamp01(deltaTime * 12f));
            steeringWheel.localRotation = steeringWheelBaseRotation * Quaternion.Euler(0f, wheelAngle, 0f);
        }
    }
}
