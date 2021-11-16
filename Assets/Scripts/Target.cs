using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private RaycastHit hit;

    // Update is called once per frame
    void FixedUpdate()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            transform.position = hit.point + new Vector3(0.0f, 0.01f, 0.0f);
        }
    }
}
