using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private Vector3 mouseDir;
    private RaycastHit hit;
    
    // Update is called once per frame
    void FixedUpdate()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) /*&& hit.collider.tag == "Ground"*/)
        {
            transform.position = hit.point + new Vector3(0.0f,0.15f,0.0f);
            Debug.Log(hit.point + new Vector3(0.0f, 0.15f, 0.0f));
        }
    }
}
