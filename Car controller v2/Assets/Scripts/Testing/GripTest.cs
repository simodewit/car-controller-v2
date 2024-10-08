using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GripTest : MonoBehaviour
{
    [Header("check torque test")]
    public TyreV2 tyre1;
    public TyreV2 tyre2;
    public TyreV2 tyre3;
    public TyreV2 tyre4;
    public float torque;

    [Header("add speed test")] 
    public Rigidbody carRb;
    public Vector3 speed;
    public bool addSpeed;

    private void Update()
    {
        CheckTorqueTest();
        AddSpeedTest();
    }

    private void CheckTorqueTest()
    {
        tyre1.forwardForce = torque;
        tyre2.forwardForce = torque;
        tyre3.forwardForce = torque;
        tyre4.forwardForce = torque;
    }

    private void AddSpeedTest()
    {
        if (addSpeed)
        {
            carRb.AddRelativeForce(speed);
            addSpeed = false;
        }
    }
}
