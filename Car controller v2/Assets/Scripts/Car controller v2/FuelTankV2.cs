using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum Fuel
{
    diesel,
    gasoline
}

public class FuelTankV2 : MonoBehaviour
{
    [Tooltip("The amount of fuel the fueltank holds in liters"), Range(0, 150)]
    [SerializeField] private float fuel = 50;
    [Tooltip("Decides if there is fuel consumption")]
    [SerializeField] private bool fuelConsumption = true;
    
    [Tooltip("The fuel type and its joules output")]
    public FuelType[] fuelType;
    
    public void ChangeFuel(float amount)
    {
        if (!fuelConsumption)
        {
            return;
        }
        
        fuel -= amount;
    }
}

[Serializable]
public class FuelType
{
    [Tooltip("The fuel type")]
    public Fuel type;
    [Tooltip("The amount of joules output the fuel can provide"), Range(0, 50)]
    public float energyContent;
}
