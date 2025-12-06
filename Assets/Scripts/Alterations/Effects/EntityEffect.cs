using UnityEngine;

public abstract class EntityEffect : Effect
{
	/// <summary>
	/// Entity to which damage should be applied if any.
	/// </summary>
	public Entity linkedEntity;

	public float effectDuration;

    public bool stackable;

	public override void LinkEffectToEntity(Entity entity)
	{
		base.LinkEffectToEntity(entity);
		linkedEntity = entity;
	}
}
