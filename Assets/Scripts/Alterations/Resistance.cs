using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Resistances
{
    public List<OneResistance> resistances;


    public OneResistance GetResistance(DamageType type)
    {
        return resistances.Find(x => (x.Type) == type);
    }

    public Resistances Copy()
    {
        Resistances newRes = new Resistances();
        newRes.resistances = new List<OneResistance>();
        foreach (OneResistance x in resistances)
            newRes.resistances.Add(x.Copy());

        return newRes;
    }
}
