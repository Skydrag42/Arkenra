using System.Collections.Generic;
using UnityEngine;

public class Boss : Entity
{
    protected Transform player;
    protected Rigidbody rb;

    public Animator animator;

    [Tooltip("How fast will the boss rotate to face the player in degrees/s.")]
    public float lookRotationSpeed = 10f;

    [Tooltip("Should the boss rotate to face the player.")]
    public bool allowLookAtPlayer = true;
    protected bool lookAtPlayer = true;

    [Tooltip("Should the boss move to math the preferred attack range.")]
    public bool allowMove = true;
    protected bool move = true;
    
    [Tooltip("Should the boss rotate (in arcs) around the player.")]
    public bool allowRotateAroundPlayer = false;
    protected bool rotateAroundPlayer = false;
    public Vector2 rotateAroundDelay = new Vector2(2f, 4f);
    private float currentRotateAroundDelay, rotateAroundTimer;
    public Vector2 rotationRange = new Vector2(10f, 60f);
    public float rotationSpeed = 5f;
    private Vector3 rotateTarget;
    private bool isRotating = false;

    private float rotateAroundTimeout, rotateAroundTimeoutValue;


	public float movementSpeed = 2f;
    public LayerMask groundMask;

    [Header("Attacks")]
    
    public AttackHolder attackHolder;
    public List<AttackListElement> closeRangeAttacks = new List<AttackListElement>();
    public List<AttackListElement> midRangeAttacks = new List<AttackListElement>();
    public List<AttackListElement> longRangeAttacks = new List<AttackListElement>();

    public Range preferredRange = Range.Close;
    private float preferredDistance = 0;
    public Vector2 attackDelay = new Vector2(1f, 15f);
    protected float attackTimer, currentAttackDelay;
    protected bool isAttacking = false;

    public AttackRange attackRange;

    protected Range currentRange = Range.Far;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        SortAttackLists();
        SetNewAttackDelay();
        SetPreferredDistance();
        SetRotateAroundTimer();

