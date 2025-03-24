Shader "Unlit/StarSky"
{
    Properties
    {
        _Iteration("Iteration",Range(10,30))=17
        _Formuparam("Formuparam",float)=0.53
        _Volstep("Volstep",int)=20
        _StepSize("StepSize",float)=0.1
        _Zoom("Zoom",float)=0.8
        _Tile("Tile",float)=0.85
        _Speed("Speed",float)=0.01
        _Brightness("Brightness",float)=0.0015
        _DarkMatter("DarkMatter",float)=0.3
        _DistFading("DistFading",float)=0.73
        _Saturation("Saturation",float)=0.85
        _CenterPosition("CenterPosition",Vector)=(0,0,0,0)
        _SphereCenter("_SphereCenter",Vector)=(0,0,1,0)
        _UpColor("_UpColor",Color)=(1,1,1,1)
        _DownColor("_DownColor",Color)=(0,0,0,1)
        _LightAngle("_LightAngle",float)=0.1
        _SphereTexture("SphereTexture",2D)="white"{}
        _RotateSpeed("RotateSpeed",float)=0.01
        _SkyColorLight("SkyColorLight",Color)=(0.5,0.9,1,1)
        _SkyColorDark("SkyColorDark",Color)=(0,0,0.2,1)
        _FogHeight("FogHeight",Range(0,1))=0.2
    }
    SubShader
    {
        Tags {"Queue" = "Background" "RenderType"="Background"}
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv:TEXCOORD0;
            };

            struct v2f
            {
                float4 posCS : SV_POSITION;
            };
            
            CBUFFER_START(UnityPerMaterial)
            float _Iteration;
            float _Formuparam;
            int _Volstep;
            float _StepSize;
            float _Zoom;
            float _Tile;
            float _Speed;
            float _Brightness;
            float _DarkMatter;
            float _DistFading;
            float _Saturation;
            float4 _CenterPosition;

            float4 _SphereCenter;
            float4 _UpColor;
            float4 _DownColor;
            float _LightAngle;
            float4 _SphereTexture_ST;
            float _RotateSpeed;

            float4 _SkyColorLight;
            float4 _SkyColorDark;

            float _FogHeight;
            CBUFFER_END

            TEXTURE2D(_SphereTexture);
            SAMPLER(sampler_SphereTexture);

            //噪波函数
            float snoise(float3 uv, float time)    
            {
                const float3 s = float3(1e0, 1e2, 1e4);
                uv *= time;          
                float3 uv0 = floor(fmod(uv, time))*s;
                float3 uv1 = floor(fmod(uv+float3(1,1,1), time))*s;
                
                float3 f = frac(uv); f = f*f*(3.0-2.0*f);
                
                float4 v = float4(uv0.x+uv0.y+uv0.z, uv1.x+uv0.y+uv0.z,
                uv0.x+uv1.y+uv0.z, uv1.x+uv1.y+uv0.z);
                
                float4 r = frac(sin(v*1e-3)*1e5);
                float r0 = lerp(lerp(r.x, r.y, f.x), lerp(r.z, r.w, f.x), f.y);
                
                r = frac(sin((v + uv1.z - uv0.z)*1e-3)*1e5);
                float r1 = lerp(lerp(r.x, r.y, f.x), lerp(r.z, r.w, f.x), f.y);

                return lerp(r0, r1, f.z)*2.-1.;
            }
            

            v2f vert (appdata_t v)
            {
                v2f o;
                o.posCS = TransformObjectToHClip(v.vertex);
                return o;
            }
            

            half4 frag (v2f i) : SV_Target
            {
                //计算屏幕UV和方向
                //用computeuv有点问题,先下面这么算
                float2 uv = i.posCS.xy/_ScreenParams.xy-0.5;
                uv.y *=_ScreenParams.y/_ScreenParams.x;
                float3 dir=float3(uv*_Zoom,1);
                //float time =floor(_Time.y*2)/2*_Speed+0.25;//一秒采样2次，减少采样次数降低Dither，原先跟fps有关
                float time =_Time.y*_Speed+0.25;
       

                //计算旋转
                float a1=0.5+_CenterPosition.x/_ScreenParams.x*2;//前面加的小数不知啥意思
                float a2=0.8+_CenterPosition.y/_ScreenParams.y*2;
                float2x2 rot1=float2x2(cos(a1), sin(a1), -sin(a1), cos(a1));
                float2x2 rot2 =float2x2(cos(a2), sin(a2), -sin(a2), cos(a2));
                dir.xz = mul(rot1, dir.xz);
                dir.xy = mul(rot2, dir.xy);
                float3 from=float3(1,0.5,0.5);
                from +=float3(time*2,time,-2);
                from.xz = mul(rot1,from.xz);
                from.xy = mul(rot2,from.xy);

                //渲染星云
                float s = 0.1;
                float fade =1;
                float3 v = float3(0,0,0);
                for (int r = 0; r<_Volstep;r++)
                {
                    float3 p = from + s*dir*0.5;
                    p=abs(float3(float3(_Tile,_Tile,_Tile)-fmod(p,float3(_Tile,_Tile,_Tile)*2)));
                    float pa = 0;
                    float a = 0;
                    for(int i=0;i<_Iteration;i++)
                    {
                        p=abs(p)/dot(p,p)-_Formuparam;
                        a+=abs(length(p)-pa);
                        pa=length(p);
                    }
                    float dm=max(0,_DarkMatter-a*a*0.001);
                    a *=a*a;
                    if(r>6)
                    {
                        fade *=1-dm;
                    }
                    v +=fade;
                    v +=float3(s,s*s,s*s*s*s)*a*_Brightness*fade;
                    fade *=_DistFading;
                    s +=_StepSize;
                }
                
                v = lerp(float3(length(v).xxx),v,_Saturation);
                float3 v1 =v;
                float gradientY = smoothstep(0.1,0.5,i.posCS.y/_ScreenParams.y);
                float3 skyColor = lerp(_SkyColorLight.xyz,_SkyColorDark.xyz,gradientY);
                v=lerp(float3(0,0,0),v,v.x*0.1);
                /*v.x =smoothstep(8,10,v.x);
                v.y =smoothstep(8,10,v.y);
                v.z =smoothstep(8,10,v.z);*/
                v.x *=0.1;
                v.y *=0.4;
                v=max(skyColor,v*0.03);

                //星球位置计算
                float2 sphereCenter =uv- _SphereCenter.xy;
                float dist0 = length(sphereCenter)*2*_SphereCenter.z;
                float dist=saturate(1-dist0);

                //星球背景颜色
                float2x2 rotationMatrix = float2x2(cos(_LightAngle),-sin(_LightAngle),sin(_LightAngle),cos(_LightAngle));
                float2 lightUV = mul(rotationMatrix,sphereCenter);
                float lightMask = smoothstep(-0.2,0.7,lightUV.y);
                float lightMask2 =  smoothstep(0,0.2,lightMask );
                lightMask2 = saturate(lightMask2+0.03);
                float3 DistColor = dist*lerp(_DownColor,_UpColor,lightMask*5)*2;
                float3 DistColor1 = DistColor*0.5;


                //星球本体
                float sphere = smoothstep(0.1,0.2,dist);
                float2 sphereUV = lightUV*_SphereTexture_ST.xy;
                sphereUV.x +=_Time.y*_RotateSpeed;
                float sphereMask = saturate(SAMPLE_TEXTURE2D(_SphereTexture,sampler_SphereTexture,sphereUV).r+0.5)*saturate(lightMask+0.2)*sphere;
                float3 sphereColor = sphere*sphereMask*DistColor;

                //光晕
                float haloSize = uv*4;
                float r2 = dot(haloSize,haloSize);
                float halo = abs(distance(uv,_SphereCenter.xy)-(1-dist0));
                halo =(1-smoothstep(-1,0.05,halo))*1000;
                float3 haloColor = halo*DistColor;

                //计算日珥,感觉uv有点问题，再看看
                float fade2 = pow(length(2*sphereCenter),0.5);
                float fVal1 = 1-fade2;
                float fVal2 =1-fade2;
                float time2=_Time.y * 0.1;
                float angle = atan2(sphereCenter.x,sphereCenter.y)*0.2;
                float coord = float3(angle,dist0,time2*0.1);
                float Noise1 = abs(snoise(coord+float3(0,0,time2*0.015),15));
                float Noise2 = abs(snoise(coord+float3(0,-time2 * 0.1*0.15,time2*0.015),45));
                float power =6;
                fVal1 += (1 / power) * snoise(coord + float3(0.0, -time2, time2 * 0.2), (power * (10.0) * (Noise1 + 1.0)));
                fVal2 += (1 / power) * snoise(coord + float3(0.0, -time2, time2 * 0.2), (power * (25.0) * (Noise2 + 1.0)));
                float corona  = pow( fVal1 * abs( 1.1 - fade2), 2.0 ) * 12.0;
                corona *= 1.2 - Noise1;
                corona += pow( fVal2 * abs( 1.1 - fade2), 2.0 ) * 12.0;
                float coronaScale = 1-pow(abs(dist0*3.8),25);
                corona *= dist;
                float3 coronaColor = corona*DistColor;

                //绘制雾气效果
                
                
                return float4(DistColor1*3*lightMask2+sphereColor*5*lightMask2+haloColor*3*saturate(lightUV.y*10),1)+float4(v*(1-sphereMask),1);
                //return float4(v*0.01,1);
                //return halo;
                //return lightMask2;
                //return float4(v,1);

                
                
            }
            ENDHLSL
        }
    }
    FallBack "RenderFX/Skybox"
}
