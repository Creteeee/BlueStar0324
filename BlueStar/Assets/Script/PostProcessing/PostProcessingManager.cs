using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager :Singleton<PostProcessingManager>
{
    public Volume postProcessingVolume;
    ChromaticAberration chromaticAberration;
    DepthOfField depthOfField;
    public PixelizeRenderPassFeature pixelizeRenderPassFeature;
    public int LowresWidth;
    public int LowresHeight;
    ColorAdjustments colorAdjustments;
    public Controller_Terra terra;
    private float initialFocalLength;
    
    void Start()
    {
        postProcessingVolume.sharedProfile.TryGet(out chromaticAberration);
        postProcessingVolume.sharedProfile.TryGet(out depthOfField);
        postProcessingVolume.sharedProfile.TryGet(out colorAdjustments);
        pixelizeRenderPassFeature.settings.LowResWidth = 405;
        pixelizeRenderPassFeature.settings.LowResHeight = 720;
        depthOfField.focalLength.value = 1f;
        colorAdjustments.hueShift.value = 0;
        colorAdjustments.contrast.value = 0;
        LowresWidth = pixelizeRenderPassFeature.settings.LowResWidth;
        LowresHeight = pixelizeRenderPassFeature.settings.LowResHeight;
        pixelizeRenderPassFeature.SetActive(true);
        

    }

    void Update()
    {
        if (terra.Health>=100)
        {
            initialFocalLength = 1;
        }

        else if (terra.Health>=60 && terra.Health<100)
        {
            initialFocalLength = 30;
        }
        else
        {
            initialFocalLength = 60;
        }

    }
    
    public void OnHurt()
    {
        StartCoroutine(HurtEffectCoroutine());
    }

    IEnumerator HurtEffectCoroutine()
    {
        // 初始爆发
        chromaticAberration.intensity.value = 1.0f;
        float initialLowresWidth = LowresWidth;
        float initialLowresHeight = LowresHeight;


        // 逐渐恢复
        float duration = 0.3f;
        float timer = 0f;
        


        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            chromaticAberration.intensity.value = Mathf.Lerp(1.0f, 0f, t);
            depthOfField.focalLength.value = Mathf.Lerp(90f, initialFocalLength, t);
            colorAdjustments.hueShift.value = Mathf.Lerp(90f, 0f, t);
            colorAdjustments.contrast.value = Mathf.Lerp(-45f, 0f, t);
            LowresWidth = Mathf.RoundToInt(Mathf.Lerp(initialLowresWidth, 10, t));
            LowresHeight = Mathf.RoundToInt(Mathf.Lerp(initialLowresHeight, 400, t));

            // ★★同步到Pixelize效果设置★★
            pixelizeRenderPassFeature.settings.LowResWidth = LowresWidth;
            pixelizeRenderPassFeature.settings.LowResHeight = LowresHeight;

            yield return null;
        }

        // 最后归位
        chromaticAberration.intensity.value = 0f;
        depthOfField.focalLength.value=initialFocalLength;
        colorAdjustments.hueShift.value = 0;
        colorAdjustments.contrast.value = 0;
        LowresWidth = Mathf.RoundToInt(initialLowresWidth);
        LowresHeight = Mathf.RoundToInt(initialLowresHeight);

        pixelizeRenderPassFeature.settings.LowResWidth = LowresWidth;
        pixelizeRenderPassFeature.settings.LowResHeight = LowresHeight;
    }

    public void ResetFocalLength()
    {
        if (terra.Health>=100)
        {
            initialFocalLength = 1;
        }

        else if (terra.Health>=60 && terra.Health<100)
        {
            initialFocalLength = 30;
        }
        else
        {
            initialFocalLength = 60;
        }

        depthOfField.focalLength.value = initialFocalLength;
    }
}
