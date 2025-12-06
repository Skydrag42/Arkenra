using System.Collections;
using UnityEngine;

public class TestStatusEffect : DamagingEntityEffect
{
	public int standardResistanceChange = -10;

	[Header("Damage dealing")]
	public float damageDelay = 1f;

	public override void ComputeResistances(ref Resistances resistances)
	{
		resistances.GetResistance(DamageType.Standard).ResistanceValue += standardResistanceChange;
	}

	private void Start()
	{
		StartCoroutine(DestroyEffect());
		StartCoroutine(DealDamage());
	}

	private IEnumerator DestroyEffect()
	{
		yield return new WaitForSeconds(effectDuration);
		linkedEntity?.RemoveEffect(this);
		Destroy(gameObject);
	}

	private IEnumerator DealDamage()
	{
		while (true)
		{
			yield return new WaitForSeconds(damageDelay);
			ApplyDamageToEntity();
		}
	}

}
