Shader "Unlit/M_Preview_Normal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma shader_feature _ Preview_Normal

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 smoothNormal : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.smoothNormal = v.tangent.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col;

                UNITY_APPLY_FOG(i.fogCoord, col);
                #ifdef Preview_Normal
                    col = float4(i.smoothNormal,1);
                #else
                    col = tex2D(_MainTex, i.uv);
                #endif
                
                return col;
            }
            ENDCG
        }
    }
}
