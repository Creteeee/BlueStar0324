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
            Tags { "LightMode"="UniversalForward" }
            Blend DstAlpha One

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            
            float4 _BaseMap_ST;
            float4 _BaseColor;
            float _HealthRatio;
            float _Alpha;

            Varyings vert (Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionHCS = TransformWorldToHClip(positionWS);
                output.uv =ComputeScreenPos(output.positionHCS);
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                float2 screenPos = input.uv.xy / input.uv.w; 
                half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, screenPos);
                float bloodMask = texColor.r;
                float ramp = texColor.g;
                float threshold = smoothstep(0,saturate(_HealthRatio),ramp);
                float4 color = float4(_BaseColor.rgb,1);
                color.a = bloodMask*threshold;
                return color.a*color*_BaseColor.a*_Alpha;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
