using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    [SerializeField] private EssencialProperties m_essencialProperties;
    [SerializeField] private GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetMass() { return m_essencialProperties.weight; }

    public float GetLargeSize() { return m_essencialProperties.largeSize; }

}
