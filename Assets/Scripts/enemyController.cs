using UnityEngine;

public class enemyController : MonoBehaviour
{
    public float health = 10;
    public float moveSpeed = 3f;
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;

    private Rigidbody2D rb;
    private Transform player;
    private float knockbackTimer = 0f;

    void Start()
    {
        health = 10;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (knockbackTimer > 0)
            knockbackTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Check if enemy and player are in the same map area
        string enemyArea  = MapController_Dynamic.Instance.GetAreaForWorldPosition(rb.position);
        string playerArea = MapController_Dynamic.Instance.GetAreaForWorldPosition(player.position);

        if (knockbackTimer <= 0 && enemyArea != null && enemyArea == playerArea)
        {
            Vector2 direction = ((Vector2)player.position - rb.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
        }
    }

    void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Destroy(gameObject);
    }

    void ApplyKnockback(Vector2 sourcePosition)
    {
        Vector2 knockbackDir = (rb.position - sourcePosition).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
        knockbackTimer = knockbackDuration;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            ApplyKnockback(other.transform.position);
            TakeDamage(1);
        }
        if (other.tag == "bullet")
        {
            TakeDamage(2 * PlayerStats.Instance.damageMult * PlayerStats.Instance.CritCheck());
        }
    }
}