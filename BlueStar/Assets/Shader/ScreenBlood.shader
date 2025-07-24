Shader "Unlit/ScreenBlood"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,0,0,1)
        _BaseMap ("Base Map", 2D) = "white" {}//血液图
        _HealthRatio("_HealthRatio",Range(0,1))=0.5
        _Alpha("Alpha",Range(0,1))=0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline"="UniversalRenderPipeline" }
        LOD 100

        Pass
        {
            Name "Unlit"
            Tags {"Queue"="Transparent" "LightMode"="UniversalForward" }
            Blend DstAlpha One
            ZTest Always
            Zwrite Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #if SHADER_API_GLES
                struct Attributes
                {
                    float4 posOS       : POSITION;
                    float2 uv0          : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };
            #else
            struct Attributes
            {
                uint vertexID : SV_VertexID;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            #endif
            


            struct Varyings
            {
                float4 posCS : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            
            float4 _BaseMap_ST;
            float4 _BaseColor;
            float _HealthRatio;
            float _Alpha;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.posCS = GetFullScreenTriangleVertexPosition(input.vertexID);
                output.uv0 = GetFullScreenTriangleTexCoord(input.vertexID);
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                float2 screenPos = input.uv0; 
                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, screenPos);
                float bloodMask = texColor.r;
                float ramp = texColor.g;
                float threshold = smoothstep(0,saturate(_HealthRatio-0.2),ramp);
                float4 color = float4(_BaseColor.rgb,1);
                color.a = bloodMask*threshold;
                //return float4(1,1,1,1);
                return color.a*color*_BaseColor.a*_Alpha;
            }
            ENDHLSL
        }
    }

   // FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
