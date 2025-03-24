using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    public enum Weapons
    {
        Shotgun,Blackhole,LightSpot,Antimatter,Laser,Analyser
    }
    public enum Bullets
    {
        Bullet_Shotgun,Bullet_Blackhole,Bullet_LightSpot,Bullet_Antimatter,Bullet_Laser,Bullet_Analyser
    }
    
    
    public GameObject[] weaponPrefabs;
    public GameObject[] bulletPrefabs;
    public Dictionary<string,Weapons> weapondic = new Dictionary<string,Weapons>();
    public Dictionary<string, Bullets> bulletdic = new Dictionary<string, Bullets>();
    public Vector3 shootPos;
    public Vector3 mousePos;
    public Vector3 direction;
    private string[] weaponIndex = new []{"Q","W","E","A","S","D"};
    private int bulletIndex;
    private string currentKeyCode;
    private Weapons currentWeapon;
    private Bullets currentBullet;
    private Animator animator;
    public float force=0f;
    public float maxForce = 3f;
    public float acceleration=0.1f;

    void Start()
    {
        animator = GetComponent<Animator>();
        shootPos = this.transform.position;
        Debug.Log(shootPos);
        //weapons[0].SetActive(true);

        bulletIndex = 0;
        
        //添加武器和对应的按键到字典中
        weapondic.Add(new string("Q"),Weapons.Shotgun);
        weapondic.Add(new string("W"),Weapons.Blackhole);
        weapondic.Add(new string("E"),Weapons.LightSpot);
        weapondic.Add(new string("A"),Weapons.Antimatter);
        weapondic.Add(new string("S"),Weapons.Laser);
        weapondic.Add(new string("D"),Weapons.Analyser);
        
        //将子弹添加到字典中
        bulletdic.Add(new string("Q"),Bullets. Bullet_Shotgun);
        bulletdic.Add(new string("W"),Bullets. Bullet_Blackhole);
        bulletdic.Add(new string("E"),Bullets. Bullet_LightSpot);
        bulletdic.Add(new string("A"),Bullets. Bullet_Antimatter);
        bulletdic.Add(new string("S"),Bullets. Bullet_Laser);
        bulletdic.Add(new string("D"),Bullets. Bullet_Analyser);
        
        //初始蓄力值为0
        force = 0f;
    }

    void Update()
    {
        
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane));
        direction = Vector3.Normalize(shootPos-mousePos);
        direction = new Vector3(direction.x, 0, direction.z);
        Shoot();
        transform.forward = direction;
        switchWeapons();
        switchBullets();
        bulletIndex = Array.IndexOf(weaponIndex, currentKeyCode);
    }

    void Shoot()
    {
        if (Input.GetButton("Fire1"))
        {
            
            if (force < maxForce)
            {
                force += acceleration;
            }
        

        }


        
       if(Input.GetButtonUp("Fire1"))

       {
           
            Fire();
            force = 0f;
        }

        Debug.Log("此时的蓄力值为：" + force);

    }

    void Fire()
    {
        
        
    
        if (bulletIndex >= 0) // 确保找到了有效的索引
        {
            GameObject bullet = Instantiate(bulletPrefabs[bulletIndex], shootPos, Quaternion.identity);
            bullet.GetComponent<Bullet>().direction = direction;
        }
        else
        {
            Debug.LogError("未找到对应的子弹索引，currentKeyCode: " + currentKeyCode);
        }

    }

    void switchWeapons()
    {
        foreach (string key in weaponIndex)
        {
            KeyCode keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), key);
            {
                if (Input.GetKeyDown(keyCode))
                {

                    if (weapondic.TryGetValue(key, out Weapons weapon))
                    {
                        // 先禁用之前的武器
                        if (weaponPrefabs[(int)currentWeapon] != null)
                        {
                            weaponPrefabs[(int)currentWeapon].SetActive(false);
                        }

                        currentWeapon = weapon;
                        Debug.Log("当前武器: " + currentWeapon.ToString());

                        // 激活当前选中的武器
                        if (weaponPrefabs[(int)currentWeapon] != null)
                        {
                            weaponPrefabs[(int)currentWeapon].SetActive(true);
                        }
                    }
                }
            }
        }
    }
    void switchBullets()
    {
        foreach (string key in weaponIndex)
        {
            KeyCode keyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), key);
            
            {
                if (Input.GetKeyDown(keyCode))
                {
                    currentKeyCode = key;

                    if (bulletdic.TryGetValue(key, out Bullets bullet))
                    {

                        currentBullet = bullet;

                    }
                    Debug.Log("当前子弹: " + currentBullet.ToString());
                }
            }
        }
    }
}
    
