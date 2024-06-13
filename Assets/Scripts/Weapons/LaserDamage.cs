using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDamage : MonoBehaviourPunCallbacks
{
    public int damagePerTick;
    public float tickInterval;


    private void Start()
    {
        InvokeRepeating(nameof(InflictDamage), 0.1f, tickInterval);
    }


    void InflictDamage()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, transform.localScale / 2f);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                PlayerControl player = collider.GetComponent<PlayerControl>();
                if (player != null)
                {
                    player.RPC_GetDamage(damagePerTick);
                }
            }
        }
    }
}
