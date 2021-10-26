using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject theEnemy;
    public LayerMask whatIsPlayer;
    public int enemyCount;
    public int TotalEnemyCount;



    public float zombieSpawnRange = 10f;
    private float randomSpawnRangeXPos;
    private float randomSpawnRangeZPos;
    public float ZombieSpawnTime;

    public bool playerInSpawnZone;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnemyDrop());
    }

    private void Update()
    {

        playerInSpawnZone = Physics.CheckBox(transform.position, new Vector3((zombieSpawnRange * 2f + 10f) / 2, transform.position.y, (zombieSpawnRange * 2f + 10f) / 2), Quaternion.identity, whatIsPlayer);




    }




    IEnumerator EnemyDrop()
    {
        while (enemyCount < TotalEnemyCount)
        {
            //Creates a random point inside of the Zombiespawnrange where the zombie can spawn
            randomSpawnRangeXPos = Random.Range(-zombieSpawnRange, zombieSpawnRange);
            randomSpawnRangeZPos = Random.Range(-zombieSpawnRange, zombieSpawnRange);
            //Spawns the zombie at a random position
            Instantiate(theEnemy, new Vector3(transform.position.x + randomSpawnRangeXPos,transform.position.y,transform.position.z + randomSpawnRangeZPos), Quaternion.identity);

            yield return new WaitForSeconds(ZombieSpawnTime);

            enemyCount += 1;

        }


    }



    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(zombieSpawnRange * 2f, transform.position.y, zombieSpawnRange * 2f));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(zombieSpawnRange * 2f + 10f, transform.position.y, zombieSpawnRange * 2f +10f));

    }

}
