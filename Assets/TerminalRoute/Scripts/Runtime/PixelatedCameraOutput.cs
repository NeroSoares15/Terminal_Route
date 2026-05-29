using UnityEngine;
using UnityEngine.UI;

namespace TerminalRoute.Runtime
{
    [RequireComponent(typeof(Camera))]
    public sealed class PixelatedCameraOutput : MonoBehaviour
    {
        public int Width = 426;
        public int Height = 240;
        public float PixelResolution = 220f;
        public float ScanlineStrength = 0.08f;
        public float VignetteStrength = 0.16f;
        public float Aberration = 0.0011f;

        private Camera sourceCamera;
        private RenderTexture renderTexture;
        private Material material;

        private void Awake()
        {
            sourceCamera = GetComponent<Camera>();
            CreateRenderTexture();
            CreateDisplayCamera();
            CreateOutputCanvas();
        }

        private void OnDestroy()
        {
            if (sourceCamera != null)
            {
                sourceCamera.targetTexture = null;
            }

            if (renderTexture != null)
            {
                renderTexture.Release();
                Destroy(renderTexture);
            }

            if (material != null)
            {
                Destroy(material);
            }
        }

        private void CreateRenderTexture()
        {
            renderTexture = new RenderTexture(Width, Height, 16, RenderTextureFormat.ARGB32);
            renderTexture.name = "Terminal Route Low Res Camera";
            renderTexture.filterMode = FilterMode.Point;
            renderTexture.wrapMode = TextureWrapMode.Clamp;
            renderTexture.antiAliasing = 1;
            renderTexture.Create();
            sourceCamera.targetTexture = renderTexture;
        }

        private void CreateDisplayCamera()
        {
            var cameraObject = new GameObject("Terminal Route Display Camera");
            var displayCamera = cameraObject.AddComponent<Camera>();
            displayCamera.clearFlags = CameraClearFlags.SolidColor;
            displayCamera.backgroundColor = Color.black;
            displayCamera.cullingMask = 0;
            displayCamera.depth = -100f;
            displayCamera.nearClipPlane = 0.03f;
            displayCamera.farClipPlane = 1f;
        }

        private void CreateOutputCanvas()
        {
            var canvasObject = new GameObject("Terminal Route Pixel Output");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = -100;
            canvasObject.AddComponent<CanvasScaler>();

            var imageObject = new GameObject("Pixelated Camera Image");
            imageObject.transform.SetParent(canvasObject.transform, false);
            var rect = imageObject.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = imageObject.AddComponent<RawImage>();
            image.texture = renderTexture;
            image.color = Color.white;
            image.raycastTarget = false;

            Shader shader = Shader.Find("TerminalRoute/PixelatedCamera");
            if (shader == null)
            {
                return;
            }

            material = new Material(shader);
            material.SetFloat("_PixelResolution", PixelResolution);
            material.SetFloat("_ScanlineStrength", ScanlineStrength);
            material.SetFloat("_VignetteStrength", VignetteStrength);
            material.SetFloat("_Aberration", Aberration);
            material.SetFloat("_ForceOpaque", 1f);
            image.material = material;
        }
    }
}
