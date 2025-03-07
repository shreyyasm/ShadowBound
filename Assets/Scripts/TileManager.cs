using UnityEngine;
using System.Collections.Generic;

public class TileManager : MonoBehaviour
{
    public Transform player;
    public float activationDistance = 5f; // Distance in front of the player to activate tiles

    private List<TileControllers> tiles = new List<TileControllers>();

    void Start()
    {
        // Get all TileController components in the scene
        foreach (TileControllers tile in FindObjectsOfType<TileControllers>())
        {
            tiles.Add(tile);
        }
    }

    void Update()
    {
        foreach (TileControllers tile in tiles)
        {
            // Check if the tile is in front of the player and within activation distance
            if (tile.transform.position.z > player.position.z && tile.transform.position.z - player.position.z < activationDistance)
            {
                tile.ActivateTile();
            }
        }
    }
}
