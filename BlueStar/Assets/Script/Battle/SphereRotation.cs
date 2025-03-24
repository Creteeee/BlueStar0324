using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereRotation : MonoBehaviour
{
    // 自转速度
    public float rotationSpeed = 30f;
    
    // 旋转轴，默认为 Y 轴
    public Vector3 rotationAxis = Vector3.up;
    
    // 更新自转
    void Update()
    {
        // 根据 rotationAxis 绕指定轴旋转
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }

    // 设置新的旋转轴
    public void SetRotationAxis(Vector3 newAxis)
    {
        rotationAxis = newAxis.normalized; // 确保轴是单位向量
    }
}

