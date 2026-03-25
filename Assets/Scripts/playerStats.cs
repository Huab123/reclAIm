using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }
    
    
    public float maxHealth = 100f;
    public float health;
    public float damageMult = 1f;
    public float attackSpeedMult = 1f; //this is percentile
    public float critChance = 0f; // this is the percent (100 is 100%)
    public float critDamageMult = 2f; //this is what the damage is multiplied by
    public float reloadSpeed = 1f;
    public float moveSpeed = 5f;
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
}