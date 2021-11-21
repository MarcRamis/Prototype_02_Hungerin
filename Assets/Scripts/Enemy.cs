using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Rigidbody m_Rb;
    [SerializeField] private GameObject player;
    [SerializeField] private LineRenderer m_LineRenderer;
    [SerializeField] private Material m_Material;
    [SerializeField] private Material m_MaterialWhite;
    [SerializeField] private MeshRenderer m_MeshRenderer;
    [SerializeField] private MeshRenderer m_MeshRendererDir;
    private Material ownMaterial;

    [SerializeField] private float health = 100f;

    [Space]
    [Header("Movement physics")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float fleeRadius = 3f;
    [SerializeField] private float speed = 200f;
    public bool isForcedToSeek { get; set; }
    private bool isForcedSeeking = false;

    [Space]
    [Header("Rotation physics")]
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    
    [Space]
    [Header("Attack physics")] 
    [SerializeField] private float attackRadius = 4f;
    private bool startAttack = true;

    [Space]
    [Header("Ground physics")]
    [SerializeField] private Transform m_Grounded;
    private bool isGrounded;
    [SerializeField] private float groundRadius = 0.1f;
    [SerializeField] private LayerMask[] groundMask;

    [Space]
    [Header("Gravity physics")]
    [SerializeField] private float gravityScale = 10f;
    [SerializeField] private float globalGravity = -9.81f;

    private bool canTakeCollapseDamage = true;

    private void Awake()
    {
        m_LineRenderer.enabled = false;
        
        ownMaterial = m_Material;
        m_MeshRenderer.material = ownMaterial;
    }

    private void FixedUpdate()
    {
        if(health <= 0) { Destroy(gameObject); }

        StandardMove();
    }

    private void StandardMove()
    {
        foreach (LayerMask layer in groundMask)
        {
            isGrounded = Physics.CheckSphere(m_Grounded.position, groundRadius, layer);
            if (isGrounded) break;
        }

        if (isGrounded)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= detectionRadius)
            {
                RotateToDirection(player.transform.position);

                if (Vector3.Distance(transform.position, player.transform.position) <= fleeRadius)
                {
                    m_Rb.velocity = Flee(player.transform.position) * speed * Time.fixedDeltaTime;
                }
                else if (Vector3.Distance(transform.position, player.transform.position) <= attackRadius)
                {
                    m_Rb.velocity = Vector3.zero * speed * Time.fixedDeltaTime;
                    if (startAttack)
                    {
                        startAttack = false;
                        StartCoroutine("Attack");
                    }
                }
                else
                {
                    m_Rb.velocity = Seek(player.transform.position) * speed * Time.fixedDeltaTime;
                }
            }
            else
            {
                if (isForcedToSeek) 
                {
                    isForcedSeeking = true;
                    isForcedToSeek = false;
                    StartCoroutine("DisableForcedSeeking");
                }

                if (isForcedSeeking)
                {
                    RotateToDirection(player.transform.position);
                    m_Rb.velocity = Seek(player.transform.position) * speed * Time.fixedDeltaTime;
                }
            }
            if (player != null)
            {
                if (player.GetComponent<Hungerin>().isInsideCollapseRadius(transform.position)
                    && player.GetComponent<Hungerin>().isCollapsing)
                {
                    // hacer un bool doOnce y un takedamage

                    if (canTakeCollapseDamage)
                    {
                        canTakeCollapseDamage = false;
                        TakeDamage(80f);

                        StartCoroutine("EnableCanTakeCollapseDamage");
                    }
                }
            }
        }
        else
        {
            Vector3 gravity = globalGravity * gravityScale * Vector3.up;
            m_Rb.AddForce(gravity, ForceMode.Acceleration);
            m_Rb.constraints = RigidbodyConstraints.None;
        }
    }

    private void RotateToDirection(Vector3 target)
    {
        Vector3 directionRotation = target - transform.position;
        directionRotation = directionRotation.normalized;
        float targetAngle = Mathf.Atan2(directionRotation.x, directionRotation.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private Vector3 Seek(Vector3 _target)
    {
        Vector3 direction = _target - transform.position;
        direction = direction.normalized;

        return new Vector3(direction.x, 0f, direction.z);
    }

    private Vector3 Flee(Vector3 _target)
    {
        Vector3 direction = transform.position - _target;
        direction = direction.normalized;

        return new Vector3(direction.x, 0f, direction.z);
    }

    private void DrawLineRenderer(Vector3 point)
    {
        m_LineRenderer.enabled = true;
        m_LineRenderer.SetPosition(0, transform.position);
        m_LineRenderer.SetPosition(1, point);

        StartCoroutine("DisableLineRenderer");
    }
    private void MakeAttack(Vector3 _target)
    {
        if(_target != null)
        {
            Vector3 direction = _target - transform.position;
            Ray raycastTarget = new Ray(transform.position, direction.normalized);
            RaycastHit hit;

            if (Physics.Raycast(raycastTarget, out hit, attackRadius) && Vector3.Distance(_target, this.gameObject.transform.position) < attackRadius && player != null)
            {
                // Implement logic when hit player
                if(hit.collider.gameObject != null)
                {
                    player.GetComponent<Hungerin>().TakeDamage();
                }
                
            }
        }
        
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        StartCoroutine("TakingDamage");
    }
    IEnumerator TakingDamage()
    {
        yield return new WaitForSeconds(0.02f);
        m_MeshRenderer.material = m_MaterialWhite;
        m_MeshRendererDir.material = m_MaterialWhite;
        yield return new WaitForSeconds(0.02f);
        m_MeshRenderer.material = m_Material;
        m_MeshRendererDir.material = m_Material;
        yield return new WaitForSeconds(0.02f);
        m_MeshRenderer.material = m_MaterialWhite;
        m_MeshRendererDir.material = m_MaterialWhite;
        yield return new WaitForSeconds(0.02f);
        m_MeshRenderer.material = m_Material;
        m_MeshRendererDir.material = m_Material;
    }
    IEnumerator DisableForcedSeeking()
    {
        yield return new WaitForSeconds(5f);
        isForcedSeeking = false;
    }
    IEnumerator DisableLineRenderer()
    {
        yield return new WaitForSeconds(0.5f);
        m_LineRenderer.enabled = false;
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(1f);
        DrawLineRenderer(player.transform.position);
        MakeAttack(player.transform.position);
        yield return new WaitForSeconds(2f);
        startAttack = true;
    }

    IEnumerator EnableCanTakeCollapseDamage()
    {
        yield return new WaitForSeconds(1f);
        canTakeCollapseDamage = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position,detectionRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, fleeRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
