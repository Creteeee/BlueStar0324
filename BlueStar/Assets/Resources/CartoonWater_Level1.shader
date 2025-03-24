Shader "Unlit/CartoonWater_Level1"
{
	Properties{
		_MainTex("MainTex",2D) = "white"{}
		_ShallowColor("Shallow Color", Color) = (1,1,1,1)
		_DeepColor("Deep Color", Color) = (1,1,1,1)
		_FoamColor("Foam Color",Color) = (1,1,1,1)
		_DepthMaxDistance("_DepthMaxDistance",float)=1.0
		_NoiseTex("Noise Texture",2D) ="white"{}
		_NoiseCutoff("Noise Cutoff",Range(0,1))=0.5
		_FoamMaxDistance("Foam MaxDistance", float)=0.4
		_FoamMinDistance("Foam MinDistance", float)=0.04
		_SurfaceNoiseScroll("Surface Noise Scroll",vector)=(1,1,1,1)
		_SurfaceDistortion("Surface Distortion",2D)="white"{}
		_SurfaceDistortionAmount("Surface Distortion Amount",range(0,1)) =0.27
		[Toggle(_EnableRipples)]_EnableRipples("Istrue", float) = 0
	}
	SubShader
	{
		Tags{
			"RenderPipeLine"="UniversalRenderPipeline"
			"Queue" = "Transparent"	
			"LightMode" = "UniversalForward"
	
		}



		Pass
		{

			Blend One OneMinusSrcAlpha
			ZWrite Off
	

			HLSLPROGRAM
            #define  SMOOTHSTEP_AA 0.01
            
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x


			#pragma vertex vert
			#pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"        
            // #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            // #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"    //没有这个库下面就要声明dethtexture
			CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_ST;
				float4 _NoiseTex_ST;
				float4 _SurfaceDistortion_ST;
				float4 _ShallowColor;
				float4 _DeepColor;
				float4 _FoamColor;
				float _DepthMaxDistance;
				float _NoiseCutoff;
				float _FoamMaxDistance;
				float _FoamMinDistance;
				float2 _SurfaceNoiseScroll;	
				float _SurfaceDistortionAmount;
				float _OrthographicCamSize;
				float3 _Position1;
				float _EnableRipples;
				


			CBUFFER_END

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_Tex);
			TEXTURE2D(_NoiseTex);
			SAMPLER(sampler_NoiseTex);
			TEXTURE2D(_CameraDepthTexture); 
			SAMPLER(sampler_CameraDepthTexture);
			TEXTURE2D(_SurfaceDistortion);
			SAMPLER(sampler_SurfaceDistortion);
			TEXTURE2D(_CameraNormalsTexture); 
			SAMPLER(sampler_CameraNormalsTexture);
			TEXTURE2D(_GlobalEffectRT1);
			SAMPLER(sampler_GlobalEffectRT1);


			struct a2v{
				float3 positionOS: POSITION;
				float2 uv: TEXCOORD0;
				float3 normalOS: NORMAL;

			};
			struct v2f{
				float4 positionCS: SV_POSITION;
				float4 uv: TEXCOORD0;
				float4 screenPos: TEXCOORD1;
				float3 viewNormal:TEXCOORD2;
				float3 worldPos:TEXCOORD3;
				float3 posWS:TEXCOORD4;
				float3 viewDirWS:TEXCOORD5;
				float3 normalWS:TEXCOORD6;
			};

			//浮沫颜色，去除颜色叠加效果影响
			float4 alphaBlend(float4 top, float4 bottom){
				float3 color =(top.rgb*top.a+bottom.rgb*(1-top.a));
				/* float alpha =top.a + max(bottom.a,(1-top.a));  *///这里本来用的是乘法不过也没关系
				float alpha = top.a + bottom.a*(1-top.a);
				return float4(color, alpha);
				}
				
			v2f vert(a2v v)
			{
				v2f o;
				// float wave = SAMPLE_TEXTURE2D(_NoiseTex,sampler_NoiseTex,v.uv);
				// v.positionOS.y +=wave;
				
				o.positionCS = TransformObjectToHClip(v.positionOS);
				o.screenPos.xy = o.positionCS.xy*0.5+o.positionCS.w*0.5;
				o.screenPos.zw = o.positionCS.zw;
				o.uv.xy = v.uv;
				o.uv.zw = v.uv;
				o.posWS = TransformObjectToWorld(v.positionOS.xyz);
                o.viewDirWS = GetCameraPositionWS()-o.posWS;
				o.normalWS = TransformObjectToWorldNormal(v.normalOS);



				o.viewNormal = TransformWorldToViewDir(v.normalOS);
				o.worldPos = TransformObjectToWorld(v.positionOS);

				return o;
			}

			half4 frag(v2f i):SV_TARGET
			{
				//计算涟漪
				float3 posWS = i.posWS;
				float3 viewDirWS = i.viewDirWS;
				float3 normalWS = normalize(i.normalWS);
				float2 uv =i.worldPos.xz -_Position1.xz;
				uv = uv/(_OrthographicCamSize*2);
				
				uv.x=uv.x*(-1);
				uv +=0.5;
				float ripples = SAMPLE_TEXTURE2D(_GlobalEffectRT1,sampler_GlobalEffectRT1,uv).b;
				ripples=step(0.9,ripples);
				float4 coloredRipples = (ripples*_FoamColor.rgb,ripples);
				//顶点上下偏移没有做（比较难）

				// i.screenPos.y += SAMPLE_TEXTURE2D(_NoiseTex,sampler_NoiseTex,i.noiseUV).r;


				//计算深度差值
				i.screenPos.xy /=i.screenPos.w;

                #ifdef UNITY_UV_STARTS_AT_TOP        //这里还挺重要的，为什么要加，好像是为了防止纹理倒转，一个宏定义
                i.screenPos.y = 1 - i.screenPos.y;
                #endif

				half existingDepth01 = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, sampler_CameraDepthTexture, i.screenPos.xy).r;
				half linearDepth01 = LinearEyeDepth(existingDepth01, _ZBufferParams );
				half depthDifference =linearDepth01- LinearEyeDepth(i.positionCS.z, _ZBufferParams); //这里为什么用裁剪空间，因为计算水的表面吗？
				half waterDepthDifference =saturate(depthDifference/_DepthMaxDistance);

				float2 uv_noise = i.uv.xy;
				float2 uv_distortion = i.uv.zw;



				//优化浮沫大小
				half4 existingNormal = SAMPLE_TEXTURE2D(_CameraNormalsTexture,sampler_CameraNormalsTexture,i.screenPos.xy);
				float3 normalDot = saturate(dot(existingNormal,i.viewNormal));	
				float foamDistance = lerp(_FoamMaxDistance,_FoamMinDistance,normalDot);
				half foamDepthDifference =saturate(depthDifference/foamDistance);
				half noiseCutoff = _NoiseCutoff * foamDepthDifference;


				//计算浮沫位置
				//uv扰动
				
				float2 distortionSample=(SAMPLE_TEXTURE2D(_SurfaceDistortion,sampler_SurfaceDistortion,uv_distortion*_SurfaceDistortion_ST.xy)*2-1)*_SurfaceDistortionAmount;			
				float2 noiseUV = float2(uv_noise .x+_Time.y*_SurfaceNoiseScroll.x, uv_noise .y+_Time.y*_SurfaceNoiseScroll.y);
				/* noiseUV = noiseUV.x+distortionSample.x + noiseUV.y+distortionSample.y; *///这句话好像不可以这么写
				noiseUV.x +=distortionSample.x;
				noiseUV.y +=distortionSample.y;
				half surfaceNoiseSample = SAMPLE_TEXTURE2D(_NoiseTex,sampler_NoiseTex,noiseUV*_NoiseTex_ST.xy+_NoiseTex_ST.zw).r;

				#define SMOOTHSTEP_AA 0.01
				half surfaceNoise = smoothstep(noiseCutoff-SMOOTHSTEP_AA,noiseCutoff+SMOOTHSTEP_AA,surfaceNoiseSample);	//将噪点图锐化
				
				

				//计算灯光
			    float addLightIntensity = 0.f;
                float specularAddLight = 0.f;
            	float3 addLightColor = (0,0,0,1);
            	float minDistance =0;
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
				

				half4 waterColor =lerp(_ShallowColor,_DeepColor,waterDepthDifference);


				waterColor =alphaBlend(_FoamColor*surfaceNoise,waterColor);

				// if (_EnableRipples == 1){
				// 	return waterColor+coloredRipples;
				// 	}

				// else{
				// 	return waterColor;
				// 	}
				
				return waterColor+coloredRipples+float4(addLightIntensity*addLightColor*0.01,0);
			}
			ENDHLSL
		}



	}
}
