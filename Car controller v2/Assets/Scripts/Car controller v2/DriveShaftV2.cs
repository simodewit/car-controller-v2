using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DriveShaftV2 : MonoBehaviour
{
    #region variables

    [Header("Refrences")]
    [Tooltip("The gearbox script")] 
    [SerializeField] private GearboxV2 gearbox;
    [Tooltip("the differentials of the car")]
    [SerializeField] private DifferentialV2[] differentials;
    [Tooltip("the clutch script")]
    [SerializeField] private ClutchV2 clutch;
    
    [Header("Settings")]
    [Tooltip("The radius of the drive shaft"), Range(0, 2)]
    public float radius = 0;
    
    [HideInInspector] public float outputTorque = 0;
    [HideInInspector] public float rpm = 0;
    [HideInInspector] public float tyreResistance = 0;
    [HideInInspector] public float clutchRpmDifference = 0;

    #endregion
    
    #region update
    
    private void FixedUpdate()
    {
        DriveShaft();
    }
    
    #endregion
    
    #region drive shaft
    
    private void DriveShaft()
    {
        outputTorque = gearbox.outputTorque;
        
        tyreResistance = CalculateResistance(differentials);
        float rpmToSubtract = -tyreResistance * radius;
        rpm -= rpmToSubtract * clutch.contact + -clutchRpmDifference * radius;
    }
    
    private float CalculateResistance(DifferentialV2[] diffs)
    {
        float resistance = 0;
        
        foreach (var diff in diffs)
        {
            resistance += diff.resistance;
        }
        
        resistance /= differentials.Length;
        
        return resistance;
    }
    
    #endregion
}
