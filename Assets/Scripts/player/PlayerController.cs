using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

	private Rigidbody2D rb;
	private Vector2 moveInput;
	private Animator animator;
	[SerializeField] private Weapon weapon;
	[SerializeField] private GameObject menu;
	public TextMeshProUGUI healthText;
	[Header("Footstep Audio")]
	[SerializeField] private AudioSource footstepSource;
	[SerializeField] private AudioClip footstepClip;
	[SerializeField] private float footstepVolume = 0.6f;
	public Image healthBarFill;
	public TextMeshProUGUI coinsText;


	Vector2 moveDirection;
	Vector2 mousePosition;
	


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
		
    	rb = GetComponent<Rigidbody2D>();    
		animator = GetComponent<Animator>();
		PlayerStats.Instance.health = PlayerStats.Instance.maxHealth;
		UpdateHealthDisplay();
		UpdateCurrencyDisplay();
	    
		if (footstepSource == null)
        {
            footstepSource = GetComponent<AudioSource>();
        }

        if (footstepSource != null)
        {
            footstepSource.clip = footstepClip;
            footstepSource.loop = true;
            footstepSource.playOnAwake = false;
            footstepSource.volume = footstepVolume;
        }	
    }
	

    private bool IsAnyShopOpen() {
        Shop[] shops = FindObjectsByType<Shop>(FindObjectsSortMode.None);
        foreach (Shop s in shops) {
            if (s.IsOpen) return true;
        }
        return false;
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
				StopFootsteps();
			}
    	}
		else if (menu.activeSelf == true) return;

		// Set player speed
       	rb.linearVelocity = moveInput * PlayerStats.Instance.moveSpeed;

		// Shoot bullet if leftMouse pressed
		if(Mouse.current.leftButton.wasPressedThisFrame && !IsAnyShopOpen()) {
			weapon.Fire();
		}
		
		// Set up gun rotation based on mouse position
		mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
		HandleFootsteps();
	}

	private void FixedUpdate() {
		Vector2 aimDirection = mousePosition - rb.position;
		float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
	}

	public void Move(InputAction.CallbackContext context) {
		if (menu.activeSelf == true) return;
		moveInput = context.ReadValue<Vector2>();
    	bool isMoving = moveInput != Vector2.zero;
		animator.SetBool("isWalking", isMoving);
		
		if(context.canceled) {
			animator.SetFloat("LastInputX", moveInput.x);
			animator.SetFloat("LastInputY", moveInput.y);
			StopFootsteps();
		}
		moveInput = context.ReadValue<Vector2>();
		animator.SetFloat("InputX", moveInput.x);
		animator.SetFloat("InputY", moveInput.y);
	}

	public void StopMovement() {
		moveInput = Vector2.zero;
		if (animator != null) {
			animator.SetBool("isWalking", false);
		}
		if (rb != null) {
			rb.linearVelocity = Vector2.zero;
		}
	}
	private void HandleFootsteps()
    {
        if (footstepSource == null || footstepClip == null) return;

        bool isMoving = moveInput != Vector2.zero && rb.linearVelocity.sqrMagnitude > 0.01f;

        if (isMoving)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.Play();
            }
        }
        else
        {
            StopFootsteps();
        }
    }

    private void StopFootsteps()
    {
        if (footstepSource != null && footstepSource.isPlaying)
        {
            footstepSource.Stop();
        }
    }


	
	public void Heal(float amount)
	{
		Debug.Log("player healed: " + amount);
		PlayerStats.Instance.health += amount;
		if (PlayerStats.Instance.health>= PlayerStats.Instance.maxHealth)
		{
			PlayerStats.Instance.health = PlayerStats.Instance.maxHealth; //we if want overheal later we can do that
		}
		UpdateHealthDisplay();
	}

	public void TakeDamage(float amount)
	{
		Debug.Log("player damaged: " + amount);
		PlayerStats.Instance.health -= amount;
		if (PlayerStats.Instance.health<=0)
		{
			Debug.Log("player is dead"); 	//TODO: add death state
		}
		UpdateHealthDisplay();
	}
	public void UpdateHealthDisplay()
	{
		healthText.text = "HP: " + PlayerStats.Instance.health + "/" + PlayerStats.Instance.maxHealth;
		Vector3 scale = healthBarFill.transform.localScale;
		scale.x = (float)PlayerStats.Instance.health / (float)PlayerStats.Instance.maxHealth;
		healthBarFill.transform.localScale = scale;
	}

	public void UpdateCurrencyDisplay()
	{
		if (coinsText != null)
		{
			coinsText.text = "$ " + PlayerStats.Instance.coins;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Enemy")
		{
			TakeDamage(1);
		}
	}
}
