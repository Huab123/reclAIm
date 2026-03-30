using UnityEngine;

public class Bullet : MonoBehaviour
{
	[SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 3f;

    private Vector2 direction;

    public void Init(Vector2 dir) {
        direction = dir.normalized;
        Destroy(gameObject, lifetime);
    }

    void Update() {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Enemy")) {
			Destroy(gameObject); 
			// Change to damage and not instant destroy
        }
		else if (other.CompareTag("Collision")) {
			Destroy(gameObject);
		}
    }

	public void Init(Vector2 dir, Collider2D shooterCollider) {
    	direction = dir.normalized;
    	Destroy(gameObject, lifetime);

	    if (shooterCollider != null)
   			Physics2D.IgnoreCollision(GetComponent<Collider2D>(), shooterCollider);
}

}
