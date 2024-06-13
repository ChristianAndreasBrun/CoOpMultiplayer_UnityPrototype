using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")]
public class Weapon_Gun : WeaponManager
{
    RaycastHit hitShoot;


    public override void UpdateWeapon()
    {
        fireTime += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && fireTime >= fireRate)
        {
            if (Physics.Raycast(target.position, target.forward, out hitShoot, Mathf.Infinity))
            {
                Debug.DrawRay(target.position, target.TransformDirection(Vector3.forward) * hitShoot.distance, Color.green);
                Debug.Log("Did Hit");

                if (hitShoot.collider.tag.Equals("Enemy"))
                {
                    int damage = shootDamage;
                    hitShoot.collider.GetComponent<EnemyControl>().TakeDamage(damage);
                    //target.GetComponent<EnemyControl>().RPC_EnemyTakeDamage(shootDamage);
                    //target.GetComponent<EnemyControl>().SetEnemyDamage(hitShoot.collider.gameObject, shootDamage);
                }
                else if (hitShoot.collider.tag.Equals("Core"))
                {
                    int damage = shootDamage;
                    hitShoot.collider.GetComponent<CoreKey>().TakeDamage(damage);
                    //target.GetComponent<CoreKey>().RPC_CoreTakeDamage(shootDamage);
                }
                else if (hitShoot.collider.tag.Equals("Player") && hitShoot.collider.transform != target)
                {
                    int damage = shootDamage;
                    target.GetComponent<PlayerControl>().SetDamage(hitShoot.collider.gameObject, shootDamage);
                }

            }
            else
            {
                Debug.DrawRay(target.position, target.TransformDirection(Vector3.forward) * 1000, Color.red);
                Debug.Log("Did not Hit");
            }
        }
    }
}
