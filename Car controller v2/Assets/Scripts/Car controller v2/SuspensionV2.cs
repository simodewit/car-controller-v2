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
    [SerializeField] private TyresV2 tyre;
    
    [Header("Variables")]
    [Tooltip("The amount of space that the suspension has"), Range(0, 1)]
    [SerializeField] private float travel = 0.2f;
    [Tooltip("this moves the whole travel up and down. standard offset is highest point on tyre start location"), Range(-1, 1)]
    [SerializeField] private float travelOffset = 0.1f;
    [Tooltip("The stiffness of the suspension"), Range(0, 200000)]
    [SerializeField] private float stiffness = 150000;
    [Tooltip("The amount that the dampers dampens the spring"), Range(0, 10000)]
    [SerializeField] private float damper = 5000;
    [Tooltip("A curve where you can change the behaviour of the spring"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve progressionCurve;

    private float clampedYAxis;
    private Rigidbody tyreRb;
    private Vector3 highestPoint;
    private Vector3 lowestPoint;
    private Vector3 tyreStartPosition;
    private Vector3 contactPoint;

    private Vector3 gizmosDebugVector3;
    private Vector3 gizmosDebugVector3_2;
    
    #endregion
    
    #region start

    private void Start()
    {
        tyreRb = tyre.GetComponent<Rigidbody>();
        tyreStartPosition = tyre.transform.localPosition;
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
    
    #region Suspension

    private void ClampTravel()
    {
        //THIS WHOLE FUNCTION SHOULD BE REPLACED BY AN ADDFORCE PUSHING THE CAR UPWARDS AND THE TYRE DOWNWARDS IN THE SUSPENSION FUNCTION
        //THIS WAY THERE ISN'T ANY FORCE APPLIED TO THE CAR WHEN THE TRAVEL GETS LOCKED
        
        //THIS SOLUTION ABOVE WOULD PROBABLY WORK BEST WHEN YOU STILL USE THE CLAMPS BUT IN A DYNAMIC WAY WHERE YOU DO APPLY FORCES
        //TO THE REST OF THE SYSTEM
        
        
        //calculate the highest point that the suspension can go and the lowest it can reach
        highestPoint = new Vector3(tyreStartPosition.x, tyreStartPosition.y + travelOffset, tyreStartPosition.z);
        lowestPoint = highestPoint - new Vector3(0, travel, 0);
        
        //clamp the Y axis between those 2 points
        clampedYAxis = Mathf.Clamp(tyre.transform.localPosition.y, lowestPoint.y, highestPoint.y);
        tyre.transform.localPosition = new Vector3(tyreStartPosition.x, clampedYAxis, tyreStartPosition.z);
    }
    
    private void Suspension()
    {
        //calculating the total travel the spring has
        float totalTravel = highestPoint.y - lowestPoint.y;
        
        //calculating how compressed the spring is
        float targetPos = travel / 2 + lowestPoint.y;
        float currentTravel = targetPos - tyre.transform.localPosition.y;
        float travelFactor = Mathf.Abs(currentTravel) / (totalTravel / 2);
        
        //calculating the variables needed to calculate the force
        float progressionFactor = progressionCurve.Evaluate(travelFactor);
        float tyreVelocity = carRb.GetPointVelocity(tyre.transform.localPosition).y;
        
        //THIS WORKS WELL BUT THE VELOCITY SHOULD ALSO BE TAKEN FROM THE TYRE VELOCITY TO GET THE TOTAL TYRE VELOCITY
        //IF YOU WOULD IMPLEMENT THIS RIGHT NOW IT WOULD CREATE A BUG WHERE THE VELOCITY INCREASES OF THE TYRE WHEN IN THE AIR
        //BECAUSE THE CLAMP HOLDS IT BUT DOESNT CHANGE THE VELOCITY
        
        //calculating the amount of force the spring should press
        float force = currentTravel * stiffness * progressionFactor - tyreVelocity * damper;
        
        //THE FORCES WHEN THE SUSPENSION IS LOWERING SHOULD GO TO THE WHEELS AND THE FORCES GOING UPWARDS SHOULD GO TO THE CAR
        //RIGHT NOW THE CAR RECIEVES BOTH FORCES CREATING A UNREALISTIC SIMULATION
        
        //check if the tyre hits the ground
        if (tyre.isGrounded)
        {
            //apply forces to the car
            Vector3 forceVector = carRb.transform.up * force;
            carRb.AddForceAtPosition(forceVector, tyre.transform.localPosition);

            gizmosDebugVector3 = forceVector;
        }
    }
    
    #endregion

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(highestPoint + carRb.transform.position, new Vector3(0.05f, 0.05f, 0.05f));
        Gizmos.DrawCube(lowestPoint + carRb.transform.position, new Vector3(0.05f, 0.05f, 0.05f));
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(tyre.transform.position, new Vector3(0.05f, 0.05f, 0.05f));
        Gizmos.color = Color.white;
        Gizmos.DrawLine(highestPoint + carRb.transform.position, lowestPoint + carRb.transform.position);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(tyre.transform.position, tyre.transform.position + gizmosDebugVector3.normalized);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(tyre.transform.position, tyre.transform.position + gizmosDebugVector3_2.normalized);
    }
}
