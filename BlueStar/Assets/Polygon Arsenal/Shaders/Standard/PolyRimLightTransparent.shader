Shader "PolygonArsenal/PolyRimLightURP"
{
    Properties
    {
        _InnerColor("Inner Color", Color) = (1,1,1,1)
        _RimColor("Rim Color", Color) = (0.26, 0.19, 0.16, 0)
        _RimWidth("Rim Width", Range(0.2, 20.0)) = 3.0
        _RimGlow("Rim Glow Multiplier", Range(0.0, 9.0)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="UniversalForward" }

            Cull Back
            Blend One One
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD0;
                float3 viewDirWS : TEXCOORD1;
            };

            float4 _InnerColor;
            float4 _RimColor;
            float _RimWidth;
            float _RimGlow;

            Varyings vert (Attributes v)
            {
                Varyings o;
                float3 positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.positionHCS = TransformWorldToHClip(positionWS);
                o.normalWS = normalize(TransformObjectToWorldNormal(v.normalOS));
                o.viewDirWS = normalize(_WorldSpaceCameraPos - positionWS);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float rim = 1.0 - saturate(dot(i.viewDirWS, i.normalWS));
                float3 emission = _RimColor.rgb * _RimGlow * pow(rim, _RimWidth);
                return half4(_InnerColor.rgb + emission, _InnerColor.a);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
