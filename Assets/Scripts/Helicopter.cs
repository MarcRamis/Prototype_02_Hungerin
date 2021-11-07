using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : Ability
{
    public Helicopter(Rigidbody rb) {
        Debug.Log("Helicopter Mode"); 
        textName = "Helicopter";
        _rb = rb;
    }
    
    public override void Movement(float speed)
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        float topFly = Input.GetAxis("Fire1");
        
        Vector3 direction = new Vector3(horizontal, topFly, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            _rb.velocity = new Vector3(horizontal, topFly, vertical) * speed * Time.deltaTime;
        }
    }
}