        entityDied += OnBossDied;
    }

	protected override void OnDestroy()
	{
        base.OnDestroy();
		entityDied -= OnBossDied;
	}

	protected void SortAttackLists()
	{
        closeRangeAttacks.Sort((e, n) => e.probability.CompareTo(n.probability));
        midRangeAttacks.Sort((e, n) => e.probability.CompareTo(n.probability));
        longRangeAttacks.Sort((e, n) => e.probability.CompareTo(n.probability));
    }

    protected void SetNewAttackDelay()
	{
        currentAttackDelay = Random.Range(attackDelay.x, attackDelay.y);
        attackTimer = 0f;
    }

    protected void SetPreferredDistance()
	{
        if (preferredRange == Range.Close)
            preferredDistance = (attackRange.min + attackRange.closeLimit) / 2f;
        else if (preferredRange == Range.Mid)
            preferredDistance = (attackRange.closeLimit + attackRange.midLimit) / 2f;
        else if (preferredRange == Range.Long)
            preferredDistance = (attackRange.midLimit + attackRange.longLimit) / 2f;
        else if (preferredRange == Range.Far)
            preferredDistance = attackRange.longLimit * 1.2f;
    }

    private void Update()
    {

        CheckRange();
        
        if (!isAttacking)
        {
            if (attackTimer >= currentAttackDelay)
            {
                if (ChooseAttack())
                {
                    isAttacking = true;
                }
                SetNewAttackDelay();
            }
            else
            {
                attackTimer += Time.deltaTime;
            }
        }

        if (rotateAroundPlayer)
        {
            if (isRotating)
            {
                rotateAroundTimeout += Time.deltaTime;
                if (rotateAroundTimeout > rotateAroundTimeoutValue)
                {
                    isRotating = false;
                    SetRotateAroundTimer();
                }
            }
            else
            {
                if (rotateAroundTimer > currentRotateAroundDelay)
                {
                    ComputeRotateAround();
                }
                else
                {
                    rotateAroundTimer += Time.deltaTime;
                }
            }
        }
    }

    RaycastHit hitInfo;
    Vector3 groundNormal;
    private void FixedUpdate()
	{
        groundNormal = Vector3.up;
		if (Physics.Raycast(transform.position + Vector3.up * .2f, Vector3.down, out hitInfo, 5f, groundMask))
		{
            groundNormal = hitInfo.normal;
		}

		#region look at player
		Vector3 lookVector = player.position - transform.position;
        lookVector.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(lookVector);

        if (lookAtPlayer)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * lookRotationSpeed);
        }
        #endregion

        #region move
        if (move)
        {
            Vector3 target = player.position + (transform.position - player.position).normalized * preferredDistance;
            target.y = transform.position.y;
            if (Vector3.Distance(transform.position, target) > .5f)
            {
                Vector3 movement = Vector3.ProjectOnPlane(target - transform.position, groundNormal).normalized;
                rb.MovePosition(transform.position + movement * movementSpeed * Time.deltaTime);
            }
        }
        #endregion

        #region rotate around player
        if (rotateAroundPlayer && isRotating)
		{
            rotateTarget.y = transform.position.y;
            Vector3 movement = Vector3.ProjectOnPlane(rotateTarget - transform.position, groundNormal).normalized;
            rb.MovePosition(transform.position + movement * rotationSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, rotateTarget) < .2f)
			{
                isRotating = false;
                SetRotateAroundTimer();
			}
		}
        #endregion
    }

    protected void SetRotateAroundTimer()
	{
        rotateAroundTimer = 0f;
        currentRotateAroundDelay = Random.Range(rotateAroundDelay.x, rotateAroundDelay.y);
	}

    protected void ResetRotateAround()
	{
        isRotating = false;
        SetRotateAroundTimer();
	}

    protected void ComputeRotateAround()
	{
        float angle = (Random.value < 0.5f ? -1 : 1) * Random.Range(rotationRange.x, rotationRange.y);
        Vector3 direction = transform.position - player.position;
        rotateTarget = player.position + Quaternion.Euler(0, angle, 0) * direction;
        rotateAroundTimeoutValue = Vector3.Distance(rotateTarget, transform.position) / rotationSpeed + .5f;
        rotateAroundTimeout = 0;
        isRotating = true;
    }

    public override void SetAttackState(string state)
	{
        attackState = System.Enum.Parse<AttackState>(state, true);
        if (attackState == AttackState.Nothing)
		{
            move = allowMove;
            rotateAroundPlayer = allowRotateAroundPlayer;
            lookAtPlayer = allowLookAtPlayer;
            isAttacking = false;
		}
        else if (attackState == AttackState.Attacking)
		{
            move = false;
            rotateAroundPlayer = false;
            lookAtPlayer = false;
		}
        else if (attackState == AttackState.PreparingAttack)
		{
            move = false;
            rotateAroundPlayer = false;
            lookAtPlayer = allowLookAtPlayer;
		}
        else if (attackState == AttackState.FinishingAttack)
		{
            move = false;
            rotateAroundPlayer = false;
            lookAtPlayer = allowLookAtPlayer;
        }
	}

    public void CheckRange()
	{
        float distance = (player.position - transform.position).magnitude;
        if (distance < attackRange.closeLimit) currentRange = Range.Close;
        else if (distance < attackRange.midLimit) currentRange = Range.Mid;
        else if (distance < attackRange.longLimit) currentRange = Range.Long;
        else currentRange = Range.Far;
	}

    public bool ChooseAttack()
	{
        Attack attack = null;
        if (currentRange == Range.Close)
		{
            attack = GetRandomAttackInList(closeRangeAttacks);
		}
        else if (currentRange == Range.Mid)
        {
            attack = GetRandomAttackInList(midRangeAttacks);
        }
        else if (currentRange == Range.Long)
        {
            attack = GetRandomAttackInList(longRangeAttacks);
        }

        attackHolder.SetCurrentAttack(attack);
        if (attack != null)
            animator.SetTrigger(attack.attackName);
        
        return attack != null;
    }

    private Attack GetRandomAttackInList(List<AttackListElement> list)
	{
        if (list.Count == 0) return null;
        float rng = Random.value;
        float p = 0;
        for (int i = 0; i < list.Count; i++)
		{
            p += list[i].probability;
            if (rng <= p)
                return list[i].attack;
		}
        return null;
    }

    public virtual void OnBossDied()
	{
        Destroy(gameObject);
	}

}

[System.Serializable]
public class AttackRange
{
    public float min = 0, closeLimit = 3, midLimit = 6, longLimit = 10;
}

[System.Serializable]
public class AttackListElement
{
    public Attack attack;
    [Tooltip("Should be a value between 0 and 1. The sum of these values in a list of AttackListELement should be 1.")]
    public float probability;
}

public enum Range
{
    Close, Mid, Long, Far
}
