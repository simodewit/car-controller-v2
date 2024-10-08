using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBarsV2 : MonoBehaviour
{
    #region variables
    
    [Header("Refrences")]
    [Tooltip("The suspension on the right side of the car")]
    [SerializeField] private SuspensionV2 rightSuspension;
    [Tooltip("The suspension on the left side of the car")]
    [SerializeField] private SuspensionV2 leftSuspension;
    
    [Tooltip("The tyre attached to the right side suspension")]
    [SerializeField] private Transform rightTyre;
    [Tooltip("The tyre attached to the left side suspension")]
    [SerializeField] private Transform leftTyre;
    
    [Tooltip("The rigidbody of the chassis")]
    [SerializeField] private Rigidbody carRb;

    [Header("Details")]
    [Tooltip("The stiffness of the anti roll bar")]
    [SerializeField] private float stiffness = 5000;

    #endregion
    
    #region update

    private void FixedUpdate()
    {
        AntiRollBar();
    }

    #endregion

    #region anti roll bar

    private void AntiRollBar()
    {
        //calculate the travel of the springs
        float rightTravel = rightSuspension.currentTravel;
        float leftTravel = leftSuspension.currentTravel;
        
        //calculate the force that the ARB should give to the suspension
        float antiRollForce = (rightTravel - leftTravel) * stiffness;

        //add the force to the suspension
        carRb.AddForceAtPosition(rightTyre.transform.up * -antiRollForce, rightTyre.position);
        carRb.AddForceAtPosition(leftTyre.transform.up * antiRollForce, leftTyre.position);
    }

    #endregion
}
