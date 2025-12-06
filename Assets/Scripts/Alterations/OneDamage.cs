using UnityEngine;

[System.Serializable]
public class OneDamage
{
    [SerializeField] private DamageType type;
    public DamageType Type
    {
        get => type;
        set => type = value;
    }

    [SerializeField] private int value;
    public int Value
    {
        get => value;
    }

    [SerializeField] private bool ignoreArmor;
    public bool IgnoreArmor { get => ignoreArmor; set => ignoreArmor = value; }
    [SerializeField] private float onHpMultiplier = 1;
    public float OnHpMultiplier { get => onHpMultiplier; set => onHpMultiplier = value; }
    [SerializeField] private float onApMultiplier = 1;
    public float OnApMultiplier { get => onApMultiplier; set => onApMultiplier = value; }
    [SerializeField] private int buildupValue;
    public int BuildupValue { get => buildupValue; set => buildupValue = value; }

    public OneDamage(DamageType type, int value, bool ignoreArmor, float onHpMultiplier, float onApMultiplier, int buildupValue)
    {
        this.type = type;
        this.value = value;
        this.ignoreArmor = ignoreArmor;
        this.onHpMultiplier = onHpMultiplier;
        this.onApMultiplier = onApMultiplier;
        this.buildupValue = buildupValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resistance">
    /// resistance should not be given with <c>in</c> keyword as they are not supposed to be modified.
    /// </param>
    /// <param name="armorPoints"></param>
    /// <returns></returns>
    public (int onHp, int onAp, int onBuildup) ComputeOneDamage(in Resistances resistance, uint armorPoints)
    {
        OneResistance res = resistance.GetResistance(type);
        int resistanceValue = res != null ? res.ResistanceValue : 0;
		int buildupResistanceValue = res != null ? res.BuildupResistanceValue : 0;

		int totalDamage = (int)(value * (1 - resistanceValue / 100f));
		int buildupDamage = (int)(buildupValue * (1 - buildupResistanceValue / 100f));
        if (!ignoreArmor)
        {
            if ((totalDamage * onApMultiplier) > armorPoints)
            {
                int onAp = (int)armorPoints;
                int temp = (int)((totalDamage * onApMultiplier - armorPoints) / onApMultiplier);
                int onHp = (int)(temp * onHpMultiplier);
                return (onHp,onAp,buildupDamage);
            }
            return (0, (int)(totalDamage * onApMultiplier), buildupDamage);
        }
        return ((int)(totalDamage * onHpMultiplier), 0,buildupDamage);
    }


    /// <summary>
    /// Adding two OneDamage will only add <c>Value</c> and <c>buildupValue</c> attributes, 
    /// meaning all other attributes will be those of <c>left</c> param.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static OneDamage operator +(OneDamage left, OneDamage right)
    {
		OneDamage res = new OneDamage(
            left.type,
            left.value + right.value,
            left.ignoreArmor,
            left.onHpMultiplier,
            left.onApMultiplier,
            left.buildupValue + right.buildupValue
        );
        return res;
    }

    /// <summary>
    /// Multiplying a OneDamage by a number will multiply its <c>value</c> <b>and</b> its <c>buildupValue</c> by this number.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static OneDamage operator *(OneDamage left, float right)
    {

        OneDamage res = new OneDamage(
            left.type,
            (int)(left.value * right),
            left.ignoreArmor,
            left.onHpMultiplier,
            left.onApMultiplier,
            (int)(left.buildupValue + right)
        );
        return res;
    }
    public static OneDamage operator *(float left, OneDamage right) => right * left;


    public OneDamage Copy()
    {
        return (OneDamage)MemberwiseClone();
    }
}

