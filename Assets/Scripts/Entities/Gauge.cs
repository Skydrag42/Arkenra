using System;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class Gauge
{
    [SerializeField] private uint currentValue;

    public uint CurrentValue
    {
        get { return currentValue; }
        set 
        { 
            currentValue = value;
            valueChanged?.Invoke(value);
        }
    }
    public Action<uint> valueChanged;


    [SerializeField] private uint maxValue;
    public uint MaxValue
    {
        get { return maxValue; }
        set 
        { 
            maxValue = value; 
            maxValueChanged?.Invoke(value);
        }
    }
    public Action<uint> maxValueChanged;

    public Gauge(uint currentValue, uint maxValue)
    {
        MaxValue = maxValue;
        CurrentValue = currentValue;
    }

    // Ajoute une valeur � la jauge
    public void Add(int amount) // amount > 0
    {
        CurrentValue += (uint)amount;
        if (CurrentValue > MaxValue)
            CurrentValue = MaxValue;
    }

    // Retire une valeur � la jauge
    public uint Substract(int amount)
    {
        if (amount > 0) // Damage
        {
            if (CurrentValue < amount)
            {
                CurrentValue = 0;
                return (uint) amount - CurrentValue;
            }
            else
            {
                CurrentValue -= (uint)amount;
                return 0;
            }
        }
        else // Healing
        {
            if (maxValue < (CurrentValue-amount))
            {
                CurrentValue = MaxValue;
            }
            else
            {
                uint temp = (uint)(-amount);
                CurrentValue += temp;
            }
            return 0;

        }

    }

    public bool IsEmpty()
    {
        if (CurrentValue == 0)
            return true;
        return false;
    }

    public bool IsFull()
    {
        if (CurrentValue == MaxValue)
            return true;
        return false;
    }
}
