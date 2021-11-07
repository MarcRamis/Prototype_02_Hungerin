using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hungerin : MonoBehaviour
{
    [SerializeField] private Rigidbody m_RigidBody;
    [SerializeField] private Transform m_Target;
    [SerializeField] LineRenderer m_LineRenderer;

    [Space]
    [SerializeField] EssencialProperties m_EssencialProperties;

    [Space]
    [SerializeField] private float speed = 300f;
    [SerializeField] private float jumpSpeed = 30f;
    [SerializeField] private float jumpForwardSpeed = 3f;
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
    [Space]
    [SerializeField] private float maxTongueDistance = 10.0f;


    private void Awake()
    {
        m_RigidBody.mass = m_EssencialProperties.weight;
    }

    private void Start()
    {
        m_LineRenderer.enabled = false;
    }

    // Here we control the player
    private void Update()
    {
    }

    // Here we make physics
    private void FixedUpdate()
    {
        NormalMovement();
        UseTongue();
    }

    private void NormalMovement()
    {
        // Inputs movement

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 directionRotation = m_Target.position - transform.position;
        directionRotation = directionRotation.normalized;

        // Rotation on forward direction
        float targetAngle = Mathf.Atan2(directionRotation.x, directionRotation.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        isGrounded = Physics.CheckSphere(m_Grounded.position, groundRadius, groundMask);
        
        if (isGrounded)
        {
            if (Input.GetButton("Jump"))
            {
                m_RigidBody.AddForce(new Vector3(direction.x * jumpForwardSpeed, jumpSpeed, direction.z * jumpForwardSpeed), ForceMode.Impulse);
            }

            m_RigidBody.velocity = new Vector3(horizontal, 0f, vertical) * speed * Time.fixedDeltaTime;
        }
        else
        {
            Vector3 gravity = globalGravity * gravityScale * Vector3.up;
            m_RigidBody.AddForce(gravity, ForceMode.Acceleration);
        }
    }

    private void UseTongue()
    {
        if (Input.GetButton("LaunchTongue"))
        {
            Vector3 direction = m_Target.position - transform.position;
            Ray raycastTarget = new Ray(transform.position, direction.normalized);
            RaycastHit hit;

            if (Physics.Raycast(raycastTarget, out hit, maxTongueDistance))
            {
                m_LineRenderer.enabled = true;
                m_LineRenderer.SetPosition(0, transform.position);
                m_LineRenderer.SetPosition(1, hit.point);
                StartCoroutine("DisableTongue");
            }
        }
    }

    IEnumerator DisableTongue()
    {
        yield return new WaitForSeconds(1f);
        m_LineRenderer.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_Grounded.transform.position, groundRadius);

        Gizmos.DrawLine(transform.position, m_Target.position);
    }
}
