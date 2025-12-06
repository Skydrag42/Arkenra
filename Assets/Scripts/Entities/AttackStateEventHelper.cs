using UnityEngine;

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
