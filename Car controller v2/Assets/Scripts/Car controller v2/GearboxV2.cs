using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
    [Tooltip("The drive train script")]
    [SerializeField] private DriveShaftV2 driveShaft;
    
    [Header("Gearbox Settings")]
    [Tooltip("the information about each gear")]
    [SerializeField] private GearInfo[] gears;
    
    [Tooltip("The amount of torque from the engine after the gearbox")]
    [HideInInspector] public float outputTorque = 0;
    [Tooltip("The rpm's of the drive shaft after the gearbox")]
    [HideInInspector] public float rpm = 0;
    [Tooltip("The current gear the car is using")]
    [HideInInspector] public Gear currentGear = Gear.neutral;

    #endregion

    #region start
    
    private void Start()
    {
        currentGear = Gear.neutral;
    }
    
    #endregion

    #region update
    
    private void FixedUpdate()
    {
        GearBox();
        Reverse();
    }
    
    #endregion
    
    #region input

    public void UpShift(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        
        int gearIndex = GetGearIndex(gears, currentGear);

        if (gearIndex >= gears.Length)
        {
            return;
        }
        
        gearIndex++;
        currentGear = gears[gearIndex].gear;
    }
    
    public void DownShift(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        
        int gearIndex = GetGearIndex(gears, currentGear);

        if (gearIndex <= 0)
        {
            return;
        }
        
        gearIndex--;
        currentGear = gears[gearIndex].gear;
    }

    private int GetGearIndex(GearInfo[] gearArray, Gear usedGear)
    {
        int gearIndex = 0;
        
        foreach (var gear in gearArray)
        {
            if (gear.gear == usedGear)
            {
                break;
            }
            
            gearIndex++;
        }
        
        return gearIndex;
    }
    
    #endregion
    
    #region get gear info
    
    private GearInfo GetGearInfo()
    {
        GearInfo currentGearInfo = new GearInfo();
        
        foreach (var gear in gears)
        {
            if (gear.gear == currentGear)
            {
                currentGearInfo = gear;
            }
        }
        
        return currentGearInfo;
    }
    
    #endregion
    
    #region gearbox
    
    private void GearBox()
    {
        GearInfo currentGearInfo = GetGearInfo();

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

    #region reverse

    private void Reverse()
    {
        GearInfo currentGearInfo = GetGearInfo();
        rpm = driveShaft.rpm / currentGearInfo.gearRatio;
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