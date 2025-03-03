using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XYZ : MonoBehaviour
{

    public GameObject CameraBig;
    private bool isInTrigger = false;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            CameraBig.SetActive(true);
            isInTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            CameraBig.SetActive(false);
        }
    }

}
