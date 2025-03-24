using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelizeRenderPassFeature : ScriptableRendererFeature
{
    
        [System.Serializable]   //serializable是对公有字段的序列化，serializedField是对私有
        public class Settings
        {
            
            public RenderPassEvent RenderPassEvent;
            //public LayerMask layerMask;
            public int LowResWidth;
            public int LowResHeight;

        }



        class PixelizeRenderPass : ScriptableRenderPass
        {
            private Settings Settings;
            private PixelizeRenderPassFeature feature;
            private RenderTargetIdentifier colorBuffer, pixelBuffer;
            private int pixelBufferID = Shader.PropertyToID("_PixelBuffer");//这个id的意义是什么
            
            
            

            
               public PixelizeRenderPass(Settings settings, PixelizeRenderPassFeature feature)
                {
                    this.Settings = settings;
                    this.feature = feature;

                }


                public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
                {
                    base.OnCameraSetup(cmd, ref renderingData);
                    colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
                    pixelBuffer = new RenderTargetIdentifier(pixelBufferID);
                    RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
                    int lowResWidth = Settings.LowResWidth;
                    int lowResWHeight = Settings.LowResHeight;
                    desc.width = lowResWidth;
                    desc.height = lowResWHeight;
                    cmd.GetTemporaryRT(pixelBufferID,desc,FilterMode.Point);
                }


                public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
                {
                    CommandBuffer cmd = CommandBufferPool.Get("像素化");
                    // 获取相机的 LayerMask
                    Camera camera = renderingData.cameraData.camera;
                    int originalCullingMask = camera.cullingMask;  // 保存原始的 cullingMask

                    // 设置新的 cullingMask，只渲染指定的 Layer
                    //camera.cullingMask = Settings.layerMask.value;
                    
                    
                    
                    
                    //cmd.Blit(colorBuffer,pixelBuffer,Settings.PixelizeMat,0);
                    cmd.Blit(colorBuffer,pixelBuffer);
                    cmd.Blit(pixelBuffer,colorBuffer);
                    context.ExecuteCommandBuffer(cmd);
                    CommandBufferPool.Release(cmd);
                   



                }

         
                public override void OnCameraCleanup(CommandBuffer cmd)
                {
                    cmd.ReleaseTemporaryRT(Shader.PropertyToID("_PixelBuffer"));
          
   
                }
        }
        

        private PixelizeRenderPass _PixelizeRenderPass;

        public Settings settings = new Settings();

        public override void Create()
        {
            _PixelizeRenderPass = new PixelizeRenderPass(settings,this);    //这个this干什么用的
            _PixelizeRenderPass.renderPassEvent = settings.RenderPassEvent;
            
            

        }

     
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(_PixelizeRenderPass);

        }
        
        
    
    
}



