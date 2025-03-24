using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CharacterController : MonoBehaviour
{
    // Start is called before the first frame update
    private CharacterController _character;
    private Animator _animator;
    void Start()
    {
        _character = GetComponent<CharacterController>();
        _animator =GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 dir = new Vector3(horizontal, 0, vertical);
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(dir);
            _animator.SetBool("isRun",true);
            transform.Translate(Vector3.forward*2*Time.deltaTime);
        }
        else
        {
            _animator.SetBool("isRun",false);
        }
    
    }
}
