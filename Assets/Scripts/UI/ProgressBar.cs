using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
	public Image barGFX;

	public int minValue = 0;
	
	[SerializeField]
	protected int maxValue = 500;
	public virtual int MaxValue
	{
		get { return maxValue; }
		set 
		{ 
			maxValue = value;
			if (!isFixedSize)
				((RectTransform)transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxValue * sizeValueRatio);
            barGFX.fillAmount = (float)(current - minValue) / (maxValue - minValue);
        }
	}

	[SerializeField]
	protected int current = 0;
	public int Current 
	{ 
		get { return current; }
		set
		{
			current = ChangeCurrentAmount(value);
		}
	}

	public bool isFixedSize = false;

	[Tooltip("x size of the bar / maxValue")]
	public float sizeValueRatio = 1;


	protected virtual int ChangeCurrentAmount(int value)
	{
		if (value >= minValue && value <= maxValue)
		{
			barGFX.fillAmount = (float)(value - minValue) / (maxValue - minValue);
		}
		return value;
	}

	protected virtual void Awake()
	{
		if (!isFixedSize)
			((RectTransform)transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxValue * sizeValueRatio);
	}
}