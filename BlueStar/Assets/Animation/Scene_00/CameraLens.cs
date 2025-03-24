using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class CameraLens : MonoBehaviour
{
    public Volume volume; // 场景中的Global Volume
    private DepthOfField depthOfField;
    public float aperture = 0;

    void Start()
    {
        if (volume == null)
        {
            Debug.LogError("Volume 未指定！");
            return;
        }

        // 获取 VolumeProfile
        VolumeProfile profile = volume.profile;
        if (profile == null)
        {
            Debug.LogError("VolumeProfile 未找到！");
            return;
        }

        // 检查 VolumeProfile 是否有 Depth of Field，并获取
        if (!profile.TryGet(out depthOfField))
        {
            Debug.LogError("未找到 Depth of Field 设置！");
            return;
        }
        
    }

    void Update()
    {
        // 动态修改景深效果
        if (depthOfField != null)
        {
            depthOfField.focalLength.value = Mathf.PingPong(Time.time*140, 200f); // 模拟焦距在0到10之间波动
        }
    }
}
