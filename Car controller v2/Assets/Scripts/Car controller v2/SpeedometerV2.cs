using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpeedometerType
{
    driveShaft,
    velocity
}

public class SpeedometerV2 : MonoBehaviour
{
    [Header("Refrences")]
    [Tooltip("The driveshaft script")]
    [SerializeField] private DriveShaftV2 driveShaft;
    [Tooltip("The chassis rigidBody")]
    [SerializeField] private Rigidbody carRb;
    
    [Header("Speedometer Data")]
    [Tooltip("The type of speedometer/the way it measures the speed")]
    [SerializeField] private SpeedometerType type;
    
    [HideInInspector] public float speed = 0;
    
    private void Update()
    {
        Speedometer();
    }

    private void Speedometer()
    {
        if (type == SpeedometerType.driveShaft)
        {
            float revsPerHour = driveShaft.rpm * 60;
            float meterPerHour = revsPerHour * driveShaft.radius;
            speed = meterPerHour / 1000;
        }
        else if (type == SpeedometerType.velocity)
        {
            speed = carRb.velocity.magnitude;
        }
    }
}
