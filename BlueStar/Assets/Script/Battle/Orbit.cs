using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class Orbit
{
    public float semiMajorAxis = 5f; // 半长轴
    public float eccentricity = 0.3f; // 偏心率
    public float orbitalSpeed = 0.05f; // 轨道速度（决定飞船在轨道上的速度）
    public float acceleration = 0f; // 外部加速度，决定飞船的加速（正值表示加速，负值表示减速）
    public float Rotation_Tangent=0f;//飞船相对于切线的旋转角度
    public Vector2 center = new Vector2(0f, 0f); // 轨道中心（可以是恒星或另一个天体）
    private float initialTrueAnomaly = 0f; // 初始真近点角 (0是开始时的位置)
    public float trueAnomaly=0f; // 当前真近点角
    private Vector3 position=new Vector3(0f, 0f,0f);
    public Vector3 CameraDir=new Vector3(0f, 0f,0f);


    public void InitializeParameter(Vector3 pos, Vector3 centerPos, float v, float a, float e,float rotationAngle,float sMA)//初始化position和六个参数
    {
        semiMajorAxis = sMA;
        eccentricity = e;
        orbitalSpeed = v;
        acceleration = a;
        Rotation_Tangent = rotationAngle;
        center = centerPos;
        position = pos;
        initialTrueAnomaly = Mathf.Atan2(pos.y - centerPos.y, pos.x - centerPos.x);
        trueAnomaly = initialTrueAnomaly;//设置初始角度
    }

    public void UpdateParameter(Vector3 centerPos,float v, float a, float e,float rotationAngle,float sMA,float angle)//初始化position和六个参数
    {
        semiMajorAxis = sMA;
        eccentricity = e;
        orbitalSpeed = v;
        acceleration = a;
        Rotation_Tangent = rotationAngle;
        trueAnomaly = angle;//设置初始角度
        center = centerPos;
    }
    public (Vector3,Vector3, float, float, float,float) UpdatePosition(Vector3 pos)//传回输入值
    {
        //更新加速度
        acceleration *=Mathf.Cos(Rotation_Tangent)*0.01f;//切向的加速度
        //acceleration += Mathf.Sin(Rotation_Tangent);//径向的加速度，向心/离心
        orbitalSpeed += acceleration*0.05f;
        
        //更新极坐标的角度
        trueAnomaly += orbitalSpeed * Time.deltaTime;
        
        if (trueAnomaly >= 2 * Mathf.PI) 
        {
            trueAnomaly -= 2 * Mathf.PI;
        }
        
        //更新半长轴
        semiMajorAxis += acceleration * Time.deltaTime*1f;
        float r = semiMajorAxis * (1 - eccentricity * eccentricity) / (1 + eccentricity * Mathf.Cos(trueAnomaly));
        float x = center.x+r * Mathf.Cos(trueAnomaly);
        float y = center.y+r * Mathf.Sin(trueAnomaly);
        CameraDir =new Vector3(Mathf.Cos(trueAnomaly), Mathf.Sin(trueAnomaly), 0);
        position= new Vector3(x, y, 0f);
        return (position,center,acceleration,trueAnomaly,semiMajorAxis,orbitalSpeed);//返回属性元组
    }
    public Vector3 GetTangentDirection()
    {
        float r = semiMajorAxis * (1 - eccentricity * eccentricity) / (1 + eccentricity * Mathf.Cos(trueAnomaly));
        float x = center.x + r * Mathf.Cos(trueAnomaly);
        float y = center.y + r * Mathf.Sin(trueAnomaly);
        Vector3 currentPosition = new Vector3(x, y, 0f);

        // 计算切线方向，正交于当前位置的向量
        Vector3 velocity = new Vector3(-Mathf.Sin(trueAnomaly), Mathf.Cos(trueAnomaly), 0f); // 切线速度方向
        return velocity.normalized; // 返回单位化的切线方向
    }
    public Vector3 GetPosition()
    {
        return position;
    }

    // 获取轨道速度
    public Vector3 GetVelocity()
    {
        float r = semiMajorAxis * (1 - eccentricity * eccentricity) / (1 + eccentricity * Mathf.Cos(trueAnomaly));
        float velocityX = -Mathf.Sqrt(semiMajorAxis * (1 - eccentricity * eccentricity)) * Mathf.Sin(trueAnomaly);
        float velocityY = Mathf.Sqrt(semiMajorAxis * (1 - eccentricity * eccentricity)) * Mathf.Cos(trueAnomaly);
        return new Vector3(velocityX, velocityY, 0);
    }
}