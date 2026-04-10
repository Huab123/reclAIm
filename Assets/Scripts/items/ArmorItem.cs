using UnityEngine;

public class ArmorItem : MonoBehaviour
{
    public ArmorSlotType armorType;

    [Header("Flat Stat Bonuses")]
    public float bonusMaxHealth     = 0f;
    public float bonusMoveSpeed     = 0f;

    [Header("Multiplier / Chance Bonuses")]
    public float bonusDamageMult        = 0f;
    public float bonusAttackSpeedMult   = 0f;
    public float bonusCritChance        = 0f;
    public float bonusCritDamageMult    = 0f;

    [Header("Reload / Ammo")]
    public float bonusReloadTimeReduce  = 0f;
    public int   bonusMaxAmmo           = 0;
}