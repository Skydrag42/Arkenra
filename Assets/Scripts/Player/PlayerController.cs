using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using Cinemachine;

[RequireComponent(typeof(Rigidbody))]
public partial class PlayerController : Entity
{
	[Header("General")]
	public new SkinnedMeshRenderer renderer;
    private Rigidbody rb;
	public Collider xbox;
	public Animator animator;
	public PlayerInput input;
	public CinemachineFreeLook freeLookCam;
	public CinemachineVirtualCamera lockedCam;

	private Transform currentLockedEntity;
	public Transform lockedOnFollow;
	public RectTransform lockDotUI;
	private bool cameraIsLocked = false;

	[Header("Movement")]
	public bool allowMovement = true, allowRotation = true;
	public float moveSpeed = 3000;
	public float maxSpeed = 5;
	public float decelerationSpeed = 10;
	public float rotationSpeed = 20f;
	private Vector2 moveInput;

	[Range(0, 90)] public float maxSlopeAngle = 45f;

	[Header("Jump")]
	public bool allowJump = true;
	public float jumpForce = 5.5f;
	public int maxJumps = 1;
	private int remainingJumps = 1;
	private bool grounded = true;
	private bool jumpRequested = false;

	[Header("Dash")]
	public bool allowDash = true;
	public int maxDashes = 1;
	public float dashSpeed = 30f;
	public float dashDuration = .1f;
	public float dashCooldown = 1f;
	public float superDashPower = 15f;
	public float framePerfectMultiplicator = 1.2f;
	private int remainingDashes;
	private bool dashRequested;
	
	private bool _isSuperDashing;
	public bool IsSuperDashing
	{
		get { return _isSuperDashing; }
		set
		{
			_isSuperDashing = value;
			if (value)
			{
				superDashVFX.SetVector4("Color", superDashColor);
				superDashVFX.Reinit();
			}
			else
			{
				superDashVFX.Stop();
			}
		}
	}
	private bool _isDashing;
	public bool IsDashing
	{
		get { return _isDashing; }
		set
		{
			_isDashing = value;
			if (value)
			{
				xbox.enabled = false;
				dashVFX.SetVector4("Color", dashColor);
				dashVFX.Reinit();
			}
			else
			{
				xbox.enabled = true;
				dashVFX.Stop();
			}
		}
	}

	private bool inDashCooldown = false;
	private Coroutine dashingCoroutine;
	public VisualEffect dashVFX, superDashVFX;
	[ColorUsage(true, true)] public Color dashColor, superDashColor;

	[Header("Gravity - Ground check")]
	public Transform groundCheckOrigin;
	public float groundCheckRadius = .45f;
	public LayerMask groundLayer;
	private RaycastHit groundCollisionResult;

	public float additionalGravityForce = -9.81f;

	[Header("Death")]
	public GameObject deathUI;

	[Header("Spawn")]
	public LayerMask deadZoneLayer;
	private Transform globalSpawnPoint, currentSpawnPoint;
	public Transform defaultCam;
	public LayerMask checkpointLayer;

	[Header("Attacks")]
	public AttackSet currentAttackSet;
	private AttackHolder attackHolder;
	
	[Header("Weapons")]
	public WeaponsHolder weaponsHolder;
	// placeholder for inventory system:
	public GameObject temp_activeWeaponPrefab, temp_offHandWeaponPrefab;
	
	
	//[Header("Other")]

	private void Start()
	{
		freeLookCam.ForceCameraPosition(defaultCam.position, defaultCam.rotation);
		rb = GetComponent<Rigidbody>();
		attackHolder = GetComponent<AttackHolder>();

		deathUI.SetActive(false);
		entityDied += OnPlayerDied;

		weaponsHolder.ChangeActive(temp_activeWeaponPrefab);
		weaponsHolder.ChangeOffHand(temp_offHandWeaponPrefab);
		SwitchAttackSet(weaponsHolder.activeWeapon.attackSet);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		entityDied -= OnPlayerDied;
	}

	public void Deactivate()
	{
		input.DeactivateInput();
	}

	public void Activate()
	{
		input.ActivateInput();
	}

	public void Respawn(bool isGlobal = false)
	{
		if (isGlobal)
		{
			transform.position = globalSpawnPoint.position;
			transform.rotation = globalSpawnPoint.rotation;
			Deactivate();
		}
		else
		{
			transform.position = currentSpawnPoint.position;
			transform.rotation = currentSpawnPoint.rotation;
		}
		freeLookCam.ForceCameraPosition(defaultCam.position, defaultCam.rotation);
		ResetStatus();
	}

	public void ResetStatus()
	{
		if (rb)
		{
			rb.linearVelocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rb.useGravity = true;
		}
		IsDashing = false;
		IsSuperDashing = false;
		remainingDashes = maxDashes;
	}

