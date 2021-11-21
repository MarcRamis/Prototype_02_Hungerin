using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragile_WoodPlatform : MonoBehaviour
{
    [SerializeField] private float weightToBreak = 2.0f;
    [SerializeField] private GameObject[] nearPlatforms = null;
    [SerializeField] private float minVelocityTobreak = -8.0f;

    public void DropPlatform()
    {
        this.gameObject.GetComponentInParent<Rigidbody>().useGravity = true;
        this.gameObject.GetComponentInParent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<Hungerin>().GetWeight() >= weightToBreak && other.gameObject.GetComponent<Rigidbody>().velocity.y < minVelocityTobreak)
            {

                foreach(GameObject platform in nearPlatforms)
                {
                    platform.GetComponentInChildren<Fragile_WoodPlatform>().DropPlatform();
                }
            }
        }
    }
}
