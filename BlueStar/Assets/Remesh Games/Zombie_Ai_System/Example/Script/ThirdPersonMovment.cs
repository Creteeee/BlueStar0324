using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class ThirdPersonMovment : MonoBehaviour
{

    public CharacterController controller;
    public float Speed = 6f;
    public float SmoothTime = 0.1f;
    float TurnSmoothVelocity;

    public Transform Cam;

    public Animator Anim;

    

    // Start is called before the first frame update
    void Start()
    {
  
    }

    // Update is called once per frame
    void Update()
    {

        float Horizontal = Input.GetAxisRaw("Horizontal");
        float Vertical = Input.GetAxisRaw("Vertical");

        Vector3 Diraction = new Vector3(Horizontal, 0f, Vertical).normalized;

        if(Diraction.magnitude >= 0.1f) 
        {
            Anim.SetBool("Walk", true);
           
            float targetAngle = Mathf.Atan2(Diraction.x, Diraction.z) * Mathf.Rad2Deg + Cam.transform.eulerAngles.y;
            float Angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref TurnSmoothVelocity, SmoothTime);
            transform.rotation = Quaternion.Euler(0f,Angle,0f);

            Vector3 MoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            //controller.Move(MoveDir.normalized * Speed * Time.deltaTime);
            controller.transform.position += MoveDir.normalized * Speed * Time.deltaTime;
            
        }

        if (Diraction.magnitude < 0.1f)
        {
            Anim.SetBool("Walk", false);
          
        }

        if(Input.GetKey(KeyCode.LeftShift) && Diraction.magnitude >= 0.1f) 
        {
            Anim.SetBool("Run", true);
            Speed = 6f;


        }

      
        if (!Input.GetKey(KeyCode.LeftShift) || Diraction.magnitude < 0.1f)
        {
            Anim.SetBool("Run", false);
            Speed = 3f;
        }



    }




}