	public void SetNewSpawnPoint(Transform spawn, bool isGlobal = false)
	{
		if (isGlobal) globalSpawnPoint = spawn;
		else currentSpawnPoint = spawn;
	}

	private void Update()
	{
		if (moveInput != Vector2.zero && allowMovement)
		{
			animator.SetBool("isRunning", true);
		}
		else
		{
			animator.SetBool("isRunning", false);
		}
		if (IsDashing || IsSuperDashing)
		{
			animator.SetBool("isDashing", true);
		}
		else
		{
			animator.SetBool("isDashing", false);
		}
	}

	void FixedUpdate()
    {
		UpdateGround();

		Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
		movement = Camera.main.transform.TransformDirection(movement);
		movement.y = 0;
		

		Move(movement);
		Dash(movement);
		Jump(movement);
		//Climb(movement);
    }

	private void LateUpdate()
	{
		if (cameraIsLocked)
		{
			if (currentLockedEntity == null)
			{
				LockCamera();
			}
			else
			{
				lockDotUI.position = Camera.main.WorldToScreenPoint(currentLockedEntity.position);
				lockedOnFollow.position = transform.position;
				lockedOnFollow.forward = (currentLockedEntity.position - new Vector3(lockedOnFollow.position.x, currentLockedEntity.position.y, lockedOnFollow.position.z)).normalized;
			}
		}
	}

	private void UpdateGround()
	{
		grounded = false;
		if (Physics.SphereCast(groundCheckOrigin.position, groundCheckRadius, Vector3.down,
								out groundCollisionResult, 0.2f, groundLayer))
		{
			if (Vector3.Angle(Vector3.up, groundCollisionResult.normal) < maxSlopeAngle)
			{
				grounded = true;
			}
		}
		animator.SetBool("isGrounded", grounded);
	}

	private void Move(Vector3 movement)
	{
		if (!IsSuperDashing && !IsDashing)
		{

			if (allowMovement)
			{
				rb.AddForce(movement * moveSpeed * Time.deltaTime);
			}

			Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
			if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
			{
				horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
				horizontalVelocity.y = rb.linearVelocity.y;
				rb.linearVelocity = horizontalVelocity;
			}
			if (moveInput == Vector2.zero || !allowMovement)
			{
				horizontalVelocity.y = 0;
				horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector2.zero, Time.deltaTime * decelerationSpeed);
				horizontalVelocity.y = rb.linearVelocity.y;
				rb.linearVelocity = horizontalVelocity;
			}
		}

