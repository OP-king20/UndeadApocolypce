using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;


    //Reference
    public Camera fpsCam;
    public RaycastHit rayHit;

    //Patroling
    public Vector3 walkpoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks, attackDamage;
    bool alreadyAttacked;
   

    //States
    public float sightRange, attackRange, chasingrange;
    public bool playerInSightRange, playerInAttackrange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

        //check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
       playerInAttackrange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if(!playerInSightRange && !playerInAttackrange)
        {
            Patroling();
        }
        else if(playerInSightRange && !playerInAttackrange)
        {
            ChasePlayer();
        }
        else if(playerInAttackrange)
        {
            AttackPlayer();
        }


    }



    //function for patroling
    void Patroling()
    {
        if(!walkPointSet)
        {
            SearchWalkPoint();
        }
        else if(walkPointSet)
        {
            agent.SetDestination(walkpoint);
        }

        Vector3 distanceToWalkpoint = transform.position - walkpoint;
        if(distanceToWalkpoint.magnitude < 1f)
        {
            walkPointSet = false;
        }

    }

    void SearchWalkPoint()
    {
        //Calculate a random point if range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkpoint = new Vector3(transform.position.x + randomX, transform.position.y,transform.position.z + randomZ);

        if(Physics.Raycast(walkpoint, -transform.up, 2f, whatIsGround))
        {
            walkPointSet = true;
        }

    }

    //function for chansing player
    void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    //function for attacking player
    void AttackPlayer()
    {
        //Make sure the enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            //AttackCode
            Debug.Log("AttackPlayer");

            Vector3 direction = fpsCam.transform.forward;

            //RayCast
            if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, attackRange))
            {
                Debug.Log(rayHit.collider.name);
          

                if (rayHit.collider.CompareTag("Player"))
                {

    
                    //Setup damage Function
                    Debug.Log("Hit Player");
                    TargetHealth target = rayHit.transform.GetComponent<TargetHealth>();
                   
                    if (target != null)
                    {
                        target.TakeDamage(attackDamage);

                    }
                }

            }







            //Reset the attack
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }




    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chasingrange);
    }


}
