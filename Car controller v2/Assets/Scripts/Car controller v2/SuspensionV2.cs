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
    
    #region Clamping

    private void ClampTravel()
    {
        //calculate the highest point that the suspension can go and the lowest it can reach
        highestPoint = new Vector3(tyreStartPosition.x, tyreStartPosition.y + travelOffset, tyreStartPosition.z);
        lowestPoint = highestPoint - new Vector3(0, travel, 0);
        
        //get the local axis before clamping it (needed for calculating the lost force)
        float yAxisBeforeClamp = tyre.transform.localPosition.y;
        
        //clamp the Y axis between those 2 points
        clampedYAxis = Mathf.Clamp(tyre.transform.localPosition.y, lowestPoint.y, highestPoint.y);
        tyre.transform.localPosition = new Vector3(tyreStartPosition.x, clampedYAxis, tyreStartPosition.z);

        return;
        
        //calculate how much force would need to be directed to the car
        if (yAxisBeforeClamp > highestPoint.y)
        {
            ApplyCorrectedForces(yAxisBeforeClamp, highestPoint.y);
        }
        else if (yAxisBeforeClamp < lowestPoint.y)
        {
            ApplyCorrectedForces(yAxisBeforeClamp, lowestPoint.y);
        }
    }

    private void ApplyCorrectedForces(float yAxisBeforeClamp, float pointToSubtract)
    {
        //set the tyre velocity to 0
        tyreRb.velocity = Vector3.zero;
        
        //calculate total distance to correct
        float overshotDistance = yAxisBeforeClamp - pointToSubtract;
        float lostForce = stiffness * overshotDistance;
        
        //apply the forces to the car
        Vector3 forceVector = new Vector3(0, lostForce, 0);
        carRb.AddForceAtPosition(forceVector, tyre.transform.localPosition);
    }
    
    #endregion
    
    #region Suspension
    
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
        float tyreVelocity = tyreRb.velocity.y;

        //calculating the amount of force the spring should press
        float force = currentTravel * stiffness * progressionFactor - tyreVelocity * -damper; //the minus at the damper variable is there for a bug somewhere else in the script

        //check how much force would be pushed to both sides
        if (tyre.isGrounded /* && force >= 0 */)
        {
            ApplyForces(-force, true);
        }
        else
        {
            ApplyForces(-force / 2, false);
        }
    }

    private void ApplyForces(float force, bool a)
    {
        //************************THE A BOOL SHOULDNT BE NEEDED AND IN ALL CASES IT SHOULD APPLY TO BOTH TYRE AND CAR. ITS THERE FOR A BUG*****************************
        if (a)
        {
            //apply forces to the car
            Vector3 carForce = carRb.transform.up * force;
            carRb.AddForceAtPosition(carForce, tyre.transform.localPosition);
            
            gizmosDebugVector3 = carForce;
        }
        else
        {
            //apply the forces to the tyre
            Vector3 tyreForce = -tyreRb.transform.up * force;
            tyreRb.AddForce(tyreForce);
        }
    }
    
    #endregion

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
    }
}
