using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    [SerializeField] private EssencialProperties m_essencialProperties;
    private Rigidbody m_Rigidbody;

    private void Awake()
    {
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        m_Rigidbody.mass = m_essencialProperties.weight;
    }

    public float GetMass() { return m_essencialProperties.weight; }
    public float GetLargeSize() { return m_essencialProperties.largeSize; }
}
