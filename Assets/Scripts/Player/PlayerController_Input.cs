using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// input event management of player controller
public partial class PlayerController
{
	public void OnMove(InputAction.CallbackContext context)
	{
		moveInput = context.ReadValue<Vector2>();
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if (context.performed && allowJump)
		{
			jumpRequested = true;
		}
	}

	public void OnDash(InputAction.CallbackContext context)
	{
		if (context.performed && allowDash)
		{
			if (attackState == AttackState.Nothing)
				dashRequested = true;
		}
	}

	public void OnCameraLock(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			LockCamera();
		}
	}
	public void OnLA(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			if (attackState == AttackState.Nothing || attackState == AttackState.FinishingAttack)
				LightAttack();
		}
	}

	public void OnHA(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			if (attackState == AttackState.Nothing || attackState == AttackState.FinishingAttack)
				HeavyAttack();
		}
	}

	public void OnParry(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			if (attackState != AttackState.Attacking)
			{
				StartParry();
			}
		}
	}

	public void OnSwitchWeapon(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			if (attackState == AttackState.Nothing)
			{
				weaponsHolder.SwitchOffhandToActive();
				SwitchAttackSet(weaponsHolder.activeWeapon.attackSet);
			}
		}
	}

	public void OnRespawn(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			Respawn();
		}
	}

	public void OnGlobalRespawn(InputAction.CallbackContext context)
	{
		if (context.performed)
		{
			//GameManager.Singleton.RestartLevel();
		}
	}
}
