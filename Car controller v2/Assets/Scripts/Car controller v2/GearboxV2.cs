using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Gear
{
    reverse,
    neutral,
    first,
    second,
    third,
    fourth,
    fifth,
    sixth,
    seventh,
    eight,
    nine
}

public class GearboxV2 : MonoBehaviour
{
    #region Variables
    
    [Header("Refrences")]
    [Tooltip("The clutch component of the car")]
    [SerializeField] private ClutchV2 clutch;
    
    [Header("Gearbox Settings")]
    [Tooltip("the information about each gear")]
    [SerializeField] private GearInfo[] gears;
    
    [Tooltip("The amount of torque from the engine after the gearbox")]
    [HideInInspector] public float outputTorque;

    private Gear currentGear;

    #endregion
    
    #region update
    
    private void FixedUpdate()
    {
        GearBox();
    }
    
    #endregion
    
    #region input

    private void Input()
    {
        
    }
    
    #endregion
    
    #region gearbox
    
    private void GearBox()
    {
        GearInfo currentGearInfo = new GearInfo();
        
        foreach (var gear in gears)
        {
            if (gear.gear == currentGear)
            {
                currentGearInfo = gear;
            }
        }

        if (currentGear == Gear.reverse)
        {
            outputTorque = clutch.outputTorque * -currentGearInfo.gearRatio;
        }
        else if (currentGear == Gear.neutral)
        {
            outputTorque = 0;
        }
        else
        {
            outputTorque = clutch.outputTorque * currentGearInfo.gearRatio;
        }
    }
    
    #endregion
}

[Serializable]
public class GearInfo
{
    [Tooltip("The gear this info is about")]
    public Gear gear;
    [Tooltip("the ratio of this gear"), Range(0, 5)]
    public float gearRatio;
}