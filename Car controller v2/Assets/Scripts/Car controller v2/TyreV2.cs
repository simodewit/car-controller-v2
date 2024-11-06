using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(MeshCollider))]
public class TyreV2 : MonoBehaviour
{
    #region variables

    [Header("Refrences")] 
    [Tooltip("The rigidbody of the chassis")]
    [SerializeField] private Rigidbody carRb;
    [Tooltip("The suspension this tyre is attached to")]
    [SerializeField] private SuspensionV2 suspension;
    [Tooltip("The clutch script")]
    [SerializeField] private ClutchV2 clutch;
    
    [Header("Longitudinal grip")]
    [Tooltip("The amount at the end of the Grip Curve")]
    [SerializeField] private float longMaximumGrip = 500;
    [Tooltip("The grip curve of the tyre that decides how much grip you have at certain slip angles"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve longGripCurve;
    [Tooltip("Here you can give the tyre extra grip outside of the simulated grip (can be buggy)"), Range(0, 5)]
    [SerializeField] private float longGripBending = 1;
    
    [Header("Lateral grip")]
    [Tooltip("The grip curve of the tyre that decides how much grip you have at certain slip angles"), Curve(0f, 0f, 1f, 1f, true)]
    [SerializeField] private AnimationCurve latGripCurve;
    [Tooltip("Here you can give the tyre extra grip outside of the simulated grip (can be buggy)"), Range(0, 5)]
    [SerializeField] private float latGripBending = 1;
    
    [Header("Resistance")]
    [SerializeField] private float rollingCoefficient = 0.02f;
    
    [Tooltip("Check how thick the tyre is")]
    [HideInInspector] public float radius = 0;
    [Tooltip("Check if the tyre is touching the ground or not")]
    [HideInInspector] public bool isGrounded = false;
    [Tooltip("add torque in newtons to this variable to accelerate with this tyre")]
    [HideInInspector] public float forwardForce = 0;
    [Tooltip("The amount of rotations per minute this tyre rotates")]
    [HideInInspector] public float rpm = 0;
    [Tooltip("The amount of force the tyre gets from the engine")]
    [HideInInspector] public float accelerationForce = 0;
    [Tooltip("The amount of resistance that this tyre produces in nm")]
    [HideInInspector] public float resistance = 0;

    private List<GameObject> collisions = new List<GameObject>();
    private float lostLongTorque = 0;
    private float lostLatTorque = 0;
    private float latMaximumGrip = 0;
    
    #endregion
    
    #region start

    private void Start()
    {
        radius = GetComponent<Renderer>().bounds.size.y / 2;
    }
    
    #endregion
    
    #region update

    private void Update()
    {
        CheckCollision();
    }
    
    private void FixedUpdate()
    {
        RPM();
        Resistance();
        Grip();
    }
    
    #endregion
    
    #region rpm

    private void RPM()
    {
        //calculate how much of the velocity is in the tyre's forward direction
        float dotProduct = Vector3.Dot(transform.forward, carRb.velocity.normalized);
        dotProduct = Mathf.Clamp(dotProduct, -1, 1);

        //calculate the velocity
        float velocity = carRb.velocity.magnitude * dotProduct;

        //calculate the circumfrence
        float circumfrence = 2 * Mathf.PI * radius;

        //calculate rpm
        float currentRPM = velocity / circumfrence * 60;

        //add the rpm's
        rpm = currentRPM;
    }

    #endregion

    #region resistance

    private void Resistance()
    {
        float resistanceForce = rollingCoefficient * suspension.downwardForce;
        resistance = resistanceForce * radius;
    }

    #endregion
    
    #region collision checking

    private void OnCollisionEnter(Collision other)
    {
        collisions.Add(other.gameObject);
    }

    private void OnCollisionExit(Collision other)
    {
        collisions.Remove(other.gameObject);
    }

    private void CheckCollision()
    {
        if (collisions.Count != 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    #endregion

    #region grip

    private void Grip()
    {
        //get the grip values of both axis
        float longGrip = LongitudinalGrip();
        float latGrip = LateralGrip();
        
        //calculate the percentages of lost grip
        float longPercentage = lostLongTorque / longMaximumGrip;
        float latPercentage = lostLatTorque / latMaximumGrip;
        float limitGrip = 1 - (longPercentage + latPercentage) / 2;
        
        //calculate the needed resistance
        float totalResistance = resistance * clutch.contact;
        
        if (longGrip >= 0)
        {
            longGrip -= totalResistance;
        }
        else
        {
            longGrip += totalResistance;
        }
        
        //add them together into one vector3 and calculate the contact patch location
        Vector3 forceVector = (longGrip * carRb.transform.forward + latGrip * -transform.right) * limitGrip;
        Vector3 contactPatch = transform.position + new Vector3(0, -radius, 0);
        
        //add the forces to the car
        carRb.AddForceAtPosition(forceVector, contactPatch);
    }
    
    private float LongitudinalGrip()
    {
        //check if the car is on the ground
        if (!isGrounded)
        {
            return 0f;
        }
        
        //get the factor of the amount of torque that the tyre allows
        float torquePercentage = Mathf.Abs(forwardForce) / longMaximumGrip;
        float graphFactor = longGripCurve.Evaluate(torquePercentage);
        
        //calculate the force to apply to the car and the unused forces
        float totalForce = forwardForce * graphFactor * longGripBending;
        lostLongTorque = totalForce - forwardForce * longGripBending;
        
        return totalForce;
    }
    
    private float LateralGrip()
    {
        //check if the car is on the ground
        if (!isGrounded)
        {
            return 0f;
        }
        
        //calculate the velocity in the tyres direction
        float dotProduct = Vector3.Dot(transform.right, carRb.GetPointVelocity(transform.position).normalized);
        dotProduct = Mathf.Clamp(dotProduct, -1, 1);
        
        //calculate the force that the car pushes against the tyres direction and get the factor from the curve
        float idealForce = carRb.mass * (carRb.velocity.magnitude * dotProduct);
        float availableGrip = latGripCurve.Evaluate(Mathf.Abs(dotProduct));
        
        //add al the modifiers for the grip and put the force to the right direction
        float totalForce = idealForce * latGripBending * availableGrip;
        
        //calculate the max and lost force that the tyre can handle
        latMaximumGrip = idealForce;
        lostLatTorque = totalForce - idealForce * latGripBending;
        
        return totalForce;
    }

    #endregion
}
