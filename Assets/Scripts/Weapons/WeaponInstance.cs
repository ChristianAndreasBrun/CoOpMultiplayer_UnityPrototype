using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInstance : MonoBehaviour
{
    public Vector2 spawnArea;
    public float spawnTime;
    public GameObject prefabWeapon;
    public List<WeaponManager> weapons;

    GameObject currentInstance;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InvokeRepeating(nameof(CreateWeapons), 1, spawnTime);
        }
    }

    void CreateWeapons()
    {
        if (currentInstance != null) return;
        
        int randomWeapon = Random.Range(0, weapons.Count);

        float randomX = transform.position.x + Random.Range(-spawnArea.x /2, spawnArea.x /2);
        float randomZ = transform.position.z + Random.Range(-spawnArea.y /2, spawnArea.y /2);
        Vector3 finalPosition = new Vector3(randomX, transform.position.y, randomZ);

        currentInstance = PhotonNetwork.Instantiate(prefabWeapon.name, finalPosition, Quaternion.Euler(0,0,0));
        currentInstance.GetComponent<WeaponItem>().SetWeapon(weapons[randomWeapon].id);
    }
}
