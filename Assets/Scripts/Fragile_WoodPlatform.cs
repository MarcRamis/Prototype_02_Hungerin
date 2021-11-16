using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragile_WoodPlatform : MonoBehaviour
{
    [SerializeField] private float weightToBreak = 2.0f;
    [SerializeField] private GameObject[] nearPlatforms = null;

    public void DropPlatform()
    {
        this.gameObject.GetComponent<Rigidbody>().useGravity = true;
        this.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log(other.gameObject.GetComponent<Rigidbody>().velocity.y);
            if (other.gameObject.GetComponent<Hungerin>().GetWeight() >= weightToBreak && other.gameObject.GetComponent<Rigidbody>().velocity.y > 0.0f)
            {

                foreach(GameObject platform in nearPlatforms)
                {
                    platform.GetComponent<Fragile_WoodPlatform>().DropPlatform();
                }
            }
        }
    }
}
