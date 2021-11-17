using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    private enum ButtonActivate { BRIDGE, DOOR, DEFAULT};
    private enum RotationBridge { X, Y, Z, DEFAULT };

    [Header("Button Settings")]
    [SerializeField] private float minWeight = 5.0f;
    [SerializeField] private ButtonActivate typeAction = ButtonActivate.DEFAULT;
    [SerializeField] private RotationBridge typeOfRotation = RotationBridge.DEFAULT;
    [SerializeField] private GameObject[] interactuableGameObjects = null;
    [SerializeField] private float howMuchToOpenDoor = 1.0f;
    [Space]
    [SerializeField] private GameObject buttonZone = null;
    public bool activate = false; //Esto esta solo en el test luego se borra
    private bool doOnce = false;
    private Vector3 vectorToRotate = Vector3.zero;
    private Vector3 position = Vector3.zero;
    private Vector3 originalPosButton = Vector3.zero;
    private bool activateDoor = false;

    // Start is called before the first frame update
    void Start()
    {
        switch(typeOfRotation)
        {
            case (RotationBridge.X):
                vectorToRotate = Vector3.right;
                break;
            case (RotationBridge.Y):
                vectorToRotate = Vector3.up;
                break;
            case (RotationBridge.Z):
                vectorToRotate = Vector3.forward;
                break;
        }

        position = interactuableGameObjects[0].transform.position;
        originalPosButton = buttonZone.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (activate && !doOnce)
            switch (typeAction)
            {
                case (ButtonActivate.BRIDGE):
                    foreach (GameObject bridge in interactuableGameObjects)
                    {
                        switch (typeOfRotation)
                        {
                            case (RotationBridge.X):
                                if (bridge.transform.localRotation.x > 0)
                                {
                                    bridge.transform.Rotate(vectorToRotate, -90.0f);
                                }
                                else
                                {
                                    bridge.transform.Rotate(vectorToRotate, 90.0f);
                                }
                                doOnce = true;
                                break;
                            case (RotationBridge.Y):
                                if (bridge.transform.localRotation.y > 0)
                                {
                                    bridge.transform.Rotate(vectorToRotate, -90.0f);
                                }
                                else
                                {
                                    bridge.transform.Rotate(vectorToRotate, 90.0f);
                                }
                                doOnce = true;
                                break;
                            case (RotationBridge.Z):
                                if (bridge.transform.localRotation.z > 0)
                                {
                                    bridge.transform.Rotate(vectorToRotate, -90.0f);
                                }
                                else
                                {
                                    bridge.transform.Rotate(vectorToRotate, 90.0f);
                                }
                                doOnce = true;
                                break;
                        }
                        
                    }
                    break;
                case (ButtonActivate.DOOR):
                    //interactuableGameObjects[0].GetComponent<Rigidbody>().velocity = Vector3.up * 5.0f * Time.fixedDeltaTime;
                    
                    interactuableGameObjects[0].transform.position = Vector3.Lerp(interactuableGameObjects[0].transform.position, new Vector3(position.x, position.y + howMuchToOpenDoor, position.z), 0.1f);
                    if (interactuableGameObjects[0].transform.position.y > position.y + howMuchToOpenDoor)
                    {
                        //interactuableGameObjects[0].GetComponent<Rigidbody>().velocity = Vector3.zero;
                        interactuableGameObjects[0].transform.position.Set(interactuableGameObjects[0].transform.position.x, interactuableGameObjects[0].transform.position.y + howMuchToOpenDoor, interactuableGameObjects[0].transform.position.z);
                        doOnce = true;
                    }

                    break;
                case (ButtonActivate.DEFAULT):
                    Debug.Log("Forgot to put what does this button do");
                    break;
                default:
                    Debug.Log("This script doesn't detect the enum");
                    break;
            }

        if(activateDoor)
        {
            interactuableGameObjects[0].transform.position = Vector3.Lerp(interactuableGameObjects[0].transform.position, new Vector3(position.x, position.y + howMuchToOpenDoor, position.z), 0.1f);
            if (interactuableGameObjects[0].transform.position.y > position.y + howMuchToOpenDoor)
            {
                //interactuableGameObjects[0].GetComponent<Rigidbody>().velocity = Vector3.zero;
                interactuableGameObjects[0].transform.position.Set(interactuableGameObjects[0].transform.position.x, interactuableGameObjects[0].transform.position.y + howMuchToOpenDoor, interactuableGameObjects[0].transform.position.z);
                activateDoor = false;
            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && !doOnce)
        {
            
            if (other.gameObject.GetComponent<Hungerin>().GetWeight() >= minWeight)
            {
                //Activa lo que haga el boton
                buttonZone.transform.position = new Vector3(buttonZone.transform.position.x, buttonZone.transform.position.y - 0.045f, buttonZone.transform.position.z);
                switch (typeAction)
                {
                    case (ButtonActivate.BRIDGE):
                        foreach (GameObject bridge in interactuableGameObjects)
                        {
                            switch (typeOfRotation)
                            {
                                case (RotationBridge.X):
                                    if (bridge.transform.localRotation.x > 0)
                                    {
                                        bridge.transform.Rotate(vectorToRotate, -90.0f);
                                    }
                                    else
                                    {
                                        bridge.transform.Rotate(vectorToRotate, 90.0f);
                                    }
                                    doOnce = true;
                                    break;
                                case (RotationBridge.Y):
                                    if (bridge.transform.localRotation.y > 0)
                                    {
                                        bridge.transform.Rotate(vectorToRotate, -90.0f);
                                    }
                                    else
                                    {
                                        bridge.transform.Rotate(vectorToRotate, 90.0f);
                                    }
                                    doOnce = true;
                                    break;
                                case (RotationBridge.Z):
                                    if (bridge.transform.localRotation.z > 0)
                                    {
                                        bridge.transform.Rotate(vectorToRotate, -90.0f);
                                    }
                                    else
                                    {
                                        bridge.transform.Rotate(vectorToRotate, 90.0f);
                                    }
                                    doOnce = true;
                                    break;
                            }

                        }
                        break;
                    case (ButtonActivate.DOOR):
                        activateDoor = true;
                        doOnce = true;
                        break;
                    case (ButtonActivate.DEFAULT):
                        Debug.Log("Forgot to put what does this button do");
                        break;
                    default:
                        Debug.Log("This script doesn't detect the enum");
                        break;
                }
            }
            else
            {
                float div = other.gameObject.GetComponent<Hungerin>().GetWeight() / minWeight;
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
