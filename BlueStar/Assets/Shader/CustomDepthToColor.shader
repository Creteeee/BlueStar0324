Shader "Custom/StencilBasedPostProcess"
{
    Properties
    {
        _BaseColor1("Base Color 1", Color) = (1, 0, 0, 1)  // 红色
        _BaseColor2("Base Color 2", Color) = (0, 1, 0, 1)  // 绿色
        _TopColor ("Top Color", Color) = (0.5, 0.5, 1.0, 1.0)  // 顶部颜色
        _BottomColor ("Bottom Color", Color) = (1.0, 1.0, 1.0, 1.0)  // 底部颜色
        _Exponent ("Exponent", Float) = 1.0  // 控制渐变强度
        _Edge1("Edge1",float)=0.2
        _Edge2("Edge2",float)=0.3
    }
    SubShader
    {
        Tags { "Queue" = "Overlay" }

        Pass
        {
            Name "StencilTest_Zero"  // Pass 1：处理模板值为0的物体
            Stencil
            {
                Ref 0           // 设置模板参考值为0
                Comp equal      // 检查模板值是否为0
                Pass Keep
                Fail Keep
                ZFail Keep
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            float4 _TopColor;
            float4 _BottomColor;
            float _Exponent;
            float _Edge1;
            float _Edge2;

            struct Attributes
            {
                float4 positionOS : POSITION;
                
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 worldDir : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor1;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float4 posCS = TransformObjectToHClip(IN.positionOS);

                OUT.positionCS = posCS;
                OUT.worldDir=normalize(mul(unity_ObjectToWorld, IN.positionOS).xyz);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Calculate the vertical factor based on the Y component of the world direction
                float verticalFactor = pow(saturate(IN.worldDir.y * 0.5 + 0.5), _Exponent);
                verticalFactor = smoothstep(_Edge1,_Edge2,verticalFactor);
                // Interpolate between the top and bottom colors
                half4 color = lerp(_BottomColor, _TopColor, verticalFactor);

             
                return _BaseColor1;  // 渲染红色
            }
            ENDHLSL
        }

        Pass
        {
            Name "StencilTest_OneAndAbove"
            Stencil
            {
                Ref 1           // 设置模板参考值为1
                Comp equal      // 检查模板值是否为1
                Pass Keep
                Fail Keep
                ZFail Keep
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float4 _TopColor;
            float4 _BottomColor;
            float _Exponent;
            float _Edge1;
            float _Edge2;

            struct Attributes
            {
                float3 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION; // 屏幕空间位置
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor2;
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
                // 获取屏幕空间的Y坐标（归一化设备坐标）
                float screenSpaceY = IN.positionCS.y / IN.positionCS.w;

                // 基于屏幕空间Y坐标计算渐变因子
                float verticalFactor = pow(saturate(screenSpaceY), _Exponent);
                verticalFactor = smoothstep(_Edge1, _Edge2, verticalFactor);

                // 在顶部和底部颜色之间进行插值
                half4 color = lerp(_BottomColor, _TopColor, verticalFactor);

                return half4(color.rgb, 1.0); // 渲染渐变颜色
            }
            ENDHLSL
        }
    }
}
