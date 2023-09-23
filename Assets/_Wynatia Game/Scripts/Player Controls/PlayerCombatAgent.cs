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
    }

    public void Setup(PlayerEquipment playerEquipment){
        mainHandContainer = transform.Find("mainHandContainer").gameObject;
        offHandContainer = transform.Find("offHandContainer").gameObject;
        rangedContainer = transform.Find("rangedContainer").gameObject;

        mainHandAnimator = mainHandContainer.GetComponent<Animator>();
        offHandAnimator = offHandContainer.GetComponent<Animator>();

        rAnimator = rangedContainer.GetComponent<Animator>();
        
        if(ES3.KeyExists("meleeWeaponsSheathed")){
            meleeSheathed = ES3.Load<bool>("meleeWeaponsSheathed");
        }

        mainHandAnimator.SetBool("Sheathed", meleeSheathed);
        offHandAnimator.SetBool("Sheathed", meleeSheathed);

        UpdateWeaponInstances(playerEquipment);
    }

    void OnApplicationQuit(){
        // For some reason, this saves the opposite of the weapon's actual state, hence the '!'
        ES3.Save("meleeWeaponsSheathed", !meleeSheathed);
    }
    
    public void M_SheatheUnsheathe(){
        meleeSheathed = !meleeSheathed;
        PlayerEquipment playerEquipment = FindObjectOfType<PlayerEquipment>();
        // Animate sheathing/unsheathing
        mainHandAnimator.SetBool("Sheathed", meleeSheathed);
        if(playerEquipment.weaponL){
            offHandAnimator.SetBool("Sheathed", meleeSheathed);
        }

        UpdateWeaponInstances(playerEquipment);
    }

    public void UpdateWeaponInstances(PlayerEquipment playerEquipment){
        mainHandDamageTrigger = mainHandContainer.GetComponentInChildren<DamageTrigger>();
        offHandDamageTrigger = offHandContainer.GetComponentInChildren<DamageTrigger>();

        if(mainHandDamageTrigger){

            mainHandDamage = playerEquipment.weaponR.meleeWeaponScriptableObject.damage;
            mainHandCooldownTime = playerEquipment.weaponR.meleeWeaponScriptableObject.cooldownTime;
            mainHandPowerAttackDamageMultiplier = playerEquipment.weaponR.meleeWeaponScriptableObject.powerAttackDamageMultiplier;
            mainHandPowerAttackCooldownMultiplier = playerEquipment.weaponR.meleeWeaponScriptableObject.powerAttackCooldownMultiplier;

            mainHandDamageTrigger.active = false;
            mainHandDamageTrigger.damageAmount = mainHandDamage;
        }

        if(offHandDamageTrigger){

            offHandDamage = playerEquipment.weaponL.meleeWeaponScriptableObject.damage;
            offHandCooldownTime = playerEquipment.weaponL.meleeWeaponScriptableObject.cooldownTime;
            offHandPowerAttackDamageMultiplier = playerEquipment.weaponL.meleeWeaponScriptableObject.powerAttackDamageMultiplier;
            offHandPowerAttackCooldownMultiplier = playerEquipment.weaponL.meleeWeaponScriptableObject.powerAttackCooldownMultiplier;
            
            offHandDamageTrigger.active = false;
            offHandDamageTrigger.damageAmount = offHandDamage;
        }
    }

    public void M_MainAttack(){
        // Animate melee strike
        if(!meleeSheathed && Time.time >= mainHandRechargedTime && !Pause.PauseManagement.paused){
            mainHandDamageTrigger.damageAmount = mainHandDamage;

            mainHandAnimator.SetTrigger("Attack");
            mainHandAnimator.SetBool("Attacking", true);

            mainHandRechargedTime = Time.time + mainHandCooldownTime;
            offHandRechargedTime = Time.time + mainHandCooldownTime;

            // Activate damage trigger
            StartCoroutine(SetDamageTrigger(mainHandAnimator, mainHandDamageTrigger));
        }

    }

    public void M_OffAttack(){
        if(!meleeSheathed && Time.time >= offHandRechargedTime && !Pause.PauseManagement.paused){
            offHandDamageTrigger.damageAmount = offHandDamage;

            offHandAnimator.SetTrigger("Attack");
            offHandAnimator.SetBool("Attacking", true);

            offHandRechargedTime = Time.time + offHandCooldownTime;
            mainHandRechargedTime = Time.time + offHandCooldownTime;

            // Activate damage trigger
            StartCoroutine(SetDamageTrigger(offHandAnimator, offHandDamageTrigger));
        }
    }

    public void M_MainPowerAtack(){
        // Animate melee strike
        if(!meleeSheathed && Time.time >= mainHandRechargedTime && !Pause.PauseManagement.paused){
            mainHandDamageTrigger.damageAmount = Mathf.RoundToInt(mainHandDamage * mainHandPowerAttackDamageMultiplier);

            mainHandAnimator.SetTrigger("Power Attack");
            mainHandAnimator.SetBool("Attacking", true);

            mainHandRechargedTime = Time.time + mainHandCooldownTime * mainHandPowerAttackCooldownMultiplier;
            offHandRechargedTime = Time.time + mainHandCooldownTime * mainHandPowerAttackCooldownMultiplier;

            // Activate damage trigger
            StartCoroutine(SetDamageTrigger(mainHandAnimator, mainHandDamageTrigger));
        }
    }

    public void M_OffPowerAtack(){
        // Animate melee strike
        if(!meleeSheathed && Time.time >= offHandRechargedTime && !Pause.PauseManagement.paused){
            offHandDamageTrigger.damageAmount = Mathf.RoundToInt(offHandDamage * offHandPowerAttackDamageMultiplier);

            offHandAnimator.SetTrigger("Power Attack");
            offHandAnimator.SetBool("Attacking", true);

            offHandRechargedTime = Time.time + offHandCooldownTime * offHandPowerAttackCooldownMultiplier;
            mainHandRechargedTime = Time.time + offHandCooldownTime * offHandPowerAttackCooldownMultiplier;

            // Activate damage trigger
            StartCoroutine(SetDamageTrigger(offHandAnimator, offHandDamageTrigger));
        }
    }

    IEnumerator SetDamageTrigger(Animator anim, DamageTrigger dt){
        dt.active = true;
        yield return new WaitUntil(() => anim.GetBool("Attacking") == false);
        anim.SetBool("On Cooldown", true);
        dt.active = false;
        StartCoroutine(ManageCooldown(anim));
    }

    IEnumerator ManageCooldown(Animator anim){
        if(anim == mainHandAnimator){
            yield return new WaitUntil(() => Time.time >= mainHandRechargedTime);
        }
        else{
            yield return new WaitUntil(() => Time.time >= offHandRechargedTime);
        }

        anim.SetBool("On Cooldown", false);
    }




    // void Update(){
        
        
    //     // if(attackingLastFrame != mainHandAnimator.GetBool("Attacking")){
            
    //         // mainHandDamageTrigger.active = mainHandAnimator.GetBool("Attacking");
    //         // attackingLastFrame = mainHandAnimator.GetBool("Attacking");
    //     // }
    // }
}
