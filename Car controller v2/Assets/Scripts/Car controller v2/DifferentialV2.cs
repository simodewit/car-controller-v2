using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DifferentialV2 : MonoBehaviour
{
    #region variables
    
    [Header("Refrences")]
    [Tooltip("The left wheel")]
    [SerializeField] private TyreV2 leftWheel;
    [Tooltip("The right wheel")]
    [SerializeField] private TyreV2 rightWheel;
    [Tooltip("The gearbox")]
    [SerializeField] private DriveShaftV2 driveShaft;
    
    [Tooltip("The ratio of the final gear in the differential"), Range(0, 5)]
    [SerializeField] private float finalGearRatio = 3;
    [Tooltip("How much of the differential can be opened or closed"), Range(0, 100)]
    [SerializeField] private float diffLock = 50;

    [Tooltip("The resistance of the tyres after the differential final gear")]
    [HideInInspector] public float resistance = 0;
    
    private List<DifferentialV2> allDiffs = new List<DifferentialV2>();
    
    #endregion
    
    #region start

    private void Start()
    {
        allDiffs.Add(FindObjectOfType<DifferentialV2>());
    }
    
    #endregion
    
    #region update

    private void FixedUpdate()
    {
        Reverse();
        Diff();
    }
    
    #endregion
    
    #region differential
    
    private void Diff()
    {
        //calculate the torque after the final gear
        float entryTorque = driveShaft.outputTorque * finalGearRatio;
        
        //calculate the percentages
        float diffRpm = driveShaft.rpm * finalGearRatio;
        float leftFactor = leftWheel.rpm / diffRpm;
        float rightFactor = rightWheel.rpm / diffRpm;

        //calculate the min and max for the clamp
        float min = (100 - diffLock / 2) / 100;
        float max = (100 + diffLock / 2) / 100;

        //clamp the values
        leftFactor = Mathf.Clamp(leftFactor, min, max);
        rightFactor = Mathf.Clamp(rightFactor, min, max);
        
        //apply the forces to the tyre
        leftWheel.accelerationForce = entryTorque / 2 * leftFactor / allDiffs.Count;
        rightWheel.accelerationForce = entryTorque / 2 * rightFactor / allDiffs.Count;
    }
    
    #endregion

    #region reverse

    private void Reverse()
    {
        resistance = (leftWheel.resistance + rightWheel.resistance) / finalGearRatio;
    }

    #endregion
}
