using UnityEngine;

public class enemyController : MonoBehaviour
{
    private float health = 10;
    public float moveSpeed = 3f;

    private Rigidbody2D rb;
    private Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = 10;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Prevent the enemy from rotating when it hits walls
        rb.freezeRotation = true;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector2 direction = ((Vector2)player.position - rb.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            TakeDamage(1);
        }

        if (other.tag == "bullet")
        {
            TakeDamage(2);
        }
    }
}