using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserReflector : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    float rotationSpeed = 0.2f;

    Vector3 position;
    Vector3 direction;
    LineRenderer lr;
    public bool isOpen;

    public bool gateOpened;
    GameObject tempReflector;
    public DoorController DoorController;
    public AudioSource audioSource;
    public AudioClip clip;
    void Start()
    {
        isOpen = false;
        lr = gameObject.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (isOpen)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, position);
            RaycastHit hit;
            if (Physics.Raycast(position, direction, out hit, Mathf.Infinity))
            {
                if (hit.collider.CompareTag("Reflector") || hit.collider.CompareTag("Grabble"))
                {
                    tempReflector = hit.collider.gameObject;
                    Vector3 temp = Vector3.Reflect(direction, hit.normal);
                    hit.collider.gameObject.GetComponent<LaserReflector>().OpenRay(hit.point, temp);
                }
                if(hit.collider.CompareTag("Receiver") && !gateOpened)
                {
                    DoorController.OpenFinalDoor();
                   
                    audioSource.PlayOneShot(clip);
                    Debug.Log("Laser Received");
                    gateOpened = true;
                    //HomingMissileSystem.instance.LaunchMissiles();
                }
                lr.SetPosition(1, hit.point);
            }
            else
            {
                if (tempReflector)
                {
                    tempReflector.GetComponent<LaserReflector>().CloseRay();
                    tempReflector=null;
                }
                lr.SetPosition(1,direction*100);
            }
        }
        else
        {
            if (tempReflector)
            {
                tempReflector.GetComponent<LaserReflector>().CloseRay();
                tempReflector=null;
            }
        }
        
    }
    public void OpenRay(Vector3 pos,Vector3 dir)
    {
        isOpen = true;
        position = pos;
        direction = dir;
    }
    public void CloseRay()
    {
        isOpen = false;
        lr.positionCount = 0;
    }

  
}
