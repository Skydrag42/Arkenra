using UnityEngine;

public abstract class DamagingEntityEffect : EntityEffect
{
    public Damage damage;

	public void ApplyDamageToEntity()
	{
		if (linkedEntity == null) return;

		linkedEntity.ApplyDamage(damage);
	}
}