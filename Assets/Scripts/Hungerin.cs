using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hungerin : MonoBehaviour
{
    EssencialProperties m_EssencialProperties;
    [SerializeField] private Rigidbody m_RigidBody;
    [SerializeField] private CharacterController m_CharacterController;
    
    [Space]
    [SerializeField] private float speed = 300f;
    [SerializeField] private float jumpSpeed = 100f;
    [Space]
    [SerializeField] private Transform m_Grounded;
    private bool isGrounded;
    [SerializeField] private float groundRadius = 0.5f;
    [SerializeField] private LayerMask groundMask;
    [Space]
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    [Space]
    [SerializeField] private float gravityScale = 40.0f;
    [SerializeField] private float globalGravity = -9.81f;

    private void Awake()
    {
    }

    // Here we control the player
    private void Update()
    {
    }

    // Here we make physics
    private void FixedUpdate()
    {
        NormalMovement();
    }

    private void NormalMovement()
    {
        float horizontal = Input.GetAxis("Horizontal") * speed;
        float vertical = Input.GetAxis("Vertical") * speed;
        float jumpY = 0f;
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
        
        isGrounded = Physics.CheckSphere(m_Grounded.position, groundRadius, groundMask);
        Debug.Log(isGrounded);

        if (isGrounded)
        {
            if (Input.GetButton("Jump"))
            {
                m_RigidBody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            }
        }
        else
        {
            Vector3 gravity = globalGravity * gravityScale * Vector3.up;
            m_RigidBody.AddForce(gravity, ForceMode.Acceleration);
        }

        m_RigidBody.velocity = new Vector3(horizontal, jumpY, vertical) * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_Grounded.transform.position, groundRadius);
    }
}
