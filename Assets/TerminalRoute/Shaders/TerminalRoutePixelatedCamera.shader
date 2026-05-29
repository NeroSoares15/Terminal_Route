Shader "TerminalRoute/PixelatedCamera"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelResolution ("Pixel Resolution", Float) = 224
        _ScanlineStrength ("Scanline Strength", Range(0, 1)) = 0.12
        _VignetteStrength ("Vignette Strength", Range(0, 1)) = 0.28
        _Aberration ("Aberration", Range(0, 0.01)) = 0.0018
        _ForceOpaque ("Force Opaque", Float) = 0
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

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _PixelResolution;
            float _ScanlineStrength;
            float _VignetteStrength;
            float _Aberration;
            float _ForceOpaque;

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
                float aspect = max(_ScreenParams.x / max(_ScreenParams.y, 1.0), 0.1);
                float2 grid = float2(max(_PixelResolution * aspect, 16.0), max(_PixelResolution, 16.0));
                float2 uv = (floor(i.uv * grid) + 0.5) / grid;

                float2 fromCenter = uv - 0.5;
                float aberration = _Aberration * saturate(dot(fromCenter, fromCenter) * 2.6);
                fixed4 center = tex2D(_MainTex, uv);
                fixed r = tex2D(_MainTex, uv + float2(aberration, 0)).r;
                fixed g = center.g;
                fixed b = tex2D(_MainTex, uv - float2(aberration, 0)).b;
                fixed4 color = fixed4(r, g, b, lerp(center.a, 1.0, saturate(_ForceOpaque)));

                float scan = 1.0 - _ScanlineStrength * step(0.5, frac(i.uv.y * _ScreenParams.y * 0.5));
                float vignette = smoothstep(0.92, 0.28, length(fromCenter));
                color.rgb *= scan;
                color.rgb *= lerp(1.0 - _VignetteStrength, 1.0, vignette);
                color.rgb = floor(color.rgb * 32.0) / 32.0;
                return color * i.color;
            }
            ENDCG
        }
    }
}
