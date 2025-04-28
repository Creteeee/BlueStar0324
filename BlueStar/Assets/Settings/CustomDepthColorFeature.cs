using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomDepthColorFeature : ScriptableRendererFeature
{
    class CustomDepthColorPass : ScriptableRenderPass
    {
        private Material depthColorMaterial;
        private RenderTargetIdentifier source;
        private RenderTargetHandle tempTexture;

        public CustomDepthColorPass(Material material)
        {
            this.depthColorMaterial = material;
            tempTexture.Init("_TempCustomDepthColorTexture");
        }

        public void Setup(RenderTargetIdentifier source)
        {
            this.source = source;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (depthColorMaterial == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("Custom Depth Color Pass");

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            cmd.GetTemporaryRT(tempTexture.id, opaqueDesc, FilterMode.Bilinear);

            // 1. 采样深度 ➔ 渲染成颜色图
            Blit(cmd, source, tempTexture.Identifier(), depthColorMaterial);

            // 2. 把新的图覆盖回相机
            Blit(cmd, tempTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (cmd == null) return;
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
        
    }

    public Material depthColorMaterial;
    CustomDepthColorPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomDepthColorPass(depthColorMaterial);

        // 注意顺序，放在Pixelize之后！
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //m_ScriptablePass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(m_ScriptablePass);
    }
}