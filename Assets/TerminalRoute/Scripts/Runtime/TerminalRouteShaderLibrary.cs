using UnityEngine;

namespace TerminalRoute.Runtime
{
    internal static class TerminalRouteShaderLibrary
    {
        private const string PixelShaderName = "TerminalRoute/PixelatedCamera";
        private const string VhsShaderResourcePath = "TerminalRoute/Shaders/TerminalRouteVhsOverlay";
        private const string VhsShaderName = "TerminalRoute/VhsOverlay";

        public static Shader PixelatedCamera()
        {
            var shader = Shader.Find(PixelShaderName);
            if (shader == null)
            {
                Debug.LogWarning("Terminal Route VHS pixel shader was not found. Check ProjectSettings/GraphicsSettings Always Included Shaders.");
            }

            return shader;
        }

        public static Shader VhsOverlay()
        {
            var shader = Resources.Load<Shader>(VhsShaderResourcePath);
            if (shader == null)
            {
                shader = Shader.Find(VhsShaderName);
            }

            if (shader == null)
            {
                Debug.LogWarning("Terminal Route VHS overlay shader was not found in Resources.");
            }

            return shader;
        }
    }
}
