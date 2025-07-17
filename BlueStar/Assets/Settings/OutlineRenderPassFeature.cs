using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineRenderPassFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Material outlineMaterial;
        // 不能在这里引用场景对象
    }

    [SerializeField]
    public Settings settings = new Settings(); // ← ← ← 这个必须加 SerializeField！

    OutlineRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new OutlineRenderPass(settings);
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }

    class OutlineRenderPass : ScriptableRenderPass
    {
        Settings settings;

        public OutlineRenderPass(Settings settings)
        {
            this.settings = settings;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (OutlineObjectsManager.objsForRender == null || OutlineObjectsManager.objsForRender.Count == 0)
                return;
            var cmd = CommandBufferPool.Get("Draw Outline");
            Debug.unityLogger.Log( OutlineObjectsManager.objsForRender[0].name);

            foreach (var obj in OutlineObjectsManager.objsForRender)
            {
                var renderer = obj?.GetComponent<Renderer>();
                if (renderer != null)
                {
                    cmd.DrawRenderer(renderer, settings.outlineMaterial,0,0);
                    cmd.DrawRenderer(renderer, settings.outlineMaterial,0,1);
                }
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}



