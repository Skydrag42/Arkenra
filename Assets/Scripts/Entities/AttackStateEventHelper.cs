using UnityEngine;

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
		entity.SetAttackState(state);
	}

	public void SetFollowUpState(int state)
	{
		attackHolder.SetFollowUpState(state);
	}
}
