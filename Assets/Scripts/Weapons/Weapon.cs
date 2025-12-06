using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponType type;
    public AttackSet attackSet;

    public MonoBehaviour[] vfxs;
    public Collider[] xboxs;

    public Vector3 activePosition;
    // needs to be vector3 for easier inspector use (quaternions can't be copy/pasted without custom inspector)
    public Vector3 activeRotation;

    // might need to consider dual weapons
    // bool isDual
}


public enum WeaponType
{
    Greatsword,
    Hand
}