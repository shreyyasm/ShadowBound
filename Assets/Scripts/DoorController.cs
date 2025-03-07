using UnityEngine;

public class DoorController : MonoBehaviour
{
    public static DoorController instance;
    public float openHeight = 3f; // How high the door moves
    public float speed = 2f; // Speed of movement
    public bool autoOpen = false; // Set to true for automatic opening when near

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpening = false;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + Vector3.up * openHeight;
    }

    void Update()
    {
        if (isOpening)
        {
            // Move door upwards smoothly
            transform.position = Vector3.MoveTowards(transform.position, openPosition, speed * Time.deltaTime);
        }
    }

    public void OpenFinalDoor()
    {
        isOpening = true;
    }
   
    // Optional: Trigger-based automatic opening
    private void OnTriggerEnter(Collider other)
    {
        if (autoOpen && other.CompareTag("Player"))
        {
            OpenFinalDoor();
        }
    }

}
