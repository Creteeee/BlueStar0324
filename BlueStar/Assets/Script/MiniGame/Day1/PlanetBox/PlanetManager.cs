using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class PlanetManager : MonoBehaviour
{
    public PlanetSlot[] slots;
    [SerializeField] private PlayableDirector _director;
    private GameObject[] planets=new GameObject[2];

    private void Update()
    {
        ClickPlanet();
        int x=0;
        foreach (PlanetSlot slot in slots)
        {

            if (!slot.isRight)
            {
                x += 1;
            }
            
        }

        if (x==0)
        {
            _director.Play();
            this.gameObject.GetComponent<PlanetManager>().enabled = false;
        }
    }

    void ClickPlanet()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 20f, LayerMask.GetMask("Click"))&& hit.transform.gameObject.CompareTag("PlanetModel"))
            {
                for (int i = 0; i < 2; i++)
                {
                    if (planets[i]==null)
                    {
                        planets[i] = hit.transform.gameObject;
                        break;
                    }
                }

                if (planets[1]!=null)
                {
                    Vector3 pos1 = planets[0].transform.position;
                    Vector3 pos2 = planets[1].transform.position;
                    planets[0].transform.DOMove(pos2, 0.5f);
                    planets[1].transform.DOMove(pos1, 0.5f);
                    planets = new GameObject[2];

                }
            }
        }
    }
}
