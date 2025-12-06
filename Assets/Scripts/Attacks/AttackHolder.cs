using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHolder : MonoBehaviour
{
	public Attack currentAttack;

	public int currentFollowUpState = 0;

	public bool damageDealt = false;

	public void SetFollowUpState(int state)
	{
		damageDealt = false;
		if (state >= 0)
			currentFollowUpState = state;
	}

	public void SetCurrentAttack(Attack attack)
	{
		currentAttack = attack;
		damageDealt = false;
	}
}
