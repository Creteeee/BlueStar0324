using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public float simulationTime = 5f;  // 直接跳到5秒后的状态

    void Start()
    {
        if (particleSystem != null)
        {
            particleSystem.Simulate(simulationTime, true, true);
            particleSystem.Play(); // 确保它继续播放
        }
    }
}
