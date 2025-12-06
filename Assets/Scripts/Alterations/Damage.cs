using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Damage
{
    public List<OneDamage> damageList;

    public Damage(List<OneDamage> damageList)
    {
        this.damageList = damageList;
    }

    public static Damage operator +(Damage left, Damage right)
    {
        Damage result = new Damage();
        result.damageList = new List<OneDamage>();

        foreach (OneDamage oneDamage in left.damageList)
        {
            result.damageList.Add(oneDamage.Copy());
        }

        foreach (OneDamage oneDamage in right.damageList)
        {
			int i;
			if ((i = result.damageList.FindIndex(x => x.Type == oneDamage.Type)) != -1)
			{
                result.damageList[i] += oneDamage;
			}
            else
            {
                result.damageList.Add(oneDamage);
            }
		}

        return result;
    }

    public static Damage operator *(Damage left, float right)
    {
        Damage result = left.Copy();

        for (int i = 0; i < left.damageList.Count; i++)
        {
            result.damageList[i] *= right;
        }

        return result;
    }
    public static Damage operator *(float left, Damage right) => right * left;


    public Damage Copy()
    {
        Damage damage = new Damage();
        damage.damageList = new List<OneDamage>();

        foreach(OneDamage oneDamage in damageList)
        {
            damage.damageList.Add(oneDamage.Copy());
        }
        return damage;
    }
}
