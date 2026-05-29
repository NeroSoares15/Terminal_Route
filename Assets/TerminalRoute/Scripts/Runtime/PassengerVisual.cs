using UnityEngine;

namespace TerminalRoute.Runtime
{
    public sealed class PassengerVisual
    {
        private readonly Transform head;
        private readonly GameObject leftEye;
        private readonly GameObject rightEye;
        private readonly Material headMaterial;
        private readonly Color normalColor;
        private readonly Quaternion normalHeadRotation;
        private readonly bool useEyeMarkers;

        public PassengerVisual(Transform root, Transform head, GameObject leftEye, GameObject rightEye, Material headMaterial, bool useEyeMarkers = true)
        {
            Root = root;
            this.head = head;
            this.leftEye = leftEye;
            this.rightEye = rightEye;
            this.headMaterial = headMaterial;
            this.useEyeMarkers = useEyeMarkers;
            normalColor = headMaterial != null ? headMaterial.color : Color.white;
            normalHeadRotation = head != null ? head.localRotation : Quaternion.identity;
            SetStaring(false);
        }

        public Transform Root { get; }

        public void SetPresent(bool present)
        {
            Root.gameObject.SetActive(present);
        }

        public void SetStaring(bool staring)
        {
            leftEye.SetActive(useEyeMarkers && staring);
            rightEye.SetActive(useEyeMarkers && staring);
            if (head != null)
            {
                head.localRotation = staring ? normalHeadRotation * Quaternion.Euler(0f, 180f, 0f) : normalHeadRotation;
            }

            if (headMaterial != null)
            {
                headMaterial.color = staring ? new Color(0.95f, 0.92f, 0.78f, 1f) : normalColor;
            }
        }
    }
}
