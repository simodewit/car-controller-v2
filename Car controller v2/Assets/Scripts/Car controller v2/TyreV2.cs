using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TyreV2 : MonoBehaviour
{
    #region variables
    
    [Tooltip("Check how thick the tyre is")]
    [HideInInspector] public float radius;
    [Tooltip("Check if the tyre is touching the ground or not")]
    [HideInInspector] public bool isGrounded = false;

    private List<GameObject> collisions = new List<GameObject>();
    
    #endregion
    
    #region start

    private void Start()
    {
        radius = GetComponent<Renderer>().bounds.size.y / 2;
    }

    private void Update()
    {
        CheckCollision();
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
}
