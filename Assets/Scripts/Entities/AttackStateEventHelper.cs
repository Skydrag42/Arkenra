using LogHelper;
using System;
using UnityEngine;

[Serializable]
public enum AttackState
{
    Nothing,
    PreparingAttack,
    Attacking,
    FinishingAttack
}

public class AttackStateEventHelper : MonoBehaviour
{
    public Entity entity;
	public AttackHolder attackHolder;

    public void SetAttackState(string state)
	{
		//Debug.Log($"Attack state: {state}" % Colorize.Magenta);
		entity.SetAttackState(state);
	}

	public void SetFollowUpState(int state)
	{
		attackHolder.SetFollowUpState(state);
	}
}
