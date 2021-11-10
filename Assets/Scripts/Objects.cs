using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    private Rigidbody m_Rigidbody;
    [SerializeField] private EssencialProperties m_essencialProperties;
    private float speedToTarget = 800f;
    
    private void Awake()
    {
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        m_Rigidbody.mass = m_essencialProperties.weight;
    }

    public float GetWeight() { return m_essencialProperties.weight; }
    public float GetLargeSize() { return m_essencialProperties.largeSize; }

    public void MoveToPlayer(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        direction = direction.normalized;

        m_Rigidbody.AddForce(direction * speedToTarget, ForceMode.Acceleration);
        this.gameObject.tag = "Untagged";
        StartCoroutine("DestroyItself");
    }

    IEnumerator DestroyItself()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
}
