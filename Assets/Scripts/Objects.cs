using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objects : MonoBehaviour
{
    [SerializeField] private float largeSize;
    [SerializeField] private float mass;
    [SerializeField] private GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(obj, this.transform.position, Quaternion.identity);
        this.gameObject.GetComponent<MeshFilter>().mesh = obj.GetComponent<MeshFilter>().sharedMesh;
        this.gameObject.GetComponent<MeshRenderer>().materials = obj.GetComponent<MeshRenderer>().sharedMaterials;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float GetMass() { return mass; }

    public float GetLargeSize() { return largeSize; }

}
