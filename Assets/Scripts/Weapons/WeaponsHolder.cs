using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponsHolder : MonoBehaviour
{
    public Weapon activeWeapon;
    public Weapon offHandWeapon;

    [Tooltip("Must be sorted based on wepontType enum")]
    public Transform[] backHolders;
    public Transform leftHand, rightHand;

    public WeaponEventHelper eventHelper;

    public void SwitchOffhandToActive()
	{
        int holder = (int)activeWeapon.type;
        activeWeapon.transform.SetParent(backHolders[holder]);
        activeWeapon.transform.localPosition = Vector3.zero;
        activeWeapon.transform.localRotation = Quaternion.identity;


        offHandWeapon.transform.SetParent(rightHand);
        offHandWeapon.transform.localPosition = offHandWeapon.activePosition;
        offHandWeapon.transform.localRotation = Quaternion.Euler(offHandWeapon.activeRotation);

        Weapon buffer = activeWeapon;
        activeWeapon = offHandWeapon;
        offHandWeapon = buffer;

        eventHelper.SwitchWeapon(activeWeapon);
	}

    public void ChangeOffHand(GameObject weaponPrefab)
	{
        if (offHandWeapon) Destroy(offHandWeapon.gameObject);
        offHandWeapon = Instantiate(weaponPrefab).GetComponent<Weapon>();
        offHandWeapon.transform.SetParent(backHolders[(int)offHandWeapon.type]);
        offHandWeapon.transform.localPosition = Vector3.zero;
        offHandWeapon.transform.localRotation = Quaternion.identity;
    }

    public void ChangeActive(GameObject weaponPrefab)
	{
        if (activeWeapon) Destroy(activeWeapon.gameObject);
        activeWeapon = Instantiate(weaponPrefab).GetComponent<Weapon>();
		activeWeapon.transform.SetParent(rightHand);
        activeWeapon.transform.localPosition = activeWeapon.activePosition;
        activeWeapon.transform.localRotation = Quaternion.Euler(activeWeapon.activeRotation);

        eventHelper.SwitchWeapon(activeWeapon);
    }
}
