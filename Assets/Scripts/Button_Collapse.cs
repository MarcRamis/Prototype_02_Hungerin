using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Collapse : MonoBehaviour
{
    private enum DirectionDoor { UP, DOWN, DEFAULT };

    [Header("Button Settings")]
    [SerializeField] private GameObject[] interactuableGameObjects = null;
    [SerializeField] private float howMuchToOpenDoor = 1.0f;
    [SerializeField] private DirectionDoor doorDirection = DirectionDoor.DEFAULT;
    [Space]
    [SerializeField] private GameObject buttonZone = null;
    public bool activate = false; //Esto esta solo en el test luego se borra
    private bool doOnce = false;
    private Vector3[] position = null;
    private Vector3 originalPosButton = Vector3.zero;
    private bool activateDoor = false;

    // Start is called before the first frame update
    void Start()
    {
        position = new Vector3[interactuableGameObjects.Length];
        for(int i = 0; i < interactuableGameObjects.Length; i++)
        {
            position[i] = interactuableGameObjects[i].transform.position;
        }
        originalPosButton = buttonZone.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (activate && !doOnce)
        {
            int i = 0;
            foreach(GameObject door in interactuableGameObjects)
            {
                if(doorDirection == DirectionDoor.UP)
                {
                    door.transform.position = Vector3.Lerp(door.transform.position, new Vector3(position[i].x, position[i].y + howMuchToOpenDoor, position[i].z), 0.1f);
                    if (door.transform.position.y > position[i].y + howMuchToOpenDoor)
                    {
                        door.transform.position.Set(door.transform.position.x, door.transform.position.y + howMuchToOpenDoor, door.transform.position.z);
                        doOnce = true;
                    }
                    i++;
                }
                else
                {
                    door.transform.position = Vector3.Lerp(door.transform.position, new Vector3(position[i].x, position[i].y - howMuchToOpenDoor, position[i].z), 0.1f);
                    if (door.transform.position.y < position[i].y - howMuchToOpenDoor)
                    {
                        door.transform.position.Set(door.transform.position.x, door.transform.position.y - howMuchToOpenDoor, door.transform.position.z);
                        doOnce = true;
                    }
                    i++;
                }
                
            }
            
        }
        if (activateDoor)
        {
            int i = 0;
            foreach (GameObject door in interactuableGameObjects)
            {
                if (doorDirection == DirectionDoor.UP)
                {
                    door.transform.position = Vector3.Lerp(door.transform.position, new Vector3(position[i].x, position[i].y + howMuchToOpenDoor, position[i].z), 0.1f);
                    if (door.transform.position.y > position[i].y + howMuchToOpenDoor)
                    {
                        door.transform.position.Set(door.transform.position.x, door.transform.position.y + howMuchToOpenDoor, door.transform.position.z);
                        doOnce = true;
                    }
                    i++;
                }
                else
                {
                    door.transform.position = Vector3.Lerp(door.transform.position, new Vector3(position[i].x, position[i].y - howMuchToOpenDoor, position[i].z), 0.1f);
                    if (door.transform.position.y < position[i].y - howMuchToOpenDoor)
                    {
                        door.transform.position.Set(door.transform.position.x, door.transform.position.y - howMuchToOpenDoor, door.transform.position.z);
                        doOnce = true;
                    }
                    i++;
                }
            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !doOnce)
        {

            if (other.gameObject.GetComponent<Hungerin>().isCollapsing)
            {
                //Activa lo que haga el boton
                buttonZone.transform.position = new Vector3(buttonZone.transform.position.x, buttonZone.transform.position.y - 0.045f, buttonZone.transform.position.z);
                
                activateDoor = true;
                doOnce = true; 
            }
            else
            {
                float div = 0.5f;
                buttonZone.transform.position = new Vector3(buttonZone.transform.position.x, buttonZone.transform.position.y - 0.045f * div, buttonZone.transform.position.z);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && !doOnce)
        {
            buttonZone.transform.position = originalPosButton;
        }
    }
}
