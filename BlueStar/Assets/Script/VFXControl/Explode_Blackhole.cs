using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode_Blackhole : MonoBehaviour
{
    private Material mat;
    public float edge;

    // Start is called before the first frame update
    void Start()
    {
        mat =this.GetComponent<Renderer>().material;
        Debug.Log(mat);
        

    }

    // Update is called once per frame
    void Update()
    {
        
        mat.SetFloat("_Edge1",edge);
    }
}
