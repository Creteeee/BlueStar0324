using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public float maxDistance = 100f;
    public int MaxReflectTimes = 5;
    private LineRenderer lineRenderer;   
    private Vector3 WorldMousePos;
    private Vector2 direction;
    private Vector2 startFirePos;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        FireLaserIteration(this.gameObject.transform.position,direction,MaxReflectTimes);
    }

    void FireLaserIteration(Vector2 origin, Vector2 direction,int maxReflectTimes)
    {
        int currentReflectTimes = 0;
        Vector2 currentPos = origin;
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(currentReflectTimes,currentPos);

        RaycastHit2D hit = Physics2D.Raycast(currentPos, direction, maxDistance);
        while (hit.collider!=null && currentReflectTimes<=maxReflectTimes)
        {

            
            currentPos = hit.point;

            lineRenderer.positionCount++;
            lineRenderer.SetPosition(++currentReflectTimes,currentPos);
            
            //计算反射
            direction = Vector2.Reflect(direction, hit.normal);

            Vector2 OFFSET = new Vector2(0.01f, 0.01f);
            currentPos = currentPos + direction * OFFSET;
            hit = Physics2D.Raycast(currentPos, direction, maxDistance);
        }
    }

}
