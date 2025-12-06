using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Create new attack", fileName = "newAttack")]
public class Attack : ScriptableObject
{
	public string attackName;
	public Damage baseDamage;
	public List<Damage> followUpBaseDamage = new List<Damage>();
	public bool isMultiHit = false;
}
