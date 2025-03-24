using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Zombie_CS : MonoBehaviour
{

    NavMeshAgent ZombieNavMesh;
    Transform Player;
    Animator Anim;
    [HideInInspector]
    public bool IsMove;
    bool Ready;
    public float Loudness = 1.0f;
    //public Color SphereColor;
    float ReadyTime;
    ///////////// MovmentState //////////////////////
    public enum Movment { Walk, Run };
    public Movment MovementState;
    [HideInInspector]
    public int NumberMovment;
    ///////////// HealthBar State //////////////////////
    public enum HealthBar { Hidden, Unhidden };
    public HealthBar HealthBarState;
    [HideInInspector]
    public int NumberHealth;

    ///////////// Destroy State //////////////////////
    public enum DestroyED { Enable, Disable };
    public DestroyED DestroyState;
    [HideInInspector]
    public int NumberDestroy;

    ///////////// Destroy State //////////////////////
    public enum AttackActivation { Enable, Disable };
    public AttackActivation AttackActivationState;
    [HideInInspector]
    public int NumberAttackActivation;


    float LookAtSpeed = 3.0f;
    public string PlayerName;
    public float Damage;
    public float Health = 100.0f;
    float RandomDestroy;
    Slider HealthBarUI;
    public Canvas HealthBarCanvas;
    public Camera camera;
    float DistanceToPlayer;
    [HideInInspector]
    public bool Spawn;
    Transform Pos;
    [HideInInspector]
    public bool ChackHit;
    [HideInInspector]
    public bool CanISee;

    // Start is called before the first frame update
    void Start()
    {


        HealthBarUI = transform.Find("HealthBar_Canvas/Slider").GetComponent<Slider>();
        HealthBarCanvas = transform.Find("HealthBar_Canvas").GetComponent<Canvas>();
        camera = GameObject.Find("MainCamera").GetComponent<Camera>();
        Pos = transform.Find("Pos").GetComponent<Transform>();
        ZombieNavMesh = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();

        Player = GameObject.Find(PlayerName).transform;
        RandomDestroy = Random.Range(5.0f, 8.0f);
        ChackHit = false;
        if (Spawn == false)
        {

            switch (AttackActivationState)

            {
                case AttackActivation.Enable:
                    {
                        NumberAttackActivation = 0;


                        break;
                    }

                case AttackActivation.Disable:
                    {
                        NumberAttackActivation = 1;

                        break;
                    }

            }



            ////////////////////////////// switch (MovmentState) //////////////////////////////
            switch (MovementState)

            {
                case Movment.Walk:
                    {

                        NumberMovment = 0;
                        break;
                    }

                case Movment.Run:
                    {

                        NumberMovment = 1;
                        break;
                    }

            }

            ////////////////////////////// switch (HealthBarState) //////////////////////////////
            switch (HealthBarState)

            {
                case HealthBar.Unhidden:
                    {

                        NumberHealth = 0;
                        break;
                    }

                case HealthBar.Hidden:
                    {

                        NumberHealth = 1;
                        break;
                    }

            }


            ////////////////////////////// switch (HealthBarState) //////////////////////////////
            switch (DestroyState)

            {
                case DestroyED.Enable:
                    {

                        NumberDestroy = 0;
                        break;
                    }

                case DestroyED.Disable:
                    {

                        NumberDestroy = 1;
                        break;
                    }

            }


        }


        if (NumberAttackActivation == 0)
        {
            ChackHit = true;
        }


        if (NumberAttackActivation == 1)
        {
            ChackHit = false;
        }




    }

    // Update is called once per frame
    void Update()
    {

        Anim.SetLayerWeight(Anim.GetLayerIndex("UpperBody"), 1);

        ReaytoFalse();
        HearingRange();


        if (DistanceToPlayer < 2 && ChackHit == true) 
        {
           LookAtTarget();
        }

            HealthBarUI.value = Health / 100;
            HealthBarUI.transform.LookAt(HealthBarCanvas.transform.position + (camera.transform.rotation * Vector3.back), camera.transform.rotation * Vector3.up);
        
       

        if (Health > 0.0f)
        {
            CheckAttack();
            SeePlayer();
        }


        ////////////////  Angle or SightSystem /////////////////////////////////

        Vector3 targetDir = Player.position - transform.position;
        float angle = Vector3.Angle(targetDir, transform.forward);

        if (angle < 45.0f && DistanceToPlayer < 10.0f && CanISee == true) 
        {
           ChackHit = true;
        }

        ////////////////////  Distance To Player/////////////////////////////////


        DistanceToPlayer = Vector3.Distance(Player.position, transform.position);

       // Debug.Log(DistanceToPlayer);

        ////////////////////////// All Stat for The Stat MoveMent ///////////////////

  
        if(NumberMovment == 0) 
        {
            if (IsMove == true)
            {

                ZombieNavMesh.speed = 1.5f;
                ZombieNavMesh.destination = Player.position;
               // LookAtTarget();
                //  transform.LookAt(new Vector3(Player.position.x, transform.position.y, Player.position.z));
                Anim.SetBool("Walk", true);
            }
            if (IsMove == false)
            {
                Anim.SetBool("Walk", false);
                ZombieNavMesh.speed = 0;
            }
        }



        //////////////////  Movment stat Run  ///////////////////////////////////
        if (NumberMovment == 1)
        {

            if (IsMove == true)
            {
                ZombieNavMesh.speed = 3f;
                ZombieNavMesh.destination = Player.position;
              //  LookAtTarget();
                // transform.LookAt(new Vector3(Player.position.x, transform.position.y, Player.position.z));
                Anim.SetBool("Run", true);
            }
            if (IsMove == false)
            {
                Anim.SetBool("Run", false);
                ZombieNavMesh.speed = 0;
            }

        }


        


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////// Health  ////////////////////////////

        if (Health <= 0.0f)
        {
          
            Death();
        }

        if(NumberHealth == 0 && Health > 0.0f) 
        {
            HealthBarUI.gameObject.SetActive(true);
        }


        if (NumberHealth == 1 && Health > 0.0f)
        { 
            HealthBarUI.gameObject.SetActive(false);
        }



        /////////////////////// Read to Hearing ///////////////////////////////////

        MakeNoise ZombieNoise = Player.GetComponent<MakeNoise>();
        if (ZombieNoise != null) 
        {
            if (Ready == true && ZombieNoise.Noise == true)
            {
                ChackHit = true;
            }
        }
 


}

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////
    /// </summary>

    void LookAtTarget()
    {
        if (Health > 0.0f)
        {
            var rotation = Quaternion.LookRotation(Player.position - transform.position);
            rotation.x = 0; //This is for limiting the rotation to the y axis. I needed this for my project so just
            rotation.z = 0;           //      delete or add the lines you need to have it behave the way you want.
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * LookAtSpeed);
        }
    }


    void CheckAttack()
    {

        RaycastHit hit;
        float range = 1.3f;
        Debug.DrawRay(Pos.position, Pos.transform.TransformDirection(Vector3.forward) * range, Color.green);

        if (Physics.Raycast(Pos.position, Pos.transform.TransformDirection(Vector3.forward), out hit, range))
        {

            if (hit.transform.gameObject.name == PlayerName)
            {
                // Debug.Log("Hit");
               // Anim.SetLayerWeight(Anim.GetLayerIndex("UpperBody"), 1);
                Anim.SetBool("Attack", true);
               // LookAtTarget();
                IsMove = false;
                LookAtSpeed = 2.0f;

            }



        }



        else
        {
            //Debug.Log("NoThink");
           //  Anim.SetLayerWeight(Anim.GetLayerIndex("UpperBody"), 0);
            Anim.SetBool("Attack", false);
          if(ChackHit == true) 
            {
                IsMove = true;
                LookAtSpeed = 3.0f;
            }
       
        }


    }

    public void EventAttack()
    {

        RaycastHit hit;
        float range = 1.3f;
        Debug.DrawRay(Pos.position, Pos.transform.TransformDirection(Vector3.forward) * range, Color.red);

        if (Physics.Raycast(Pos.position, Pos.transform.TransformDirection(Vector3.forward), out hit, range))
        {

            if (hit.transform.gameObject.name == PlayerName)
            {
                hit.transform.gameObject.SendMessage("ApplyDamage", Damage);
            }



        }


    }


    void Death()
    {

        Anim.SetBool("Attack", false);
        HealthBarUI.gameObject.SetActive(false);
        Anim.SetBool("Death", true);
        int RandomDeath = Random.Range(1, 5);
        Anim.SetInteger("Death_Int", RandomDeath);
        ZombieNavMesh.speed = 0.0f;
        //ZombieNavMesh.enabled = false;
        this.GetComponent<Collider>().enabled = false;
        if (NumberDestroy == 0)
        {
            // StartCoroutine(TimeToDestroy());
        }
  
            
    }


    IEnumerator TimeToDestroy()

    { 
        yield return new WaitForSeconds(RandomDestroy);
        Destroy(gameObject);
                
     }


    public void MakeNoise(float Loudness) 
    
    {
        if (DistanceToPlayer < Loudness) 
        {
            IsMove = true;
        }

       
    
    }

    /*
   void OnTriggerEnter(Collider other)
   {
     //  other.gameObject.GetComponent<MakeNoise>().IsMove = true;
     if(other.gameObject.name == PlayerName) 
       {
           Ready = true;
       }
   }

   void OnTriggerExit(Collider other)
   {
       //  other.gameObject.GetComponent<MakeNoise>().IsMove = true;
       if (other.gameObject.name == PlayerName)
       {
           Ready = false;
       }
   }
   */



