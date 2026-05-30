Shader "TerminalRoute/VhsOverlay"
{
    Properties
    {
        _Intensity ("Intensity", Range(0, 1)) = 0.50
        _ScanlineAlpha ("Scanline Alpha", Range(0, 1)) = 0.055
        _NoiseAlpha ("Noise Alpha", Range(0, 1)) = 0.010
        _VignetteAlpha ("Vignette Alpha", Range(0, 1)) = 0.16
        _LineCount ("Line Count", Float) = 430
        _TrackingSpeed ("Tracking Speed", Float) = 0.10
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Overlay"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            float _Intensity;
            float _ScanlineAlpha;
            float _NoiseAlpha;
            float _VignetteAlpha;
            float _LineCount;
            float _TrackingSpeed;

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float time = _Time.y;

                float scanMask = step(0.52, frac(uv.y * _LineCount));
                float scanAlpha = scanMask * _ScanlineAlpha;

                float noise = hash21(float2(floor(uv.x * 240.0), floor(uv.y * 160.0) + floor(time * 12.0)));
                float grainAlpha = noise * _NoiseAlpha;

                float trackingY = frac(time * _TrackingSpeed);
                float tracking = smoothstep(0.010, 0.0, abs(uv.y - trackingY)) * 0.018;

                float2 centered = uv - 0.5;
                centered.x *= _ScreenParams.x / max(_ScreenParams.y, 1.0);
                float edge = smoothstep(0.46, 0.96, length(centered));
                float vignetteAlpha = edge * _VignetteAlpha;

                float alpha = saturate((scanAlpha + grainAlpha + tracking + vignetteAlpha) * _Intensity);
                fixed3 color = fixed3(0.0, 0.0, 0.0);
                return fixed4(color, alpha) * i.color;
            }
            ENDCG
        }
    }
}
