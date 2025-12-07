Shader "Custom/Grid500_URP"
{
    Properties
    {
        _CellSize ("Cell Size (m)", Float) = 0.5
        _LineColor ("Line Color", Color) = (0.7,0.7,0.7,1)
        _BackgroundColor ("Background Color", Color) = (1,1,1,1)
        _Thickness ("Line Thickness", Float) = 0.02
        _GridOffset ("Grid Offset (X,Z)", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "GridPass"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float _CellSize;
            float4 _LineColor;
            float4 _BackgroundColor;
            float _Thickness;

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                float3 world = TransformObjectToWorld(v.positionOS);
                o.worldPos = world;
                o.positionHCS = TransformWorldToHClip(world);
                return o;
            }
            
            float4 _GridOffset; // (x, z) 사용
            float4 frag(Varyings i) : SV_Target
            {
                // float2 p = i.worldPos.xz / _CellSize;
                float2 p = (i.worldPos.xz - _GridOffset.xy) / _CellSize;

                float gx = abs(frac(p.x) - 0.5);
                float gz = abs(frac(p.y) - 0.5);

                float lineMask  = step(gx, _Thickness) + step(gz, _Thickness);

                return lerp(_BackgroundColor, _LineColor, lineMask);
            }

            ENDHLSL
        }
    }
}
