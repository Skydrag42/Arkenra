using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEventHelper : MonoBehaviour
{
	private Weapon currentWeapon;

	public void ToggleXbox(int toggle)
	{
		if (!currentWeapon) return;
		foreach (Collider m in currentWeapon.xboxs)
		{
			m.enabled = toggle != 0;
		}
	}

	public void ToggleVfxs(int toggle)
	{
		if (!currentWeapon) return;
		foreach (MonoBehaviour m in currentWeapon.vfxs)
		{
			m.enabled = toggle != 0;
		}
	}

	public void SwitchWeapon(Weapon weapon)
	{
		currentWeapon = weapon;
	}
}

