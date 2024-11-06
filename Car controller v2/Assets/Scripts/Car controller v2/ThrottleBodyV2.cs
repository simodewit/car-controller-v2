using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrottleBodyV2 : MonoBehaviour
{
    #region variables

    [Header("Refrences")]
    [Tooltip("The rigidbody of the car")]
    [SerializeField] private Rigidbody carRb;
    [Tooltip("The engine script")]
    [SerializeField] private EngineV2 engine;
    
    [Header("Throttle")]
    [Tooltip("The deadzone in the pedal before the throttle is used"), Range(0, 100)]
    [SerializeField] private float throttleDeadzone = 0;
    [Tooltip("Minimum amount of air getting thru to keep the car running in stationary conditions in liters"), Range(0, 20)]
    [SerializeField] private float minimumAir;
    [Tooltip("The amount of air every cylinder of the engine can take up in liters (total capacity / cylinder amount)"), Range(0, 2)]
    [SerializeField] private float cylinderVolume;
    [Tooltip("The amount of cylinders in the engine"), Range(0, 26)]
    [SerializeField] private int cylinderAmount;

    //get variables
    [Tooltip("The amount of air intake in liters")]
    [HideInInspector] public float air = 0;

    //private variables
    private float throttleAxis = 0;

    #endregion

    #region update and input

    public void FixedUpdate()
    {
        Throttle();
    }

    public void ThrottleInput(InputAction.CallbackContext context)
    {
        throttleAxis = context.ReadValue<float>();
    }

    #endregion

    #region throttle

    private void Throttle()
    {
        float axis = throttleAxis;

        if (axis < throttleDeadzone)
        {
            axis = 0;
        }

        float frameRpm = engine.rpm / 60 / 50;
        air = frameRpm / 4 * (cylinderVolume * cylinderAmount);

        if(air < minimumAir)
        {
            air = minimumAir;
        }
    }

    #endregion
}
