using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviourPunCallbacks
{
    public GameObject enemy;
    public Transform[] spawnPoints;
    public int maxEnemies;

    int currentEnemies;


    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    IEnumerator SpawnEnemies()
    {
        while (currentEnemies < maxEnemies)
        {
            yield return new WaitForSeconds(3f);

            if (PhotonNetwork.IsMasterClient)
            {
                if (currentEnemies < maxEnemies)
                {
                    int spawnIndex = Random.Range(0, spawnPoints.Length);
                    PhotonNetwork.Instantiate(enemy.name, spawnPoints[spawnIndex].position, Quaternion.identity);
                    currentEnemies++;
                }
            }
        }
    }
}

