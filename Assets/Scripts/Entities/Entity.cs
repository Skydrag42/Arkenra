using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
	public int maxHealth = 500;
	public int health = 500;
	public HealthBar healthBar;
	public float damageResistance = .2f;

	public List<Transform> targetLocks;
	[HideInInspector] 
	public AttackState attackState = AttackState.Nothing;

	public Action entityDied;


	protected virtual void Awake()
	{
		EntitiesManager.RegisterEntity(this);
		healthBar.MaxValue = maxHealth;
		healthBar.Current = health;
	}

	protected virtual void OnDestroy()
	{
		EntitiesManager.DeregisterEntity(this);
	}

	public Transform GetTargetLock()
	{
		if (targetLocks == null || targetLocks.Count == 0) return transform;
		else 
			return targetLocks[0];
	}

	public virtual void TakeDamage(Attack attack, int followUpState)
	{
		int damage;
		if (followUpState == 0) damage = attack.baseDamage;
		else
		{
			if (attack.followUpBaseDamage.Count >= followUpState)
				damage = attack.followUpBaseDamage[followUpState - 1];
			else
			{
				damage = attack.baseDamage;
				Debug.LogError("Invalid follow up state for attack : index out of bounds.\nDefaulting to baseDamage.");
			}
		}
		damage = (int)(damage * (1 - damageResistance));
		health -= damage;
		if (health < 0)
		{
			health = 0;
			entityDied?.Invoke();
		}
		healthBar.SetNewDamageValue(damage);
		healthBar.Current = health;
	}

	public virtual void SetAttackState(string state) { }
}

public enum AttackState
{
	Nothing,
	PreparingAttack,
	Attacking,
	FinishingAttack
}