		if (allowRotation)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation,
					Quaternion.LookRotation(movement != Vector3.zero ? movement : new Vector3(transform.forward.x, 0, transform.forward.z)),
					Time.deltaTime * rotationSpeed);
		}
	}

	private void Dash(Vector3 movement)
	{
		if (dashRequested)
		{
			if (remainingDashes > 0)
			{
				remainingDashes--;
				if (jumpRequested && grounded)
				{
					SuperDash(movement, framePerfectMultiplicator);
				}
				else
				{
					rb.linearVelocity = movement * dashSpeed;
					IsDashing = true;
					dashingCoroutine = StartCoroutine(Dashing());
					StartCoroutine(DashCooldown());
				}
			}
			dashRequested = false;
		}

		if (grounded)
		{
			if (!IsDashing && !IsSuperDashing && !inDashCooldown)
			{
				if (remainingDashes != maxDashes) StartCoroutine(BlinkShader());
				remainingDashes = maxDashes;
			}
		}
	}

	private void SuperDash(Vector3 movement, float multiplier = 1f)
	{
        Vector3 dashForce;
        if (movement != Vector3.zero)
        {
            dashForce = movement * superDashPower * multiplier;
            dashForce.y = jumpForce;
        }
        else
            dashForce = (transform.forward * superDashPower * multiplier + Vector3.up * jumpForce);
        jumpRequested = false;
        remainingJumps--;

		rb.linearVelocity = Vector3.zero;
        rb.AddForce(dashForce, ForceMode.Impulse);
        IsSuperDashing = true;
    }

	private void Jump(Vector3 movement)
	{
		if (grounded)
		{
			if (jumpRequested)
			{
				if (IsDashing)
				{
					if (dashingCoroutine != null) StopCoroutine(dashingCoroutine);
					IsDashing = false;
					SuperDash(movement);
				}
				else
				{
					if (remainingJumps > 0)
					{
						remainingJumps--;
						rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
						if (!IsDashing) animator.SetTrigger("jump");
					}
					jumpRequested = false;
				}
			}
			else
			{
				remainingJumps = maxJumps;
			}
		}
		else
		{
			jumpRequested = false;
			rb.AddForce(Vector3.up * additionalGravityForce * Time.deltaTime * Time.deltaTime);
		}
	}

	//private void WallJump(Vector3 movement)
	//{
	//	if (jumpRequested)
	//	{
	//		if (wasSuperDashing)
	//		{
	//			currentRebounds++;
	//			superReboundDirection = Vector3.Reflect(superReboundDirection, currentClimbNormal).normalized;
	//			Vector3 dashForce = (superReboundDirection * superDashPower
	//				* Mathf.Pow(superReboundVelocityMultiplier, currentRebounds)
	//				+ Vector3.up * jumpForce);
	//			rb.AddForce(dashForce, ForceMode.Impulse);
	//			if (currentRebounds == 1) 
	//			{
	//				remainingDashes++;
	//				StartCoroutine(BlinkShader());
	//			}
	//			IsSuperDashing = true;
	//			jumpRequested = false;
	//			climbRequested = false;
	//			ReleaseClimb();
	//			if (superReboundDelayRoutine != null) StopCoroutine(superReboundDelayRoutine);
	//			wasSuperDashing = false;
	//		}
	//	}
	//}

	private IEnumerator Dashing()
	{
		yield return new WaitForSeconds(dashDuration);
		IsDashing = false;
	}

	private IEnumerator DashCooldown()
	{
		inDashCooldown = true;
		yield return new WaitForSeconds(dashCooldown);
		inDashCooldown = false;
		// StartCoroutine(BlinkShader());
		// if (remainingDashes < maxDashes) StartCoroutine(DashCooldown());
	}

	public bool IsMissingDashes() => remainingDashes < maxDashes;

	public void ResetDash(int dashesAmount)
	{
		if (remainingDashes < maxDashes)
		{
			inDashCooldown = false;
			StartCoroutine(BlinkShader());
			remainingDashes += dashesAmount;
			if (remainingDashes > maxDashes) remainingDashes = maxDashes;
		}
	}

	private IEnumerator BlinkShader()
	{
		float t = 0;
		while (t < .5f)
		{
			t += Time.deltaTime;
			renderer.material.SetFloat("_BlinkValue", t/.5f);
			yield return null;
		}
		renderer.material.SetFloat("_BlinkValue", 0);
	}

	public void OnPlayerDied()
	{
		deathUI.SetActive(true);
		Deactivate();
	}


	public void LockCamera()
	{
		if (cameraIsLocked)
		{
			freeLookCam.enabled = true;
			lockedCam.enabled = false;
			cameraIsLocked = false; 
			lockDotUI.gameObject.SetActive(false);
		}
		else
		{
			Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
			float min = float.MaxValue;
			Entity closest = null;
			foreach (Entity e in EntitiesManager.entities)
			{
				if (e.transform == transform) continue;
				Vector2 pos = Camera.main.WorldToScreenPoint(e.GetTargetLock().position);
				float d = Vector2.Distance(pos, center);
				if (d < min)
				{
					min = d;
					closest = e;
				}
			}
			if (closest != null)
			{
				freeLookCam.enabled = false;
				lockedCam.enabled = true;
				cameraIsLocked = true;
				currentLockedEntity = closest.GetTargetLock();
			}
			lockDotUI.gameObject.SetActive(true);
		}
	}

	public void SwitchAttackSet(AttackSet set)
	{
		currentAttackSet = set;
		animator.runtimeAnimatorController = set.animatorController;
		// switch weapon here or before calling
		// (maybe have weapon class with own attackset)
	}

	public void LightAttack()
	{
		if (!IsDashing)
		{
			animator.SetTrigger("LA");
			if (!grounded)
				attackHolder.SetCurrentAttack(currentAttackSet.jumpingLightAttack);
			else
				attackHolder.SetCurrentAttack(currentAttackSet.lightAttack);
			SetAttackState("PreparingAttack");
		}
	}

	public void HeavyAttack()
	{
		if (!IsDashing)
		{
			animator.SetTrigger("HA");
			if (!grounded)
				attackHolder.SetCurrentAttack(currentAttackSet.jumpingHeavyAttack);
			else
				attackHolder.SetCurrentAttack(currentAttackSet.heavyAttack);
			SetAttackState("PreparingAttack");
		}
	}


	

	private void OnCollisionEnter(Collision collision)
	{
		if (IsDashing || IsSuperDashing)
			rb.linearVelocity = Vector3.zero;
		IsSuperDashing = false;
		IsDashing = false;
	}

	public override void SetAttackState(string state)
	{
		attackState = System.Enum.Parse<AttackState>(state, true);
		if (attackState == AttackState.Nothing)
		{
			allowMovement = true;
			allowRotation = true;
			allowJump = true;
			allowDash = true;
		}
		else if (attackState == AttackState.Attacking)
		{
			allowMovement = false;
			allowRotation = false;
			allowJump = false;
			allowDash = false;
		}
		else if (attackState == AttackState.PreparingAttack)
		{
			allowMovement = false;
			allowRotation = true;
			allowJump = false;
			allowDash = false;
		}
		else if (attackState == AttackState.FinishingAttack)
		{
			allowMovement = false;
			allowRotation = false;
			allowJump = false;
			allowDash = true;
		}
	}
}
