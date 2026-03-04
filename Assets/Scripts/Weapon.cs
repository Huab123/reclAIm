using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
	[SerializeField] private float orbitRadius = 1f;
	private Transform player;
	[SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;

    private float nextFireTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
       	player = GameObject.FindGameObjectWithTag("Player").transform; 
    }

    // Update is called once per frame
    void Update() {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
        new Vector3(Mouse.current.position.ReadValue().x, 
                    Mouse.current.position.ReadValue().y, 0f));
    	mouseWorld.z = 0f;

    	Vector2 direction = (mouseWorld - player.position).normalized;
    	transform.position = player.position + (Vector3)(direction * orbitRadius);

    	float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    	transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

	public void Fire() {
		if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        // Spawn bullet at firepoint
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Pass direction to bullet (gun's right = forward since sprite points right)
        bulletObj.GetComponent<Bullet>().Init(firePoint.right);
	}
}
