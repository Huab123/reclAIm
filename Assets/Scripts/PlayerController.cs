using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerMovement : MonoBehaviour
{

	private Rigidbody2D rb;
	private Vector2 moveInput;
	private Animator animator;
	[SerializeField] private Weapon weapon;
	[SerializeField] private GameObject menu;
	public TextMeshProUGUI healthText;


	Vector2 moveDirection;
	Vector2 mousePosition;
	


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
		
    	rb = GetComponent<Rigidbody2D>();    
		animator = GetComponent<Animator>();
		PlayerStats.Instance.health = PlayerStats.Instance.maxHealth;
		UpdateHealthDisplay();
    }

    // Update is called once per frame
    void Update() {
		// Pause time if Menu UI is opened
		if (Keyboard.current.escapeKey.wasPressedThisFrame) {
        	bool opening = !menu.activeSelf;
        	Time.timeScale = opening ? 0f : 1f;
        	if (opening) {
        		moveInput = Vector2.zero;
        		rb.linearVelocity = Vector2.zero;
        	}
    	}
		else if (menu.activeSelf == true) return;

		// Set player speed
       	rb.linearVelocity = moveInput * PlayerStats.Instance.moveSpeed;

		// Shoot bullet if leftMouse pressed
		if(Mouse.current.leftButton.wasPressedThisFrame) {
			weapon.Fire();
		}
		
		// Set up gun rotation based on mouse position
		mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
		
		//update the player health display
		UpdateHealthDisplay(); // this isn't the best solution (as its inefficent) but it will abosulutely make sure the health is correct
    }

	private void FixedUpdate() {
		Vector2 aimDirection = mousePosition - rb.position;
		float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
	}

	public void Move(InputAction.CallbackContext context) {
		if (menu.activeSelf == true) return;
		animator.SetBool("isWalking", true);
		
		if(context.canceled) {
			animator.SetBool("isWalking", false);
			animator.SetFloat("LastInputX", moveInput.x);
			animator.SetFloat("LastInputY", moveInput.y);
		}
		moveInput = context.ReadValue<Vector2>();
		animator.SetFloat("InputX", moveInput.x);
		animator.SetFloat("InputY", moveInput.y);
	}
	
	private void Heal(float amount)
	{
		Debug.Log("player healed: " + amount);
		if (PlayerStats.Instance.health>= PlayerStats.Instance.maxHealth)
		{
			PlayerStats.Instance.health = PlayerStats.Instance.maxHealth; //we if want overheal later we can do that
		}
		
	}

	private void TakeDamage(float amount)
	{
		Debug.Log("player damaged: " + amount);
		if (PlayerStats.Instance.health<=0)
		{
			Debug.Log("player is dead"); 	//TODO: add death state
		}

	}
	void UpdateHealthDisplay()
	{
		healthText.text = "health: " + PlayerStats.Instance.health + "/" + PlayerStats.Instance.maxHealth ;
	}
}
