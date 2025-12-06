using UnityEngine;

[System.Serializable]
public class OneResistance
{
    [SerializeField] private DamageType type;

    public DamageType Type
    {
        get => type;
        set => type = value;
    }

    public OneResistance(int resistanceValue, int buildupResistanceValue, DamageType type)
    {
        this.resistanceValue = resistanceValue;
        this.buildupResistanceValue = buildupResistanceValue;
        this.type = type;
    }

    /// <summary>
    /// Percentage of damage reduction: 100 means no damage goes through, -100 means taking double damage
    /// </summary>
    [SerializeField] private int resistanceValue;
    public int ResistanceValue
    {
        get => resistanceValue;
        set => resistanceValue = value > 100 ? 100 : value;
    }

    [SerializeField] private int buildupResistanceValue; // maybe if negative doesn't count
    public int BuildupResistanceValue
    {
        get => buildupResistanceValue;
        set => buildupResistanceValue = value > 100 ? 100 : value; //see later for values
    }

	/// <summary>
	/// Creates a deep copy of the object using MemberwiseClone (with primitive types).
	/// </summary>
	/// <remarks>
	/// <i>Note: If non-primitive types are added to the class, the method will then act as a shallow copy.</i>
	/// </remarks>
	/// <returns></returns>
	public OneResistance Copy()
    {
        return (OneResistance)MemberwiseClone();
    }
}
