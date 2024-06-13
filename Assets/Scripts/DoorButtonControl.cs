using Photon.Pun.Demo.SlotRacer.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Photon.Pun.Demo.PunBasics
{
    public class DoorButtonControl : MonoBehaviourPunCallbacks, IPunObservable
    {
        public bool pressed;


        private void OnTriggerEnter(Collider other)
        {
            pressed = (photonView.IsMine && other.tag.Equals("Player"));
        }
        
        private void OnTriggerExit(Collider other)
        {
            pressed = !(photonView.IsMine && other.tag.Equals("Player"));
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // ! ENVIO

                stream.SendNext(pressed);
            }
            else
            {
                // ! RECIBO

                pressed = (bool)stream.ReceiveNext();
            }
        }
    }
}