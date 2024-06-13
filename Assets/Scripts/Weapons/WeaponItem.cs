using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviourPunCallbacks, IPunObservable
{
    public WeaponManager weapon;
    public string weaponName;


    private void Start()
    {
        if (weapon == null && weaponName != "")
        {
            weapon = Resources.Load<WeaponManager>("Scriptables/" + weaponName);
        }
    }

    public void SetWeapon(string _weaponName)
    {
        this.weaponName = _weaponName;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(weaponName);
        }
        else
        {
            weaponName = (string)stream.ReceiveNext();

            if (weapon == null && weaponName != "")
            {
                weapon = Resources.Load<WeaponManager>("Scriptables/" + weaponName);
            }
        }
    }
}
