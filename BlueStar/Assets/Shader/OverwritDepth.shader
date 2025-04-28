Shader "Custom/OverrideStencil"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _StencilRef("Stencil Reference Value", Range(0,255)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode"="UniversalForward"}

            // 开启深度测试和深度写入
            ZTest LEqual
            ZWrite On

            // 设置模板测试和模板操作
            Stencil
            {
                Ref 1     // 使用传入的模板参考值
                Comp always           // 始终通过模板测试
                Pass replace          // 将模板缓冲区的值替换为指定的参考值
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _StencilRef;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float4 posCS = TransformObjectToHClip(IN.positionOS);

                OUT.positionCS = posCS;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                return _BaseColor;
            }
            ENDHLSL
        }
    }
}