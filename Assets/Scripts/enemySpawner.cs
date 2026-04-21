using UnityEngine;
using UnityEngine.InputSystem;

public class FollowerAndSpawner : MonoBehaviour
{
    [Header("Follower Settings")]
    [Tooltip("The entity this object will stay on top of")]
    public Transform targetToFollow;

    [Tooltip("Offset from the target's position (0,0 = exactly on top)")]
    public Vector2 followOffset = Vector2.zero;

    [Header("Spawn Settings")]
    [Tooltip("The prefab to spawn around the player")]
    public GameObject spawnPrefab;

    [Tooltip("The transform to treat as the player for spawning")]
    public Transform player;

    [Tooltip("Distance from the player at which the entity spawns")]
    public float spawnRadius = 5f;
    
    [Tooltip("How often (in seconds) to spawn an entity")]
    public float spawnInterval = 3f;


    void Start()
    {
        InvokeRepeating(nameof(SpawnAroundPlayer), spawnInterval, spawnInterval);
    }
    public void SpawnAroundPlayer()
    {
        if (spawnPrefab == null || player == null)
        {
            Debug.LogWarning("FollowerAndSpawner: spawnPrefab or player is not assigned.");
            return;
        }

        float angle = Random.Range(0f, Mathf.PI * 2f);

        Vector2 offset = new Vector2(
            Mathf.Cos(angle) * spawnRadius,
            Mathf.Sin(angle) * spawnRadius
        );

        Vector3 spawnPosition = (Vector2)player.position + offset;
        // Preserve the prefab's original Z so sorting layers work correctly
        spawnPosition.z = spawnPrefab.transform.position.z;

        Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);
    }

    void LateUpdate()
    {
        if (targetToFollow == null) return;

        // Only update X and Y — leave Z untouched for 2D layer ordering
        Vector3 pos = targetToFollow.position;
        pos.x += followOffset.x;
        pos.y += followOffset.y;
        pos.z = transform.position.z;
        transform.position = pos;
    }
}