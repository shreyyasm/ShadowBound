using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSource : MonoBehaviour
{
    [SerializeField] Transform laserStartPoint;
    LineRenderer lr;
    GameObject tempReflector;
    public DoorController doorController; 

    void Start()
    {
        lr = gameObject.GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, laserStartPoint.position);
    }
    public AudioSource audioSource;
    public AudioClip clip;
    public bool gateOpened;
    void Update()
    {
        Vector3 direction = laserStartPoint.forward; // Update direction dynamically
        RaycastHit hit;

        if (Physics.Raycast(laserStartPoint.position, direction, out hit, Mathf.Infinity))
        {
            if (hit.collider.CompareTag("Reflector") || hit.collider.CompareTag("Grabble"))
            {
                tempReflector = hit.collider.gameObject;
                Vector3 reflectedDirection = Vector3.Reflect(direction, hit.normal);
                hit.collider.gameObject.GetComponent<LaserReflector>().OpenRay(hit.point, reflectedDirection);
            }
            else if (hit.collider.CompareTag("Receiver") && !gateOpened)
            {
                Debug.Log("Laser Received");
                doorController.OpenFinalDoor();
                audioSource.PlayOneShot(clip);
                gateOpened = true;
            }

            lr.SetPosition(1, hit.point);
        }
        else
        {
            if (tempReflector)
            {
                tempReflector.GetComponent<LaserReflector>().CloseRay();
                tempReflector = null;
            }

            // Ensure the laser continues straight
            lr.SetPosition(1, laserStartPoint.position + direction * 200);
        }

        // Update the laser start position
        lr.SetPosition(0, laserStartPoint.position);
    }
}
