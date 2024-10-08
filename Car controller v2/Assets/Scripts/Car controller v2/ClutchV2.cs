using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClutchV2 : MonoBehaviour
{
    [Header("Refrences")]
    [Tooltip("The engine script")]
    [SerializeField] private EngineV2 engine;
    
    [Header("Clutch information")]
    [Tooltip("The clutch progression curve"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve clutchCurve;
    
    [Tooltip("The amount of torque from the engine after the clutch")]
    [HideInInspector] public float outputTorque;

    private float clutchAxis;
    
    private void FixedUpdate()
    {
        Clutch();
    }

    public void Input(InputAction.CallbackContext context)
    {
        clutchAxis = context.ReadValue<float>();
    }
    
    private void Clutch()
    {
        //get the amount of friction between the 2 clutch plates
        float friction = clutchCurve.Evaluate(clutchAxis);
        
        //calculate the torque after the clutch
        outputTorque = engine.outputTorque * friction;
    }
}
