Shader "Unlit/Face"
{
    Properties
    {
        _MainColor("MainColor",Color)=(1,1,1,1)
        _DarkColor("DarkColor",Color)=(0,0,0,1)
    	_MainTex("MainTex",2D)="white"{}
        _RimColor("RimColor",Color)=(1,1,1,1)
        _StepEdge("StepEdge",float)=10
        [Toggle(_AdditionalLights)]_AddLights ("AddLights", Int) = 1
    	[Toggle(_isShadow)]_isShadow ("isShadow", Int) = 1
        [Toggle(_isRimLight)]_isRimLight("RimLight",Int)=1
        _offset("Offset",float)=0.5
        _threshold("threshold",float)=0.1
        _RimIntensity("RimIntensity",float)=0.1
    	_shadowThreshold1("ShadowThreshold1",float)=0.3
    	_Ramp1("Ramp1",2D)="white"{}
    	_Ramp2("Ramp2",2D)="white"{}
    	_shadowThreshold2("ShadowThreshold2",float)=0.15
    	_ShadowMask("ShadowMask",2D)="white"{}
        
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
             
        }
        LOD 100
        
        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

        CBUFFER_START(UnityPerMaterial)
         float4 _MainColor;
         float4 _DarkColor;
         float4 _MainTex_ST;
         float4 _RimColor;
         float _StepEdge;
         float  _offset;
         float _threshold;
         float _RimIntensity;
         float _isRimLight;
         float _shadowThreshold1;
         float _shadowThreshold2;
         float4 _Ramp1_ST;
         float4 _Ramp2_ST;
         float4 _ShadowMask_ST;
        CBUFFER_END
		TEXTURE2D(_MainTex);
		SAMPLER(sampler_MainTex);
		TEXTURE2D(_Ramp1);
		SAMPLER(sampler_Ramp1);
        TEXTURE2D(_Ramp2);
		SAMPLER(sampler_Ramp2);
		TEXTURE2D(_ShadowMask);
		SAMPLER(sampler_ShadowMask);        
        
        ENDHLSL
        
        Pass
        {
        	Cull Off
            Tags{"LightMode"="UniversalForward"}
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _AdditionalLights
            #pragma shader_feature _isShadow
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ Anti_Aliasing_ON
            #pragma multi_compile _SoftShadow_ON

            float ToonLambert(float HalfLambert)
        {
            float ToonLambert = lerp(0,1,floor(HalfLambert*3)/3)+1;
        	
            return ToonLambert;
        }
            
            
        struct a2v
        {
            float4 posOS:POSITION;
            float3 normalOS:NORMAL;
            float2 uv:TEXCOORD0;
            
        };

        struct v2f
        {
            float4 posCS:SV_POSITION;
            float2 uv:TEXCOORD0;
            float3 normalWS:TEXCOORD1;
            float3 posWS:TEXCOORD2;
            float3 viewDirWS:TEXCOORD3;
            float4 posNDC:TEXCOORD4;
            float3 normalVS:TEXCOORD5;
            float3 posVS:TEXCOORD6;
            float4 shadowCoord: TEXCOORD7;            
            
        };
            v2f vert(a2v i)
            {
                v2f o;
                //o.posCS = TransformObjectToHClip(i.posOS);
                o.normalWS = TransformObjectToWorldNormal(i.normalOS);
                o.uv = i.uv;
                o.posWS = TransformObjectToWorld(i.posOS.xyz);
                o.viewDirWS = GetCameraPositionWS()-o.posWS;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(i.posOS.xyz);
                o.posCS = vertexInput.positionCS;
                o.posVS = vertexInput.positionVS;
                o.posNDC = vertexInput.positionNDC;
                o.normalVS = normalize(TransformWorldToViewNormal(i.normalOS,true));
                o.shadowCoord= TransformWorldToShadowCoord(o.posWS);
                return o;
            }
            float4 frag(v2f i):SV_Target
            {
                //计算主光源
                float3 posWS = i.posWS;
            	float2 uv = i.uv;
            	float4 baseColor = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,uv*_MainTex_ST.xy);
                float3 normalWS = normalize(i.normalWS);
                float4 posCS = i.posCS;
                float3 normalVS = i.normalVS;
                float3 MainlightDirWS = normalize(GetMainLight().direction);//GetMainLight返回的是Light类型的物体
                //主光源阴影
                float shadowAttenuation = MainLightRealtimeShadow(i.shadowCoord);
            	#ifdef _isShadow
                float HalfLambertMainLight = ((dot(normalWS,MainlightDirWS))*0.5+0.5)*shadowAttenuation;
            	 float LambertMainLight = saturate((dot(normalWS,MainlightDirWS)))*shadowAttenuation;
            	#else
            	float HalfLambertMainLight = ((dot(normalWS,MainlightDirWS))*0.5+0.5);
            	float LambertMainLight = saturate(dot(normalWS,MainlightDirWS));
            	#endif
                float3 viewDirWS = i.viewDirWS;
                float3 halfDirMainLight = normalize(MainlightDirWS+normalize(viewDirWS));
                float specularMainLight = pow(max(0,dot(halfDirMainLight,normalWS)),20);
            	float3 mainlightColor = GetMainLight().color;
                specularMainLight = step(1,specularMainLight);
                

                //计算额外灯光(没加阴影开关）
                float addLightIntensity = 0.f;
                float specularAddLight = 0.f;
            	float3 addLightColor = (0,0,0);
            	float minDistance =0;
                #ifdef _AdditionalLights
                    int addLightsCount = GetAdditionalLightsCount();
                    for(int i=0;i<addLightsCount;i++)
                    {
                        Light addLight = GetAdditionalLight(i,posWS);
                        float3 addlightDirWS = normalize(addLight.direction);
                        float3 halfDir = normalize(addLight.direction+viewDirWS);
                        specularAddLight += pow(max(0,dot(halfDir,normalWS)),20);
                        addLightIntensity +=(dot(normalWS,addlightDirWS)*0.5+0.5)*addLight.distanceAttenuation*addLight.shadowAttenuation*10;
							
                    	if(addLightColor.r <=1 && addLightColor.g <=1 && addLightColor.b <=1)
						{
							addLightColor +=addLight.color;
						}
                    	                        
                    }
                #else
                addLightIntensity = 0;
                specularAddLight = 0;
            	addLightColor = (0,0,0,1);
                
                #endif
                specularAddLight = step(1,specularAddLight);

                //计算全局半兰伯特
                float HalfLambert = max(HalfLambertMainLight,clamp(addLightIntensity,0,1));
            	float Lambert =max(LambertMainLight,clamp(addLightIntensity,0,1));
            	//HalfLambert=smoothstep(_shadowThreshold,_shadowThreshold+0.1,HalfLambert);

                //获取物体的相机空间法线偏移并加到屏幕坐标
               
                float2 screenUV = float2(posCS.x/_ScreenParams.x,posCS.y/_ScreenParams.y);
                float2 RimOffsetUV = _offset*normalVS.xy*0.01f/posCS.w;
                RimOffsetUV +=screenUV;
                

                //获得目前屏幕深度和偏移深度
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,sampler_CameraDepthTexture,screenUV).r;
                float offsetDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,sampler_CameraDepthTexture,RimOffsetUV).r;

                //线性化深度
                float linearEyeDepth = LinearEyeDepth(depth,_ZBufferParams);                
                float linearEyeOffsetDepth = LinearEyeDepth(offsetDepth,_ZBufferParams);

                //深度差
                float depthDiff = linearEyeOffsetDepth - linearEyeDepth;
                float rim = step(_threshold,depthDiff)*_RimIntensity;



                //获得深浅Mask

            	
				float CartoonLambert = ToonLambert(HalfLambert)+specularAddLight+specularMainLight;
            	float SmoothLambert = HalfLambert+specularAddLight+specularMainLight;

				//计算shadow颜色
            	float2 rampUV1=float2(SmoothLambert,0);
            	float4 Ramp1_Color =  SAMPLE_TEXTURE2D(_Ramp1,sampler_Ramp1,rampUV1*_Ramp1_ST.xy);
            	float4 Ramp2_Color =  SAMPLE_TEXTURE2D(_Ramp2,sampler_Ramp2,rampUV1*_Ramp2_ST.xy);
            	//阴影的范围
            	float shadowArea1 = saturate(smoothstep(_shadowThreshold1,_shadowThreshold1+0.1,SmoothLambert)+0.2);
            	_shadowThreshold2=clamp(_shadowThreshold2,_shadowThreshold2,_shadowThreshold1);
            	float shadowArea2 = saturate(smoothstep(_shadowThreshold2,_shadowThreshold2+0.05,SmoothLambert)+0.2);
            	//剔除不需要阴影的部分
            	float shadowMask =SAMPLE_TEXTURE2D(_ShadowMask,sampler_ShadowMask,uv*_ShadowMask_ST.xy).r;
				shadowArea1 += (1-shadowMask);
            	shadowArea1 =saturate(shadowArea1);
            	shadowArea2 +=(1-shadowMask);
				shadowArea2 =saturate(shadowArea2);

                float ColorMask = step(_StepEdge,CartoonLambert);
                float4 finalMainColor =ColorMask*(_MainColor*0.8)+(1-ColorMask)*_DarkColor;
            	

            	//加上shadow颜色
            	float4 baseColor_Ramped1 = lerp(Ramp1_Color*baseColor,baseColor,shadowArea1);
            	float4 baseColor_Ramped2 =lerp(baseColor_Ramped1*0.8+Ramp2_Color*0.05,baseColor_Ramped1,shadowArea2);
            	finalMainColor = baseColor_Ramped2*1+float4(mainlightColor+addLightColor*0.005,1)*0.4;
            	
                float4 finalColor = _isRimLight*rim*_RimColor+(1-rim*_isRimLight)*finalMainColor;
               return finalColor;
            	
                
            }

            
            ENDHLSL
           
        }
        Pass
        {

			Name "ShadowCast"
 
			Tags{ "LightMode" = "ShadowCaster" }
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
				struct appdata
			{
				float4 vertex : POSITION;
			};
 
			struct v2f
			{
				float4 pos : SV_POSITION;
			};
			
 
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}
			float4 frag(v2f i) : SV_Target
			{
				float4 color;
				color.xyz = float3(0.0, 0.0, 0.0);
				return color;
			}
			ENDHLSL


        }
    }

    FallBack "Universal Render Pipeline/Lit"
    }
