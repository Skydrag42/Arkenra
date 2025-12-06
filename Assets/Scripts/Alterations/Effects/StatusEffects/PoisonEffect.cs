using System.Collections;
using UnityEngine;

public class PoisonEffect : DamagingEntityEffect
{

	[Header("Damage dealing")]
	public float damageDelay = 1f;

	public override void ComputeResistances(ref Resistances resistances)
	{
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
