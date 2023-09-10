using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatAgent : MonoBehaviour
{
    public GameObject mainHandContainer;
    public GameObject offHandContainer;
    public GameObject rangedContainer;
    Animator mainHandAnimator;
    Animator offHandAnimator;
    Animator rAnimator;

    DamageTrigger mainHandDamageTrigger;
    DamageTrigger offHandDamageTrigger;

    [Header("This gameObject must be named 'CombatAgent'. Container gameObjects must be named \n 'mainHandContainer,' 'offHandContainer,' and 'rangedContainer'.")]
    // public MeleeWeapon meleeWeapon;

    public bool meleeSheathed = true;

    public int mainHandDamage;
    public int offHandDamage;
    public float mainHandPowerAttackDamageMultiplier;
    public float offHandPowerAttackDamageMultiplier;
    public float mainHandCooldownTime;
    public float offHandCooldownTime;
    public float mainHandPowerAttackCooldownMultiplier;
    public float offHandPowerAttackCooldownMultiplier;
    float mainHandRechargedTime = 0;
    float offHandRechargedTime = 0;
    bool attackingLastFrame = false;


    void Start(){
        mainHandContainer = transform.Find("mainHandContainer").gameObject;
        offHandContainer = transform.Find("offHandContainer").gameObject;
        rangedContainer = transform.Find("rangedContainer").gameObject;

        mainHandAnimator = mainHandContainer.GetComponent<Animator>();
        offHandAnimator = offHandContainer.GetComponent<Animator>();

        rAnimator = rangedContainer.GetComponent<Animator>();

        mainHandAnimator.SetBool("Sheathed", meleeSheathed);
        offHandAnimator.SetBool("Sheathed", meleeSheathed);


        // mainHandDamage = meleeWeapon.damage;
        // meleeCooldownTime = meleeWeapon.cooldownTime;
        // mainHandPowerAttackDamageMultiplier = meleeWeapon.powerAttackDamageMultiplier;
        // mainHandPowerAttackCooldownMultiplier = meleeWeapon.powerAttackCooldownMultiplier;

        // if(mainHandDamageTrigger){
        //     mainHandDamageTrigger.active = false;
        //     mainHandDamageTrigger.damageAmount = mainHandDamage;

        //     offHandDamageTrigger.active = false;
        //     offHandDamageTrigger.damageAmount = mainHandDamage;
        // }
    }
    
    void Update(){
        if(attackingLastFrame != mainHandAnimator.GetBool("Attacking")){

            mainHandDamageTrigger.active = mainHandAnimator.GetBool("Attacking");
            attackingLastFrame = mainHandAnimator.GetBool("Attacking");
        }
    }
    
    public void M_SheatheUnsheathe(){
        meleeSheathed = !meleeSheathed;
        PlayerEquipment playerEquipment = FindObjectOfType<PlayerEquipment>();
        // Animate sheathing/unsheathing
        mainHandAnimator.SetBool("Sheathed", meleeSheathed);
        if(playerEquipment.weaponL){
            offHandAnimator.SetBool("Sheathed", meleeSheathed);
        }

        if(!meleeSheathed){
            mainHandDamageTrigger = mainHandContainer.GetComponentInChildren<DamageTrigger>();
            offHandDamageTrigger = offHandContainer.GetComponentInChildren<DamageTrigger>();

            if(mainHandDamageTrigger){
                mainHandDamageTrigger.active = false;
                mainHandDamageTrigger.damageAmount = mainHandDamage;

                mainHandDamage = playerEquipment.weaponR.meleeWeaponScriptableObject.damage;
                mainHandCooldownTime = playerEquipment.weaponR.meleeWeaponScriptableObject.cooldownTime;
                mainHandPowerAttackDamageMultiplier = playerEquipment.weaponR.meleeWeaponScriptableObject.powerAttackDamageMultiplier;
                mainHandPowerAttackCooldownMultiplier = playerEquipment.weaponR.meleeWeaponScriptableObject.powerAttackCooldownMultiplier;
            }

            if(offHandDamageTrigger){
                offHandDamageTrigger.active = false;
                offHandDamageTrigger.damageAmount = offHandDamage;

                offHandDamage = playerEquipment.weaponL.meleeWeaponScriptableObject.damage;
                offHandCooldownTime = playerEquipment.weaponL.meleeWeaponScriptableObject.cooldownTime;
                offHandPowerAttackDamageMultiplier = playerEquipment.weaponL.meleeWeaponScriptableObject.powerAttackDamageMultiplier;
                offHandPowerAttackCooldownMultiplier = playerEquipment.weaponL.meleeWeaponScriptableObject.powerAttackCooldownMultiplier;
            }
        }
    }

    public void M_MainAttack(){
        // Animate melee strike
        if(!meleeSheathed && Time.time >= mainHandRechargedTime){
            mainHandDamageTrigger.damageAmount = mainHandDamage;
            mainHandAnimator.SetTrigger("Attack");
            mainHandRechargedTime = Time.time + mainHandCooldownTime;
        }

    }

    public void M_OffAttack(){
        if(!meleeSheathed && Time.time >= offHandRechargedTime){
            offHandDamageTrigger.damageAmount = offHandDamage;
            offHandAnimator.SetTrigger("Attack");
            offHandRechargedTime = Time.time + offHandCooldownTime;
        }
    }

    public void M_MainPowerAtack(){
        // Animate melee strike
        if(!meleeSheathed && Time.time >= mainHandRechargedTime){
            mainHandDamageTrigger.damageAmount = Mathf.RoundToInt(mainHandDamage * mainHandPowerAttackDamageMultiplier);
            mainHandAnimator.SetTrigger("Power Attack");
            mainHandRechargedTime = Time.time + mainHandCooldownTime * mainHandPowerAttackCooldownMultiplier;
        }
    }

    public void M_OffPowerAtack(){
        // Animate melee strike
        if(!meleeSheathed && Time.time >= offHandRechargedTime){
            offHandDamageTrigger.damageAmount = Mathf.RoundToInt(offHandDamage * offHandPowerAttackDamageMultiplier);
            offHandAnimator.SetTrigger("Power Attack");
            offHandRechargedTime = Time.time + offHandCooldownTime * offHandPowerAttackCooldownMultiplier;
        }
    }
}
