using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SuspensionV2 : MonoBehaviour
{
    #region variables
    
    [Header("Refrences")]
    [Tooltip("The rigidbody attached to the chassis")]
    [SerializeField] private Rigidbody carRb;
    [Tooltip("The script of the wheel that the suspension is attached to")]
    [SerializeField] private TyreV2 tyre;
    
    [Header("Variables")]
    [Tooltip("The amount of space that the suspension has"), Range(0, 1)]
    [SerializeField] private float travel = 0.2f;
    [Tooltip("this moves the whole travel up and down. standard offset is highest point on tyre start location"), Range(-1, 1)]
    [SerializeField] private float travelOffset = 0.1f;
    [Tooltip("The place where the targetPosition is located in the suspension 0 = at the top, 1 = at the bottom, 0.5 = in the middle(recommended)"), Range(0, 1)]
    [SerializeField] private float targetPosition = 0.5f;
    [Tooltip("The stiffness of the suspension"), Range(0, 300000)]
    [SerializeField] private float stiffness = 50000;
    [Tooltip("The amount that the dampers dampens the spring"), Range(0, 300000)]
    [SerializeField] private float damper = 25000;
    [Tooltip("A curve where you can change the behaviour of the spring"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve progressionCurve;

    [Tooltip("The place in the travel where the suspension is")]
    [HideInInspector] public float currentTravel = 0;
    [Tooltip("The downward force from the suspension onto the tyre")]
    [HideInInspector] public float downwardForce = 0;
    
    private float clampedYAxis = 0;
    private float lastTimestepTravel = 0;
    private float travelFactor = 0;
    private Rigidbody tyreRb;
    private Vector3 highestPoint = Vector3.zero;
    private Vector3 lowestPoint = Vector3.zero;
    private Vector3 tyreStartPosition = Vector3.zero;
    private Vector3 contactPoint = Vector3.zero;

    private Vector3 gizmosDebugVector3 = Vector3.zero;
    private Vector3 gizmosDebugVector3_2 = Vector3.zero;
    
    #endregion
    
    #region start

    private void Start()
    {
        tyreRb = tyre.GetComponent<Rigidbody>();
        tyreStartPosition = tyre.transform.localPosition;
        ClampTravel();
    }
    
    #endregion
    
    #region update

    private void Update()
    {
        ClampTravel();
    }

    private void FixedUpdate()
    {
        Suspension();
    }
    
    #endregion
    
    #region Clamping

    private void ClampTravel()
    {
        //calculate the highest point that the suspension can go and the lowest it can reach
        highestPoint = new Vector3(tyreStartPosition.x, tyreStartPosition.y + travelOffset, tyreStartPosition.z);
        lowestPoint = highestPoint - new Vector3(0, travel, 0);
        
        //clamp the Y axis between those 2 points
        clampedYAxis = Mathf.Clamp(tyre.transform.localPosition.y, lowestPoint.y, highestPoint.y);
        tyre.transform.localPosition = new Vector3(tyreStartPosition.x, clampedYAxis, tyreStartPosition.z);
    }
    
    #endregion
    
    #region Suspension
    
    private void Suspension()
    {
        //calculating the total travel the spring has
        float totalTravel = highestPoint.y - lowestPoint.y;
        
        //calculating how compressed the spring is
        float targetPos = travel * targetPosition + lowestPoint.y;
        currentTravel = targetPos - tyre.transform.localPosition.y;
        travelFactor = Mathf.Abs(currentTravel) / (totalTravel / 2);
        
        //checking the progression of the spring
        float progressionFactor = progressionCurve.Evaluate(travelFactor);
        
        //calculating the velocity of the spring
        float springVelocity = (currentTravel - lastTimestepTravel) / Time.fixedDeltaTime;
        lastTimestepTravel = currentTravel;
        
        //calculating the amount of force the spring should press
        float force = -Mathf.Sign(currentTravel) * stiffness * progressionFactor - springVelocity * damper;
        
        //check how much force would be pushed to both sides
        if (tyre.isGrounded && force >= 0)
        {
            ApplyForces(force);
        }
        else
        {
            ApplyForces(force / 2);
        }
    }

    private void ApplyForces(float force) 
    {
        //apply forces to the car
        Vector3 carForce = carRb.transform.up * force;
        carRb.AddForceAtPosition(carForce, tyre.transform.position);
        
        gizmosDebugVector3 = carForce;
        
        //apply the forces to the tyre
        Vector3 tyreForce = -tyreRb.transform.up * force;
        tyreRb.AddForce(tyreForce);

        downwardForce = tyreForce.y;
        
        gizmosDebugVector3_2 = tyreForce;
    }
    
    #endregion

    #region gizmos

    /*

    public void OnDrawGizmos()
    {


        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(highestPoint + carRb.transform.position, new Vector3(0.05f, 0.05f, 0.05f));
        Gizmos.DrawCube(lowestPoint + carRb.transform.position, new Vector3(0.05f, 0.05f, 0.05f));
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(new Vector3(highestPoint.x, tyre.transform.localPosition.y, highestPoint.z) + carRb.transform.position, new Vector3(0.05f, 0.05f, 0.05f));
        Gizmos.color = Color.white;
        Gizmos.DrawLine(highestPoint + carRb.transform.position, lowestPoint + carRb.transform.position);
        Gizmos.color = Color.green;
        Vector3 midTravel = highestPoint + carRb.transform.position - new Vector3(0, travel / 2, 0);
        Gizmos.DrawLine(midTravel, midTravel + gizmosDebugVector3.normalized);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(midTravel, midTravel + gizmosDebugVector3_2.normalized);


    }

    */

    #endregion
}
