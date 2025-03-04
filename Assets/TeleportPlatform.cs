using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlatform : MonoBehaviour
{
    public bool canTeleport;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.CompareTag("Tiles"))
        {
            canTeleport = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
       
            canTeleport = false;
        
    }
}
