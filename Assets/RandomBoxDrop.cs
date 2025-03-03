using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBoxDrop : MonoBehaviour
{
    public float delay = 3f; // Time in seconds before enabling gravity
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // Disable gravity initially
            Invoke("EnableGravity", delay);
        }
        else
        {
            Debug.LogError("Rigidbody not found on " + gameObject.name);
        }
    }

    void EnableGravity()
    {
        rb.useGravity = true;
    }
}
