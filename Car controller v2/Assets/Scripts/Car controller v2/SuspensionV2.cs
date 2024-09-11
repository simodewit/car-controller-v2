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
    [SerializeField] private TyresV2 tyre;
    
    [Header("Variables")]
    [Tooltip("The amount of space that the suspension has")]
    [SerializeField] private float travel = 0.5f;
    [Tooltip("this moves the whole travel up and down. usefull for when the upper part of the tyre isnt the same height as the car connection point")]
    [SerializeField] private float connectionOffset = 0.2f;
    [Tooltip("The stiffness of the suspension")]
    [SerializeField] private float stiffness = 150000;
    [Tooltip("The amount that the dampers dampens the spring")]
    [SerializeField] private float damper = 5000;
    [Tooltip("A curve where you can change the behaviour of the spring"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve progressionCurve;

    private float clampedYAxis;
    private Vector3 highestPoint;
    private Vector3 lowestPoint;
    private Rigidbody tyreRb;
    private Vector3 tyreStartPosition;

    private Vector3 test;
    
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
        //calculate the highest point that the suspension can go and the lowest it can reach
        highestPoint = new Vector3(0, carAttachPoint.localPosition.y - tyre.radius + connectionOffset, 0);
        lowestPoint = highestPoint - new Vector3(0, travel, 0);
        
        //clamp the Y axis between those 2 points
        clampedYAxis = Mathf.Clamp(tyre.transform.localPosition.y, lowestPoint.y, highestPoint.y);
        tyre.transform.localPosition = new Vector3(tyreStartPosition.x, clampedYAxis, tyreStartPosition.z);
    }
    
    private void Suspension()
    {
        //calculating how far the spring is in its travel
        float totalTravel = highestPoint.y - lowestPoint.y;
        float currentTravel = highestPoint.y - clampedYAxis;
        float travelFactor = currentTravel / totalTravel;
        
        //calculating the variables needed to calculate the force
        float progressionFactor = progressionCurve.Evaluate(travelFactor);
        Vector3 tyreToCarLocalPosition = carRb.transform.InverseTransformPoint(tyre.transform.position);
        float localVelocity = carRb.GetPointVelocity(tyreToCarLocalPosition).y;
        
        //potential bug in block above. the progressionFactor variable seems unstable and inconsistent
        
        //calculating the amount of force the spring should press
        float force = (stiffness * progressionFactor) - (localVelocity * damper);
        print(force + "         " + tyre.name);
        //check if the tyre hits the ground
        if (tyre.isGrounded)
        {
            //apply forces to the car
            Vector3 forceVector = carRb.transform.up * force;
            //Vector3 forcePosition = transform.TransformPoint(carAttachPoint.position);
            carRb.AddForceAtPosition(forceVector, carAttachPoint.position);
            
            test = forceVector;
        }

        return;
        
        //recalculating the force variable
        float massFactor = tyreRb.mass / carRb.mass;
        float correctedForce = force * massFactor;

        //apply the forces to the tyre
        Vector3 forceVector2 = -tyre.transform.up * correctedForce;
        tyreRb.AddForce(forceVector2);
    }
    
    #endregion

    public void OnDrawGizmos()
    {
        if (tyre.isGrounded)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawLine(tyre.transform.position, (tyre.transform.position + test) / 1000);
    }
}
