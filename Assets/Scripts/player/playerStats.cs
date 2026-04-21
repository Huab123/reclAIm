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
    public float reloadTime = 1f; // this is the time it takes
	public int MaxAmmo = 7; // how many shots you have before needing to reload
    public float moveSpeed = 5f;


    // Utility
    public int coins = 100;
    public int mana = 0;

    // if you are to add more stats, MAKE SURE IT IS public, else nothing will be able to use the stat
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public float CritCheck()
    {
        float critRoll = Random.Range(0f, 100f);
        if (critRoll <= critChance)
        {
            //crit succeded
            return critDamageMult;
        }

        return 1f;
    }
}
// to refrence the values, use PlayerStats.Instance.var with var being the name of the variable