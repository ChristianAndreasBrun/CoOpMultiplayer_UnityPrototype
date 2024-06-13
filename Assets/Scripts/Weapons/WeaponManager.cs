using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : ScriptableObject
{
    //public GameObject weaponModel;
    public string id;
    public int shootDamage;
    public float fireRate;

    protected Transform target;
    protected float fireTime;


    public void InitWeapon(Transform target)
    {
        this.target = target;
    }

    public virtual void UpdateWeapon()
    {
        InitWeapon(target);
    }
}
