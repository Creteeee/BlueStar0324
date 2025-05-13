using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public float maxDistance = 100f;
    public int MaxReflectTimes = 5;
    private LineRenderer lineRenderer;
    public Vector3 direction = Vector3.forward;  // 默认方向（你可以改成 Vector3.right）

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // 推荐设置材质和颜色以便可见
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    private void Update()
    {
        FireLaserIteration(transform.position, direction.normalized, MaxReflectTimes);
    }

    void FireLaserIteration(Vector3 origin, Vector3 dir, int maxReflectTimes)
    {
        int currentReflectTimes = 0;
        Vector3 currentPos = origin;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, currentPos);

        while (currentReflectTimes < maxReflectTimes)
        {
            Ray ray = new Ray(currentPos, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                currentReflectTimes++;
                currentPos = hit.point;
                lineRenderer.positionCount = currentReflectTimes + 1;
                lineRenderer.SetPosition(currentReflectTimes, currentPos);

                // 计算反射方向
                dir = Vector3.Reflect(dir, hit.normal);
                currentPos += dir * 0.01f; // 防止下一帧仍然命中同一个点
            }
            else
            {
                // 没打到东西，延伸到最大距离
                lineRenderer.positionCount = currentReflectTimes + 2;
                lineRenderer.SetPosition(currentReflectTimes + 1, currentPos + dir * maxDistance);
                break;
            }
        }
    }
}