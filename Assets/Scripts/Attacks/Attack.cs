using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/Create new attack", fileName = "newAttack")]
public class Attack : ScriptableObject
{
	public string attackName;
	public int baseDamage = 10;
	public List<int> followUpBaseDamage = new List<int>();
	public bool isMultiHit = false;
}
