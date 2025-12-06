using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Create attack set", fileName = "newAttackSet")]
public class AttackSet : ScriptableObject
{
	public RuntimeAnimatorController animatorController;
	public Attack lightAttack;
	public Attack heavyAttack;
	public Attack jumpingLightAttack;
	public Attack jumpingHeavyAttack;

}
