using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [HideInInspector] public EntityData entityData = new EntityData();
    [SerializeField] protected EntityData defaultEntityData = new EntityData();

	[SerializeField] protected Resistances baseResistances;
    protected Resistances currentResistances;

    /*[HideInInspector] */public Gauge hp;

	/*[HideInInspector] */public Gauge ap;


    private Dictionary<DamageType, Gauge> buildups;

	private List<Effect> effects;

    [SerializeField] private GameObject poisonEffectPrefab;
    [SerializeField] private GameObject charmEffectPrefab;
    [SerializeField] private GameObject blightEffectPrefab;
    [SerializeField] private GameObject lightningEffectPrefab;
	Dictionary<DamageType, GameObject> effectPrefabs;

    
	public Action entityDied;

    public List<Transform> targetLocks;
    [HideInInspector]
    public AttackState attackState = AttackState.Nothing;


    [Header("UI")]
	[SerializeField] HealthBar healthBar;
	[SerializeField] HealthBar armorBar;
    public bool onlyShowBarsOnHit = false;
    public float timeBeforeHidingBars = 2;
    private Coroutine hideBars;

    private void Awake()
	{
        EntitiesManager.RegisterEntity(this);
     
        effects = new List<Effect>();
		
		// storing status effect prefabs in a dict for easier access.
		effectPrefabs = new Dictionary<DamageType, GameObject> 
		{
			[DamageType.Poison] = poisonEffectPrefab,
			[DamageType.Charm] = charmEffectPrefab,
			[DamageType.Lightning] = lightningEffectPrefab,
			[DamageType.Blight] = blightEffectPrefab
		};


        entityData = defaultEntityData.Copy();
        // initializing health points, armor points and buildup gauges
		hp = new Gauge((uint)entityData.healthPoints, (uint)entityData.healthPoints);
		ap = new Gauge((uint)entityData.armorPoints, (uint)entityData.armorPoints);

        // registering gauge events for updating ui
        if (healthBar)
        {
            healthBar.MaxValue = entityData.healthPoints;
            healthBar.Current = entityData.healthPoints;
            hp.valueChanged += x => healthBar.Current = (int)x;
            hp.maxValueChanged += x => healthBar.MaxValue = (int)x;
        }
		if (armorBar)
		{
			armorBar.MaxValue = entityData.armorPoints; 
			armorBar.Current = entityData.armorPoints;
			ap.valueChanged += x => armorBar.Current = (int)x;
			ap.maxValueChanged += x => armorBar.MaxValue = (int)x;
		}


        if (onlyShowBarsOnHit)
        {
            healthBar.HideHealthBar();
            if (armorBar != null) armorBar.HideHealthBar();
        }

        // adding missing resistances
        baseResistances.resistances.Sort((x, y) => x.Type.CompareTo(y.Type));
		for (int i = 0; i < Enum.GetValues(typeof(DamageType)).Length; i++)
		{
			if (baseResistances.resistances.Count == i)
			{
				baseResistances.resistances.Add(new OneResistance(0, 100, (DamageType)i));
			}
		}
        currentResistances = baseResistances.Copy();
		

		// since we just sorted resistances based on DamageType, we can directly access them instead of calling 
		// GetResistance() which would perform a linear search
		// (the gains here are marginals, but depending on how many entities are spawned at the same time, it could be worth)
        buildups = new Dictionary<DamageType, Gauge>
        {
            [DamageType.Poison] = new Gauge(0, (uint)baseResistances.resistances[(int)DamageType.Poison].BuildupResistanceValue),
            [DamageType.Charm] = new Gauge(0, (uint)baseResistances.resistances[(int)DamageType.Charm].BuildupResistanceValue),
            [DamageType.Lightning] = new Gauge(0, (uint)baseResistances.resistances[(int)DamageType.Lightning].BuildupResistanceValue),
            [DamageType.Blight] = new Gauge(0, (uint)baseResistances.resistances[(int)DamageType.Blight].BuildupResistanceValue)
        };

	}

    protected virtual void OnDisable()
    {
        EntitiesManager.DeregisterEntity(this);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="state">Legacy string param for when using UnityEvents that still doesn't support enums...</param>
    public virtual void SetAttackState(string state) 
    {
        attackState = System.Enum.Parse<AttackState>(state, true);
        SetAttackState(attackState);
    }
    public virtual void SetAttackState(AttackState state) { }

    public virtual void ReceiveAttack(Attack attack, int followUpState)
    {
        Damage damage;
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
        ApplyDamage(damage);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="oneDamage"></param>
    /// <returns>
    /// Returns exact damage dealt on health points and on armor points.
    /// </returns>
    public (int hpDamageValue, int apDamageValue) ApplyOneDamage(OneDamage oneDamage)
	{
		var damage = oneDamage.ComputeOneDamage(currentResistances, ap.CurrentValue);
		int hpDamageValue = damage.onHp - (int)hp.Substract(damage.onHp);
		int apDamageValue = damage.onAp - (int)ap.Substract(damage.onAp);
		DamageType type = oneDamage.Type;
        if (buildups.ContainsKey(type))
		{
			buildups[type].Add(damage.onBuildup);
			CheckBuildup(type);
		}
        return (hpDamageValue, apDamageValue);
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>
    /// The exact damage dealt, split for each damage type by damage on armor points (onApMultiplier = 1) and damage on health points (onHpMultiplier = 1).
    /// </returns>
    public Damage ApplyDamage(Damage damage)
	{
        Damage damageDealt = new Damage(new List<OneDamage>());
        foreach (OneDamage oneDamage in damage.damageList)
        {
            var oneDamageDealt = ApplyOneDamage(oneDamage);
            damageDealt.damageList.Add(new OneDamage(oneDamage.Type, oneDamageDealt.hpDamageValue, false, 1, 0, 0));
            damageDealt.damageList.Add(new OneDamage(oneDamage.Type, oneDamageDealt.apDamageValue, false, 0, 1, 0));

			if (hp.IsEmpty())
            {
                entityDied?.Invoke();
                Die();
                return damageDealt;
            }

            if (oneDamage.Type != DamageType.Healing)
            {
                // we were hit, we stop any ongoing shield regen and restart the shield regen cooldown
                if (APRegenRoutine != null) StopCoroutine(APRegenRoutine);
                if (APRegenCooldownRoutine != null) StopCoroutine(APRegenCooldownRoutine);
                APRegenCooldownRoutine = StartCoroutine(APRegenCooldown());
            }
        }

        if (onlyShowBarsOnHit)
        {
            healthBar.ShowHealthBar();
            if (armorBar != null) armorBar.ShowHealthBar();

            if (hideBars != null)
                StopCoroutine(hideBars);
            hideBars = StartCoroutine(HideUIBars());
        }

        return damageDealt;
    }

    public void CheckBuildup(DamageType type)
    {
        Gauge buildup = buildups[type];
        if (buildup.IsFull())
        {
            EntityEffect e = Instantiate(effectPrefabs[type], transform).GetComponent<EntityEffect>();
            AddEffect(e);
            if (e.stackable)
            {
                buildup.CurrentValue = 0;
            }
            else
            {
                StartCoroutine(ResetBuildupAfterDelay(buildup, e.effectDuration));
            }
        }
    }

    private IEnumerator ResetBuildupAfterDelay(Gauge buildup, float delay)
    {
        yield return new WaitForSeconds(delay);
        buildup.CurrentValue = 0;
    }

    private IEnumerator HideUIBars()
    {
        yield return new WaitForSeconds(timeBeforeHidingBars);

        healthBar.HideHealthBar();
        if (armorBar != null) armorBar.HideHealthBar();
    }

    private Coroutine APRegenRoutine, APRegenCooldownRoutine;
    private IEnumerator APRegenCooldown()
    {
        yield return new WaitForSeconds(entityData.armorRegenCooldown);
        APRegenRoutine = StartCoroutine(RegenerateAP());
    }

    private IEnumerator RegenerateAP()
    {
        float remainder = 0;
        while (ap.CurrentValue < ap.MaxValue)
        {
            remainder += entityData.armorRegenSpeed * Time.deltaTime;
            float addedValue = MathF.Floor(remainder);
            ap.Add((int)addedValue);
            remainder -= addedValue;
            yield return null;
        }
    }


    protected virtual void Die()
    {
        Debug.Log("Entity " + name + " is dead !");
        gameObject.SetActive(false);
        // Ou Destroy(gameObject); pour le supprimer completement
    }


    public void AddEffect(Effect effect)
	{
		effects.Add(effect);
		ComputeCurrentResistances();
	}

	public void RemoveEffect(Effect effect)
	{
		Debug.Log("Removing effect");
		effects.Remove(effect);
		ComputeCurrentResistances();
	}

	protected void ComputeCurrentResistances()
	{
		currentResistances = baseResistances.Copy();
		foreach (Effect effect in effects)
		{
			effect.ComputeResistances(ref currentResistances);
		}

		foreach (DamageType type in buildups.Keys)
		{
			buildups[type].MaxValue = (uint)currentResistances.resistances[(int)type].BuildupResistanceValue;
            // we need to check current buildup value in the case where max buildup was decreased and went under current value
            CheckBuildup(type);
		}
	}
}
