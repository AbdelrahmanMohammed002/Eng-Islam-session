using System.Collections;
using UnityEngine;

public class CanRespawner : MonoBehaviour
{
    [Tooltip("Prefab of the object to respawn.")]
    public GameObject objectPrefab;

    [Tooltip("Time in seconds between each respawn.")]
    public float respawnInterval = 1.0f;

    // Store the prefab's original spawn position and rotation
    private Vector3 originalSpawnPosition;
    private Quaternion originalSpawnRotation;

    void Start()
    {
        // Get the prefab's transform reference (position & rotation)
        // Note: This only works if the prefab is already placed in the scene
        originalSpawnPosition = objectPrefab.transform.position;
        originalSpawnRotation = objectPrefab.transform.rotation;

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        for (int x =0 ;x<5;x++) { 
            yield return new WaitForSeconds(respawnInterval);
            Instantiate(objectPrefab, originalSpawnPosition, originalSpawnRotation);
        }
    }
}
