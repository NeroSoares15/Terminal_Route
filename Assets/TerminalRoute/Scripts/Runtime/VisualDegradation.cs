using TerminalRoute.Core;
using UnityEngine;

namespace TerminalRoute.Runtime
{
    public sealed class VisualDegradation
    {
        private readonly Transform cameraTransform;
        private readonly TerminalRouteUi ui;
        private readonly Vector3 baseCameraPosition = new Vector3(0f, 1.35f, 0.35f);

        public VisualDegradation(Transform cameraTransform, TerminalRouteUi ui)
        {
            this.cameraTransform = cameraTransform;
            this.ui = ui;
        }

        public void Reset()
        {
            cameraTransform.localPosition = baseCameraPosition;
            ui.SetVisualDistortion(0f);
            ui.SetLightsOut(0f);
        }

        public void Tick(float deltaTime, GameState state, bool mirrorActive)
        {
            Tick(deltaTime, state, mirrorActive, 0f);
        }

        public void Tick(float deltaTime, GameState state, bool mirrorActive, float lightsOutAmount)
        {
            var profile = SanitySystem.Evaluate(state.Sanity);
            float shake = profile.VisualDistortion * 0.055f + lightsOutAmount * 0.035f;
            float x = (Mathf.PerlinNoise(Time.time * 8f, 0.1f) - 0.5f) * shake;
            float y = (Mathf.PerlinNoise(0.2f, Time.time * 7f) - 0.5f) * shake;
            Vector3 targetPosition = mirrorActive ? new Vector3(0f, 1.55f, 0.20f) : baseCameraPosition;
            cameraTransform.localPosition = targetPosition + new Vector3(x, y, 0f);
            ui.SetVisualDistortion(profile.VisualDistortion);
            ui.SetLightsOut(lightsOutAmount);
        }
    }
}
