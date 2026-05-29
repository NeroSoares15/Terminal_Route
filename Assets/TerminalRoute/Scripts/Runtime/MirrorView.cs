using System;
using UnityEngine;

namespace TerminalRoute.Runtime
{
    public sealed class MirrorView
    {
        private readonly Transform cameraTransform;
        private readonly TerminalRouteUi ui;

        public MirrorView(Transform cameraTransform, TerminalRouteUi ui)
        {
            this.cameraTransform = cameraTransform;
            this.ui = ui;
        }

        public event Action Activated;
        public event Action Deactivated;

        public bool IsActive { get; private set; }

        public void Reset()
        {
            IsActive = false;
            cameraTransform.localRotation = Quaternion.identity;
            ui.SetMirror(false);
        }

        public void Tick(float deltaTime)
        {
            if (TerminalRouteInput.MirrorPressed())
            {
                SetActive(!IsActive);
            }
        }

        private void SetActive(bool active)
        {
            if (IsActive == active)
            {
                return;
            }

            IsActive = active;
            cameraTransform.localRotation = active ? Quaternion.Euler(0f, 180f, 0f) : Quaternion.identity;
            ui.SetMirror(active);

            if (active)
            {
                Activated?.Invoke();
            }
            else
            {
                Deactivated?.Invoke();
            }
        }
    }
}
