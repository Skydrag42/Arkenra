using UnityEngine;

[System.Serializable]
public class EntityData
{
    public int healthPoints = 500;
    public int armorPoints = 0;

    [Tooltip("in ap/s")]
    public float armorRegenSpeed = 0;

    [Tooltip("How long to wait without taking damage before starting armor regen, in s")]
    public float armorRegenCooldown = 0;

    public EntityData Copy()
    {
        return (EntityData)MemberwiseClone();
    }
}
