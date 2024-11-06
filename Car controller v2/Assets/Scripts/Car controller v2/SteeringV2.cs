using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class SteeringV2 : MonoBehaviour
{
    [Tooltip("Change the linearity of the steering"), Curve(0, 0, 1f, 1f, true)]
    [SerializeField] private AnimationCurve steeringCurve;
    [Tooltip("Here you can assign all the tyres and decide if and how much they should turn")]
    [SerializeField] private TyreInfo[] tyreInfo;
    
    private float steeringAxis;
    
    public void SteeringInput(InputAction.CallbackContext context)
    {
        steeringAxis = context.ReadValue<float>();
    }

    private void Start()
    {
        foreach (var tyre in tyreInfo)
        {
            tyre.GetStartLocation();
        }
    }
    
    private void Update()
    {
        Steering();
    }

    private void Steering()
    {
        //do this for each tyre
        foreach (var tyre in tyreInfo)
        {
            //create a Vector3 with the base offset
            Vector3 clampedRotation = tyre.localStartRotation + new Vector3(0, tyre.rotationOffset.x, tyre.rotationOffset.y);
            
            //check if the tyre has the ability to steer
            if (tyre.shouldSteer)
            {
                //get the steering input through the linearity curve
                float correctedInput = steeringCurve.Evaluate(Mathf.Abs(steeringAxis));
                correctedInput *= Mathf.Sign(steeringAxis);
                
                //correct the base Vector3 with the steering
                clampedRotation.y += correctedInput * tyre.turnAmount;
            }
            
            //apply the rotations
            tyre.tyre.localRotation = Quaternion.Euler(clampedRotation);
        }
    }
}

[System.Serializable]
public class TyreInfo
{
    [Header("Refrences")]
    [Tooltip("The transform of the tyre")]
    public Transform tyre;
    
    [Header("Details")]
    [Tooltip("Here you can decide if the tyre should turn or not")]
    public bool shouldSteer = true;
    [Tooltip("This changes the maximum degrees of steering for this tyre"), Range(0, 120)]
    public float turnAmount = 30;
    [Tooltip("The offset that the tyre has. X = toe, Y = camber")]
    public Vector2 rotationOffset;
    
    [HideInInspector] public Vector3 localStartRotation;
    
    public void GetStartLocation()
    {
        localStartRotation = tyre.localEulerAngles;
    }
}
