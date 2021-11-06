using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hungerin : MonoBehaviour
{
    EssencialProperties m_EssencialProperties;
    [SerializeField] private Rigidbody m_RigidBody;
    [SerializeField] private GameObject m_Grounded;
    
    [Space]
    //private Ability m_CurrentAbility;
    //[SerializeField] private string currentAbilityName;
    private float speed = 300f;
    private float jumpSpeed = 100f;
    private bool isGrounded;

    private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    private void Awake()
    {
        //m_CurrentAbility = new Normal(m_RigidBody);
    }

    // Here we control the player
    private void Update()
    {
        //currentAbilityName = m_CurrentAbility.textName;
    }

    // Here we make physics
    private void FixedUpdate()
    {
        NormalMovement();
    }

    //private void Movement()
    //{
    //    m_CurrentAbility.Movement(speed);
    //}
    private void NormalMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f,angle,0f);
        
        //isGrounded = RaycastHit();

        if (isGrounded)
        {
            if (Input.GetButton("Jump"))
            {
                m_RigidBody.AddForce(new Vector3(0, jumpSpeed, 0));
            }
        }

        m_RigidBody.velocity = new Vector3(horizontal, 0f, vertical) * speed * Time.deltaTime;
    }
}
