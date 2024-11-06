using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineV2 : MonoBehaviour
{
    #region variables

    [Header("Refrences")] 
    [Tooltip("The throttle body script")]
    [SerializeField] private ThrottleBodyV2 throttleBody;
    [Tooltip("The fuel tank script")]
    [SerializeField] private FuelTankV2 fuelTank;

    [Header("Details")] 
    [Tooltip("The amount in liter of air to add 1 liter fuel to"), Range(0, 30)]
    [SerializeField] private float fuelAirRatio;
    [Tooltip("The sort of fuel that this engine uses")]
    [SerializeField] private Fuel typeOfFuel;
    [Tooltip("How efficient the engine is to convert joules of the fuel to nm"), Range(0, 100)]
    [SerializeField] private float efficiency;
    [Tooltip("The radius of the crankshaft"), Range(0, 2)]
    [SerializeField] private float crankShaftRadius;
    [Tooltip("The resistance that the engine produces over its rpm range"), Range(0, 50)]
    [SerializeField] private float resistance;
    [Tooltip("The maximum amount of rpm that the engine can handle"), Range(0, 25000)]
    [SerializeField] private int maxRpm;
    [Tooltip("How much 1 kubic metre of air weights in grams"), Range(0, 1)]
    [SerializeField] private float airWeight;
    [Tooltip("How much 1 litre of the used fuel weights in kg"), Range(0, 3)]
    [SerializeField] private float fuelWeight;
    
    [Tooltip("The rpm's of the engine")]
    [HideInInspector] public float rpm = 0;
    [Tooltip("The amount of torque from the engine")]
    [HideInInspector] public float outputTorque = 0;
    [Tooltip("The amount resistance that the whole drivetrain produces")]
    [HideInInspector] public float drivetrainResistance = 0;


    private float fuelToCut = 1;
    private FuelType fuelSort;
    
    #endregion
    
    #region start

    private void Start()
    {
        foreach (var fuelType in fuelTank.fuelType)
        {
            if (fuelType.type == typeOfFuel)
            {
                fuelSort = fuelType;
            }
        }
    }
    
    #endregion
    
    #region update

    private void FixedUpdate()
    {
        Engine();
        Rpm();
    }
    
    #endregion
    
    #region fuel consumption

    private float FuelConsumtion()
    {
        float airInGrams = throttleBody.air * airWeight;
        float airInKg = ConvertGrams(airInGrams);
        float fuelInKg = airInKg / fuelAirRatio;
        
        float totalFuel = fuelInKg / fuelWeight;
        fuelTank.ChangeFuel(totalFuel);
        
        return totalFuel;
    }

    #endregion
    
    #region gram kg converter

    private float ConvertGrams(float grams)
    {
        float kg = grams / 1000;
        return kg;
    }
    
    #endregion
    
    #region fuel to nm

    private float FuelToNm(float fuel)
    {
        //calculate the amount of joules from the fuel
        float nmPerL = fuelSort.energyContent * (efficiency / 100);

        //calculate the amount of joules from the used fuel
        float totalNm = fuel * nmPerL;
        
        return totalNm;
    }
    
    #endregion
    
    #region engine

    private void Engine()
    {
        float fuelConsumption = FuelConsumtion();
        float nmTorque = FuelToNm(fuelConsumption);

        float engineResistance = Mathf.Sqrt(resistance * rpm);
        
        float totalResistance = engineResistance + drivetrainResistance * crankShaftRadius;
        
        outputTorque = nmTorque - totalResistance;
    }

    private void Rpm()
    {
        float rpmToAdd = outputTorque * crankShaftRadius;
        rpm += rpmToAdd;
    }
    
    #endregion
}

