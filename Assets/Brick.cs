using UnityEngine;

public class TileController : MonoBehaviour
{
    public float riseHeight = 0.2f; // Height the tile will rise
    public float moveSpeed = 2f; // Speed of movement
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure player has the "Player" tag
        {
            targetPosition = originalPosition + Vector3.up * riseHeight;
            isMoving = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            targetPosition = originalPosition;
            isMoving = true;
        }
    }

    void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

            if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }
}
