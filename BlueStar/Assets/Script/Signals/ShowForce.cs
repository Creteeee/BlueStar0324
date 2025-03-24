using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShowForce : MonoBehaviour
{
    private Shooter shooter;
    private float force;
    private float maxForce;
    private Material mat;
    public Color color1;
    public Color color2;
    public Color color3;

    
    // Start is called before the first frame update
    void Start()
    {
      
        shooter = this.GetComponent<Shooter>();
        mat = this.GetComponent<Renderer>().material;
        mat.SetColor("_MainColor",color1);
        Debug.Log("当前的颜色值为"+mat.color);



    }

    // Update is called once per frame
    void Update()
    {
        force = shooter.force;
        maxForce = shooter.maxForce;
        mat.SetColor("_MainColor",color3*force/maxForce);
        
      /*  if (force <  maxForce*0.5  || force >= 0)
        {
            mat.SetColor("_MainColor",color1);
        }
        if (force >= 0.5 * maxForce || force <  maxForce)
        {
            mat.SetColor("_MainColor",color2);
        }
        if (force >= maxForce)
        {
            mat.SetColor("_MainColor",color3);
        }*/
        
    }
}
