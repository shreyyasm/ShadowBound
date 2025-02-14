using UnityEngine;

public class TileControllers : MonoBehaviour
{
    public float targetHeight = 0f; // Ground level
    public float riseSpeed = 5f; // Speed of rising
    private bool isRising = false;

    void Update()
    {
        if (isRising)
        {
            // Move tile towards ground level
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, targetHeight, transform.position.z), riseSpeed * Time.deltaTime);
        }
    }

    public void ActivateTile()
    {
        isRising = true;
    }
}
