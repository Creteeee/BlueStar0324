using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineObjectsManager : MonoBehaviour
{
   

    public List<GameObject> outlineObjects = new List<GameObject>();
    public static List<GameObject> objsForRender = new List<GameObject>();
    public Camera camera;
    private GameObject previousObj;

    private void Awake()
    {
        objsForRender.Clear();
        //camera = Camera.main;
    }

    void Update()
    {
        objsForRender=outlineObjects;
        if (camera != null)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
    

            if (Physics.Raycast(ray, out hit, 20f, LayerMask.GetMask("InteractiveMesh","Click")))
            {
                //Debug.Log(this.gameObject.name);
                if (hit.transform.gameObject != previousObj && ObserveItem.currentObserveObj== null)
                {
                    objsForRender.Clear();
                    objsForRender.Add(hit.collider.gameObject);
                    previousObj=hit.transform.gameObject;
                }

            }

            else
            {
                objsForRender.Clear();
                previousObj = null;
            }
        }
    }
    
}
