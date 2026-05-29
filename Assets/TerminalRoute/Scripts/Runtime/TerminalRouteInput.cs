using UnityEngine;

namespace TerminalRoute.Runtime
{
    public static class TerminalRouteInput
    {
        public static bool StartPressed()
        {
            return Input.GetKeyDown(KeyCode.Return) ||
                Input.GetKeyDown(KeyCode.KeypadEnter) ||
                Input.GetKeyDown(KeyCode.Space);
        }

        public static bool CancelPressed()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }

        public static bool MirrorPressed()
        {
            return Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(1);
        }

        public static float Steering()
        {
            float input = 0f;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                input -= 1f;
            }

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                input += 1f;
            }

            return Mathf.Clamp(input, -1f, 1f);
        }
    }
}
