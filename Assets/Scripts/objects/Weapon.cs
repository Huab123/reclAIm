using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

public class Weapon : MonoBehaviour
{
	[SerializeField] private float orbitRadius = 1f;
	private Transform playerPos;
	[SerializeField] private GameObject bulletPrefab;
	[SerializeField] private GameObject player;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private int currentAmmo = 7;
    private bool reloading = false;
    private float nextFireTime = 0f;
	public TextMeshProUGUI AmmoText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    player = GameObject.FindGameObjectWithTag("Player");
       	playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        currentAmmo = PlayerStats.Instance.MaxAmmo;
        UpdateAmmoDisplay();
    }

    // Update is called once per frame
    void Update() {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(
        new Vector3(Mouse.current.position.ReadValue().x, 
                    Mouse.current.position.ReadValue().y, 0f));
    	mouseWorld.z = 0f;

    	Vector2 direction = (mouseWorld - playerPos.position).normalized;
    	transform.position = playerPos.position + (Vector3)(direction * orbitRadius);

    	float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    	transform.rotation = Quaternion.Euler(0f, 0f, angle);
	    if (Keyboard.current.rKey.wasPressedThisFrame)
	    {
		    Reload();
	    }
    }
	private void UpdateAmmoDisplay(){
		if(reloading)
		{
			AmmoText.text = "Reloading...";
		}
		else
		{
			AmmoText.text = "Ammo: \n" + currentAmmo + "/" + PlayerStats.Instance.MaxAmmo;
		}
	}

    public void Reload()
    {
	    // do nothing if ammo is already at max or if we are already reloading
	    if (currentAmmo >= PlayerStats.Instance.MaxAmmo || reloading) return;

	    StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
	    
	    reloading = true;
	    UpdateAmmoDisplay();
	    yield return new WaitForSeconds(PlayerStats.Instance.reloadTime);

	    currentAmmo = PlayerStats.Instance.MaxAmmo;
	    reloading = false;
	    UpdateAmmoDisplay();
    }

	public void Fire() {
		
		if (Time.time < nextFireTime|| reloading ) return;
		if (currentAmmo <= 0 ){
			Reload();
			return;
		}
        nextFireTime = Time.time + (fireRate / PlayerStats.Instance.attackSpeedMult); //this is where modding attack speed needs to go

        // Spawn bullet at firepoint
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
		currentAmmo -= 1;
		UpdateAmmoDisplay();
        // Pass direction to bullet (gun's right = forward since sprite points right)
        bulletObj.GetComponent<Bullet>().Init(firePoint.right);
        if (SoundEffectManager.Instance != null)
        {
            SoundEffectManager.Instance.PlayGunShot();
        }
    }
}
