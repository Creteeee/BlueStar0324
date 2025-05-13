using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLaserCube : MonoBehaviour
{
    public Camera mainCamera;
    public float rotateDuration = 1;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            int layerMask = ~(1 << LayerMask.NameToLayer("Player"));
            if (Physics.Raycast(ray, out hit,50f,layerMask))
            {
                if (hit.transform.gameObject==this.gameObject)
                {
                    StartCoroutine(RotateCubeWorldAxis(Vector3.back, 10f, rotateDuration));
                }
            }
        }
        
    }

    private IEnumerator RotateCubeWorldAxis(Vector3 axis, float angle, float duration)
    {
       

        Vector3 pivot = transform.position; // 绕自己的中心旋转
        float rotated = 0f;

        while (rotated < angle)
        {
            float step = (angle / duration) * Time.deltaTime;
            float remaining = angle - rotated;
            float actualStep = Mathf.Min(step, remaining);

            transform.RotateAround(pivot, axis.normalized, actualStep);
            rotated += actualStep;
            yield return null;
        }

        
    }
}
