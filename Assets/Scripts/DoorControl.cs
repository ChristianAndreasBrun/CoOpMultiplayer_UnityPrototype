using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace Photon.Pun.Demo.PunBasics
{
    public class DoorControl : MonoBehaviourPunCallbacks
    {
        public Transform closePoint, openPoint;
        public DoorButtonControl[] buttons;
        public float moveSpeed;

        public bool doorState;


        void Start()
        {

        }

        void Update()
        {
            doorState = setDoorState();
            Transform finalTransform = doorState ? openPoint: closePoint;
            transform.position = Vector3.MoveTowards(transform.position, finalTransform.position, moveSpeed * Time.deltaTime);
        }

        bool setDoorState()
        {
            if (doorState) return true;

            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i].pressed == false) return false;   
            }
            return true;
        }
    }
}
