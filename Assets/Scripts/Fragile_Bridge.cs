using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragile_Bridge : MonoBehaviour
{
    [SerializeField] private float weightSupported = 5.0f;
    [SerializeField] private float timerToReturn = 3.0f;
    [SerializeField] private GameObject[] bridges = null;
    private Vector3 pos = Vector3.zero;
    private bool startTimer = false;
    private float currentTime = 0.0f;

    private void Start()
    {
        pos = transform.position;
    }

    private void Update()
    {
        if(startTimer)
        {
            if(currentTime >= timerToReturn)
            {
                //ResetBridge();
                foreach(GameObject bridge in bridges)
                {
                    bridge.GetComponent<Fragile_Bridge>().ResetBridge();
                }
            }
            else
            {
                currentTime += Time.deltaTime;
            }
        }
    }

    public void DropBridge()
    {
        this.gameObject.GetComponent<Rigidbody>().useGravity = true;
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        startTimer = true;
    }

    public void ResetBridge()
    {
        this.gameObject.GetComponent<Rigidbody>().useGravity = false;
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        transform.position = pos;
        transform.localRotation = Quaternion.identity;
        currentTime = 0.0f;
        startTimer = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(other.gameObject.GetComponent<Hungerin>().GetWeight() > weightSupported)
            {
                //DropBridge();
                foreach (GameObject bridge in bridges)
                {
                    bridge.GetComponent<Fragile_Bridge>().DropBridge();
                }
            }
            
        }
    }
}
