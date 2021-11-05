using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normal : Ability
{
    public Normal(Rigidbody rb) 
    { 
        Debug.Log("Normal Mode"); 
        textName = "Normal";
        _rb = rb;
    }
    
    public override void Movement(float speed)
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        
        if (direction.magnitude >= 0.1f)
        {
            _rb.velocity = new Vector3(horizontal, 0f, vertical) * speed * Time.deltaTime;
        }
    }
}
