using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 5f;
	private Rigidbody2D rb;
	private Vector2 moveInput;
	private Animator animator;
	[SerializeField] private Weapon weapon;

	Vector2 moveDirection;
	Vector2 mousePosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
    	rb = GetComponent<Rigidbody2D>();    
		animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
       	rb.linearVelocity = moveInput * moveSpeed;
		if(Mouse.current.leftButton.wasPressedThisFrame) {
			weapon.Fire();
		}
		
		mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }

	private void FixedUpdate() {
		Vector2 aimDirection = mousePosition - rb.position;
		float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
	}

	public void Move(InputAction.CallbackContext context) {
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
}
