using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoreKey : MonoBehaviourPunCallbacks, IPunObservable
{
    public int maxHealth;
    public Image healthFill;

    public int currentHealth;


    private void Start()
    {
        currentHealth = maxHealth;
        healthFill.fillAmount = 1f;
    }


    [PunRPC]
    public void RPC_CoreTakeDamage(int damage)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }

        float healthPercent = (float)currentHealth / maxHealth;
        healthFill.fillAmount = healthPercent;
    }

    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ENVIO
            stream.SendNext(currentHealth);
            stream.SendNext(healthFill.fillAmount);
        }
        else
        {
            // RECIBO
            currentHealth = (int)stream.ReceiveNext();
            healthFill.fillAmount = (float)stream.ReceiveNext();
        }
    }
}
