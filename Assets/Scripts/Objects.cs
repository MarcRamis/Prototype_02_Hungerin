using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    private enum SizeType { SMALL, MEDIUM, BIG, DEFAULT}; 
    private Rigidbody m_Rigidbody;
    [SerializeField] private EssencialProperties m_essencialProperties;
    [SerializeField] private SizeType typeObj = SizeType.DEFAULT;
    private float sumSize = 0.0f;
    private float sumWeight = 0.0f;
    private float speedToTarget = 800f;
    
    private void Awake()
    {
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        m_Rigidbody.mass = m_essencialProperties.weight;
        switch(typeObj)
        {
            case (SizeType.SMALL):
                sumSize = 2;
                sumWeight = 0.1f;
                break;
            case (SizeType.MEDIUM):
                sumSize = 10;
                sumWeight = 0.5f;
                break;
            case (SizeType.BIG):
                sumSize = 20;
                sumWeight = 1f;
                break;
            case (SizeType.DEFAULT):
                Debug.Log("Forgot to init values");
                break;
            default:
                Debug.Log("Fatal Error no enum found");
                break;
        }
    }

    public float GetWeight() { return m_essencialProperties.weight; }
    public float GetLargeSize() { return m_essencialProperties.largeSize; }

    public float GetSumWeight() { return sumWeight; }

    public float GetSumSize() { return sumSize; }

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
