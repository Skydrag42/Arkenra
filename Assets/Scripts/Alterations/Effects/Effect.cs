using UnityEngine;

public abstract class Effect : MonoBehaviour
{
	public abstract void ComputeResistances(ref Resistances originalResistances);

	public virtual void LinkEffectToEntity(Entity entity)
	{
		entity.AddEffect(this);
	}

	public virtual void UnLinkEffectToEntity(Entity entity)
	{
		entity.RemoveEffect(this);
	}
}
