using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviourPunCallbacks
{
    int playersInside = 0;

    public void BackMenu()
    {
        SceneManager.LoadScene(0);
        PhotonNetwork.Disconnect();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playersInside++;
            if(playersInside == 2)
            {
                ActivateTrigger();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playersInside--;
        }
    }

    private void ActivateTrigger()
    {
        PhotonNetwork.LoadLevel(3);
    }
}