private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Loudness);
     
    }

    void HearingRange()
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, Loudness);
      
        foreach (Collider hitCollider in hitColliders)
        {
            if(hitCollider.gameObject.name == PlayerName) 
            {
             //   Debug.Log(hitCollider.gameObject.name);
                Ready = true;
            }


        }

        
       
    }

    void ReaytoFalse() 
    {
        if(ReadyTime > 0.0f) 
        {
            ReadyTime -= Time.deltaTime;
        }
        if (ReadyTime <= 0.0f)
        {
            Ready = false;
            ReadyTime = 1.0f;
        }
    }

  


    void SeePlayer() 
    {


        RaycastHit hit;
        float range = 1000f;
        Vector3 fromPosition = Pos.transform.position;
        Vector3 toPosition = new Vector3(Player.transform.position.x, Player.transform.position.y + 1, Player.transform.position.z);
        Vector3 direction = toPosition - fromPosition;

        Debug.DrawRay(Pos.position, direction, Color.cyan);
       
        if (Physics.Raycast(Pos.position, direction, out hit, range))
        {

            if (hit.transform.gameObject.name == PlayerName)
            {
                CanISee = true;
            }
            if (hit.transform.gameObject.name != PlayerName)
            {
                CanISee = false;
            }


        }



    }



    public void EnemyDamage(float Damage) 
    {
        Health -= Damage;
        IsMove = true;
       // Debug.Log(Health);
    }




}


