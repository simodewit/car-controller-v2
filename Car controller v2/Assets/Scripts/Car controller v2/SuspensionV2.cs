using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionV2 : MonoBehaviour
{
    #region variables
    
    [Header("Refrences")]
    [Tooltip("The place where the suspension is attached to the car")]
    [SerializeField] private Transform carAttachPoint;
    [Tooltip("The place where the suspension is attached to the wheel")]
    [SerializeField] private Transform wheelAttachPoint;
    
    [Tooltip("The rigidbody attached to the chassis")]
    [SerializeField] private Rigidbody carRb;
    [Tooltip("The script of the wheel that the suspension is attached to")]
    [SerializeField] private Tyre tyre;
    
    [Header("Variables")]
    [Tooltip("The added ride height to the ")]
    [SerializeField] private float rideHeight = 0.5f;
    [Tooltip("The amount of space that the suspension has")]
    [SerializeField] private float travel = 0.5f;
    [Tooltip("The stiffness of the suspension")]
    [SerializeField] private float stiffness = 150000;
    [Tooltip("The amount that the dampers damps the spring")]
    [SerializeField] private float damper = 5000;
    [Tooltip("A curve where you can change the behaviour of the spring"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve springCurve;
    
    #endregion
    
    #region update

    private void FixedUpdate()
    {
        ClampTravel();
        Suspension();
    }
    
    #endregion
    
    #region Suspension

    private void ClampTravel()
    {
        //take the car attach point y axis and thats your start
        //then that point + travel is point under
    }
    
    private void Suspension()
    {
        
    }
    
    #endregion
}

/*
float positionInSpring = targetPos.localPosition.y - tyre.transform.localPosition.y;

//calculate the value the force needs according to the spring progression graph
float graphValue = Mathf.Abs(positionInSpring) / (travel / 2);
float progressionFactor = springCurve.Evaluate(graphValue);

//calculate the force that should be applied to the car
Vector3 wheelPosition = carRb.transform.InverseTransformPoint(tyre.transform.position);
float wheelVelocity = carRb.GetPointVelocity(wheelPosition).y;
float force = (positionInSpring * stiffness * progressionFactor) - (wheelVelocity * -damper);

//calculate the place where the force towards the car should be added
Vector3 offset = new Vector3(0, travel / 2, 0);
Vector3 forcePoint = carRb.transform.TransformPoint(targetPos.localPosition + offset);

//calculate force to hold the car upwards
Vector3 forceInVector = -carRb.transform.up * force;

//calculate force to keep the wheel to the target position
float weightFactor = tyreRb.mass / carRb.mass;
Vector3 wheelForce = tyre.transform.up * force * weightFactor;
*/