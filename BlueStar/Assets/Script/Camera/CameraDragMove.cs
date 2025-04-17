using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDragZoom : MonoBehaviour
{
    [Header("拖动设置")]
    public float dragSpeed = 0.5f;
    public Vector2 moveXRange = new Vector2(-10f, 10f);
    public Vector2 moveYRange = new Vector2(-5f, 5f);

    [Header("缩放设置")]
    public float zoomSpeed = 5f;
    public Vector2 zoomZRange = new Vector2(-20f, -5f); // 摄像机Z轴缩放范围（越小越远）

    private Vector3 dragOrigin;

    void Update()
    {
        HandleDrag();
        HandleZoom();
    }

    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(2))  // 中键按下
        {
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))  // 中键按住拖动
        {
            Vector3 difference = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(-difference.x * dragSpeed, -difference.y * dragSpeed, 0);

            Vector3 newPos = transform.position + move;

            // 限制移动范围
            newPos.x = Mathf.Clamp(newPos.x, moveXRange.x, moveXRange.y);
            newPos.y = Mathf.Clamp(newPos.y, moveYRange.x, moveYRange.y);

            transform.position = newPos;

            dragOrigin = Input.mousePosition;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            Vector3 pos = transform.position;
            pos.z += scroll * zoomSpeed;
            pos.z = Mathf.Clamp(pos.z, zoomZRange.x, zoomZRange.y);
            transform.position = pos;
        }
    }
}

