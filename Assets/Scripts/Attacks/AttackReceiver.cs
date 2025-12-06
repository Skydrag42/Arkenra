using LogHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AttackReceiver : MonoBehaviour
{
	public Entity entity;

	private void OnTriggerEnter(Collider other)
	{
		AttackHolder attackHolder = other.GetComponentInParent<AttackHolder>();
		if (attackHolder != null && attackHolder.transform != transform && attackHolder.GetComponent<Entity>().attackState == AttackState.Attacking)
		{
			Attack attack = attackHolder.currentAttack;
			if (attack.isMultiHit || !attackHolder.damageDealt)
			{
				Debug.Log("Sending attack to entity " % Colorize.Olive + entity.name % Colorize.Cyan);
				entity.ReceiveAttack(attack, attackHolder.currentFollowUpState);
				attackHolder.damageDealt = true;
			}
		}
	}
}
