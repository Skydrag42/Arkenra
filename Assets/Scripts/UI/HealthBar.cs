using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class HealthBar : ProgressBar
{
	public Image decreaseGFX;

	public float visualDecreaseWaitDuration = 1f;
	public float visualDecreaseDuration = .5f;
	private Coroutine visualDecreaseRoutine;

	public float gradualIncreaseDuration = .5f;
	private Coroutine gradualIncreaseRoutine;

	public bool showDamage = false;
	public TMP_Text damageText;
	public float damageVisibleDuration = 1.5f;
	private Coroutine damageRoutine;
	private int currentDamageValue = 0;

	public GameObject GFXParent;

	public override int MaxValue
	{
		set
		{
			base.MaxValue = value;
            decreaseGFX.fillAmount = (float)Current / MaxValue;
        }
	}

	protected override void Awake()
	{
		base.Awake();
		if (damageText != null) damageText.text = "";
	}

	protected override int ChangeCurrentAmount(int value)
	{
		if (value < current)
		{
			SetNewDamageValue(current - value);
			if (visualDecreaseRoutine != null)
			{
				//decreaseGFX.fillAmount = (float)current / maxValue;
				StopCoroutine(visualDecreaseRoutine);
			}
			visualDecreaseRoutine = StartCoroutine(VisualDecrease());

			return base.ChangeCurrentAmount(value);
		}
		else
		{
			if (gradualIncreaseRoutine != null)
			{
				StopCoroutine(gradualIncreaseRoutine);
			}
			gradualIncreaseRoutine = StartCoroutine(GradualIncrease());
			return value;
		}
	}

	// TODO:
	// use inside change current amount
	// maybe add +green value for healing
	private void SetNewDamageValue(int value)
	{
		if (!showDamage) return;
		if (damageRoutine != null)
			StopCoroutine(damageRoutine);
		currentDamageValue += value;
		damageRoutine = StartCoroutine(ShowDamage());
	}

	private IEnumerator ShowDamage()
	{
		damageText.text = currentDamageValue.ToString();
		yield return new WaitForSeconds(damageVisibleDuration);
		damageText.text = "";
		currentDamageValue = 0;
	}

	private IEnumerator VisualDecrease()
	{
		yield return new WaitForSeconds(visualDecreaseWaitDuration);
		float startAmount = decreaseGFX.fillAmount;
		float t = 0;
		while (t < visualDecreaseDuration)
		{
			t += Time.deltaTime;
			decreaseGFX.fillAmount = Mathf.Lerp(startAmount, (float)Current / MaxValue, t / visualDecreaseDuration);
			yield return null;
		}
		decreaseGFX.fillAmount = (float)Current / MaxValue;
	}

	private IEnumerator GradualIncrease()
	{
		float startAmount = barGFX.fillAmount;
		float t = 0;
		while (t < gradualIncreaseDuration)
		{
			float value = Mathf.Lerp(startAmount, (float)Current / MaxValue, t / gradualIncreaseDuration);
			decreaseGFX.fillAmount = value;
			barGFX.fillAmount = value;
			t += Time.deltaTime;
			yield return null;
		}
		decreaseGFX.fillAmount = (float)Current / MaxValue;
		barGFX.fillAmount = (float)Current / MaxValue;
	}

	public void HideHealthBar()
	{
		GFXParent.SetActive(false);
	}

	public void ShowHealthBar()
	{
		GFXParent.SetActive(true);
	}
}
