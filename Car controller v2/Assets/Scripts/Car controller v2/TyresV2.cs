using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyresV2 : MonoBehaviour
{
    #region variables
    
    [Tooltip("Check how thick the tyre is")]
    [HideInInspector] public float radius;
    [Tooltip("Check if the tyre is touching the ground or not")]
    [HideInInspector] public bool isGrounded = false;

    #endregion
    
    #region start

    private void Start()
    {
        radius = GetComponent<Renderer>().bounds.size.y / 2;
    }
    
    #endregion
    
    #region collision checking

    private void OnCollisionEnter(Collision other)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision other)
    {
        isGrounded = false;
    }

    #endregion
}
