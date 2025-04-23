
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Emitter : MonoBehaviour,Aircraft
{

    public Orbit orbit;
    public Vector3 center_Star = new Vector2(-1,1);
    public float semiMajorAxis = 5f; // 半长轴
    public float eccentricity = 0.3f; // 偏心率
    public float orbitalSpeed = 0.8f; // 轨道速度（决定飞船在轨道上的速度）
    private float initialSpeed;
    public float acceleration = 0f; // 外部加速度，决定飞船的加速（正值表示加速，负值表示减速）
    public float Rotation_Tangent=0f;//飞船相对于切线的旋转角度
    private SpaceShip sp;
    private List<Vector3> points ;
    public int pointCounts = 50;
    private GameObject linePrefab;
    private GameObject line;
    private LineRenderer lineRenderer;
    private int timer=0;
    private float a;
    private float a_initial;
 
    public List<GameObject> enemies;
    public float minDistance = 1f;
    public GameObject bulletPrefab;
    public Vector3 TangentDir=new Vector3();
    public static Vector3 BulletDir;
    public static int currentBulletID = -1;
    
    

    
        void Start()
    {
        orbit = new Orbit();
        sp =GameObject.Find("SpaceShip").GetComponent<SpaceShip>();
        center_Star = sp.center_Star;
        semiMajorAxis = sp.semiMajorAxis+0.01f;
        orbitalSpeed = sp.orbitalSpeed + 0.01f;
        initialSpeed = orbitalSpeed;
        orbit.InitializeParameter(this.transform.position,center_Star,sp.orbitalSpeed,acceleration,sp.eccentricity,Rotation_Tangent,semiMajorAxis);
        points = new List<Vector3>();
        linePrefab = Resources.Load<GameObject>("Prefabs/Line/Line");
        bulletPrefab=Resources.Load<GameObject>("Prefabs/Bullet/Type_1/Bullet_Type1");
        a = semiMajorAxis;
        a_initial = a;
    }

    private void OnEnable()
    {
     
    }

    void Update()
    {
        if (timer < 1)
        {
            line=Instantiate(linePrefab);
            timer++;
        }
        onMove();
        Addpoints(orbit.trueAnomaly);
        lineRenderer = line.GetComponent<LineRenderer>();
        lineRenderer.positionCount = pointCounts;
        lineRenderer.SetPositions(points.ToArray());
        
        orbitalSpeed=a/a_initial*initialSpeed;
        




    }

    public void onMove()
    {
        //这句先暂时注释掉，改成由键盘输入赋值
        //acceleration = DataManager.emitterAcceleration;
        var (newPosition,newCenter, newAcceleration, newTrueAnomaly, newSemiMajorAxis,newOrbitalSpeed) = orbit.UpdatePosition(this.transform.position);
        orbit.UpdateParameter(center_Star,orbitalSpeed,acceleration,eccentricity,Rotation_Tangent,newSemiMajorAxis,newTrueAnomaly);//这个只是给orbit里的方法使用
        //这个物体本身的各个参数还要刷新一遍
        a = newSemiMajorAxis;
        this.transform.position = newPosition;
        
        //  设置物体的旋转，让 Y 轴（transform.up）指向切线方向
        Vector3 tangent = orbit.GetTangentDirection();
        if (points != null && points.Count >= 2)
        {
            Vector3 tangentFromPoints = (points[1] - points[0]).normalized;
            this.transform.up = tangentFromPoints;
        }
    }

    public void onShoot()
    {
        
    }


    public void Addpoints(float beginAngle)
    {
        
        points.Clear();
        for (int i = 0; i <= pointCounts; i++)
        {
            float deltaAngle = 0.5f * Mathf.PI / pointCounts; //之前这里写的是pointCounts
            float r = a * (1 - eccentricity * eccentricity) / (1 + eccentricity * Mathf.Cos(beginAngle));
            float x = r * Mathf.Cos(beginAngle) + center_Star.x;
            float y = r * Mathf.Sin(beginAngle) + center_Star.y;
            Vector3 point = new Vector3(x, y, 0);
            
            beginAngle += deltaAngle;
            points.Add(point);
            a += acceleration * 0.01f;
        }
        
    }
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            if (points == null) points = new List<Vector3>();
            Addpoints(0f); // 用默认角度画一下（你可以改成其他合理角度）
        }

        if (points == null || points.Count < 2) return;

        Vector3 tangent = (points[1] - points[0]).normalized;
        Vector3 start = points[0];
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(start, start + tangent * 2f);
    }

}
