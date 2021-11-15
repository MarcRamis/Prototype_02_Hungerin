using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Rigidbody m_Rb;
    [SerializeField] private GameObject player;
    [SerializeField] private LineRenderer m_LineRenderer;

    [SerializeField] private float health = 100f;

    [Space]
    [Header("Movement physics")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float fleeRadius = 3f;
    [SerializeField] private float speed = 200f;    

    [Space]
    [Header("Rotation physics")]
    [SerializeField] private float turnSmoothTime;
    private float turnSmoothVelocity;
    
    [Space]
    [Header("Attack physics")] 
    [SerializeField] private float attackRadius = 4f;
    private bool startAttack = true;

    private void Awake()
    {
        m_LineRenderer.enabled = false;
    }

    private void FixedUpdate()
    {
        if(Vector3.Distance(transform.position, player.transform.position) <= detectionRadius)
        {
            //Vector3 directionRotation = target.position - transform.position;
            //directionRotation = directionRotation.normalized;
            //float targetAngle = Mathf.Atan2(directionRotation.x, directionRotation.z) * Mathf.Rad2Deg;
            //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            //transform.rotation = Quaternion.Euler(0f, angle, 0f);

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
        
        if (player.GetComponent<Hungerin>().isInsideCollapseRadius(transform.position) 
            && player.GetComponent<Hungerin>().isCollapsing)
        {
            // hacer un bool doOnce y un takedamage
            Debug.Log("a");
        }
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
        Vector3 direction = _target - transform.position;
        Ray raycastTarget = new Ray(transform.position, direction.normalized);
        RaycastHit hit;

        if (Physics.Raycast(raycastTarget, out hit, attackRadius))
        {
            // Implement logic when hit player
            //hit.collider.gameObject.GetComponent<Hungerin>().TakeDamage(1f);
            Debug.Log(hit.collider.gameObject.name + " TakeDamage");
        }
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
