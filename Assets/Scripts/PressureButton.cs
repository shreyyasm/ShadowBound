using UnityEngine;

public class PressureButton : MonoBehaviour
{
    public Transform door; // Assign the door in the Inspector
    public float doorOpenHeight = 3f; // How high the door moves
    public float doorSpeed = 2f; // Speed of door movement
    public float buttonPressDepth = 0.2f; // How much the button moves down
    public float buttonSpeed = 5f; // Speed of button movement

    private Vector3 buttonStartPos;
    private Vector3 buttonPressedPos;
    private Vector3 doorStartPos;
    private Vector3 doorOpenPos;
    private bool isPlayerOnButton = false;

    void Start()
    {
        buttonStartPos = transform.position;
        buttonPressedPos = buttonStartPos + Vector3.down * buttonPressDepth;

        doorStartPos = door.position;
        doorOpenPos = doorStartPos + Vector3.up * doorOpenHeight;
    }

    void Update()
    {
        // Move button
        transform.position = Vector3.MoveTowards(transform.position, isPlayerOnButton ? buttonPressedPos : buttonStartPos, buttonSpeed * Time.deltaTime);

        // Move door
        door.position = Vector3.MoveTowards(door.position, isPlayerOnButton ? doorOpenPos : doorStartPos, doorSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Main") || other.CompareTag("Grabble"))
        {
            isPlayerOnButton = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Main") || other.CompareTag("Grabble"))
        {
            isPlayerOnButton = false;
        }
    }
}
