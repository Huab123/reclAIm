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
    [Tooltip("Minimum distance from the player at which the entity spawns")]
    public float spawnRadius = 5f;
    [Tooltip("How often (in seconds) to spawn an entity")]
    public float spawnInterval = 3f;

    [Header("Spawn Bounds")]
    [Tooltip("PolygonCollider2D that restricts the outer spawn area. Spawns are rejected outside this boundary.")]
    public PolygonCollider2D spawnBounds;
    [Tooltip("Max attempts to find a valid spawn point inside the polygon before giving up")]
    public int maxSpawnAttempts = 10;

    void Start()
    {
        InvokeRepeating(nameof(SpawnAroundPlayer), spawnInterval, spawnInterval);
    }

    public void SpawnAroundPlayer()
    {
        if (!combatSwitcher.CombatActive) return;

        if (spawnPrefab == null || player == null)
        {
            Debug.LogWarning("FollowerAndSpawner: spawnPrefab or player is not assigned.");
            return;
        }

        Vector3 spawnPosition = GetValidSpawnPosition();

        if (spawnPosition == Vector3.positiveInfinity)
        {
            Debug.LogWarning("FollowerAndSpawner: Could not find a valid spawn point inside the polygon bounds.");
            return;
        }

        Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 GetValidSpawnPosition()
    {
        int attempts = spawnBounds != null ? maxSpawnAttempts : 1;

        for (int i = 0; i < attempts; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            Vector2 offset = new Vector2(
                Mathf.Cos(angle) * spawnRadius,
                Mathf.Sin(angle) * spawnRadius
            );

            Vector2 candidate = (Vector2)player.position + offset;

            // If no bounds assigned, accept immediately
            if (spawnBounds == null || spawnBounds.OverlapPoint(candidate))
            {
                Vector3 result = new Vector3(candidate.x, candidate.y, spawnPrefab.transform.position.z);
                return result;
            }
        }

        return Vector3.positiveInfinity; // Signal failure
    }

    void LateUpdate()
    {
        if (targetToFollow == null) return;

        Vector3 pos = targetToFollow.position;
        pos.x += followOffset.x;
        pos.y += followOffset.y;
        pos.z = transform.position.z;
        transform.position = pos;
    }
}