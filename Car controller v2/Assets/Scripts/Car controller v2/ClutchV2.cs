using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClutchV2 : MonoBehaviour
{
    #region variables
    
    [Header("Refrences")]
    [Tooltip("The engine script")]
    [SerializeField] private EngineV2 engine;
    [Tooltip("The gearbox script")] 
    [SerializeField] private GearboxV2 gearbox;
    [Tooltip("The drive shaft script")]
    [SerializeField] private DriveShaftV2 driveShaft;
    
    [Header("Clutch information")]
    [Tooltip("The clutch progression curve"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve clutchCurve;
    
    [Tooltip("The amount of torque from the engine after the clutch")]
    [HideInInspector] public float outputTorque = 0;
    [Tooltip("The amount of contact between the plates")]
    [HideInInspector] public float contact = 0;

    private float clutchAxis = 0;
    
    #endregion
    
    #region update
    private void FixedUpdate()
    {
        Clutch();
    }
    
    #endregion

    #region input

    public void Input(InputAction.CallbackContext context)
    {
        clutchAxis = context.ReadValue<float>();
    }

    #endregion

    #region clutch

    private void Clutch()
    {
        //get the amount of friction between the 2 clutch plates
        contact = clutchCurve.Evaluate(clutchAxis);
        float reversedContact = 1 - contact;
        
        //calculate the torque from the engine after the clutch
        outputTorque = engine.outputTorque * reversedContact;
        
        //calculate the rpm difference between the engine and the driveshaft
        float rpmDifference = engine.rpm - gearbox.rpm;
        float contactDifference = rpmDifference * reversedContact;
        float differenceToAdd = contactDifference / 2;
        
        //add the resistance to the engine
        engine.drivetrainResistance = driveShaft.tyreResistance * reversedContact;

        //add the resistance to the drivetrain
        driveShaft.clutchRpmDifference = differenceToAdd;
    }

    #endregion
}
