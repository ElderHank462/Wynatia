using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatAgent : MonoBehaviour
{
    GameObject meleeContainer;
    GameObject rangedContainer;
    Animator mAnimator;
    Animator rAnimator;

    DamageTrigger mDamageTrigger;

    [Header("This gameObject must be named 'CombatAgent'. Container gameObjects must be named 'meleeContainer' and 'rangedContainer'.")]
    public MeleeWeapon meleeWeapon;

    public bool meleeSheathed = true;

    public int meleeDamage;
    public float meleePowerAttackDamageMultiplier;
    public float meleeCooldownTime;
    public float meleePowerAttackCooldownMultiplier;
    float mRechargedTime = 0;
    bool attackingLastFrame = false;


    void Start(){
        meleeContainer = transform.Find("meleeContainer").gameObject;
        rangedContainer = transform.Find("rangedContainer").gameObject;

        mAnimator = meleeContainer.GetComponent<Animator>();
        rAnimator = rangedContainer.GetComponent<Animator>();

        mAnimator.SetBool("Sheathed", meleeSheathed);

        mDamageTrigger = meleeContainer.GetComponentInChildren<DamageTrigger>();

        meleeDamage = meleeWeapon.damage;
        meleeCooldownTime = meleeWeapon.cooldownTime;
        meleePowerAttackDamageMultiplier = meleeWeapon.powerAttackDamageMultiplier;
        meleePowerAttackCooldownMultiplier = meleeWeapon.powerAttackCooldownMultiplier;

        mDamageTrigger.active = false;
        mDamageTrigger.damageAmount = meleeDamage;
    }
    
    void Update(){
        if(attackingLastFrame != mAnimator.GetBool("Attacking")){

            mDamageTrigger.active = mAnimator.GetBool("Attacking");
            attackingLastFrame = mAnimator.GetBool("Attacking");
        }
    }
    
    public void M_SheatheUnsheathe(){
        meleeSheathed = !meleeSheathed;
        // Animate sheathing/unsheathing
        mAnimator.SetBool("Sheathed", meleeSheathed);
    }

    public void M_Attack(){
        // Animate melee strike
        if(!meleeSheathed && Time.time >= mRechargedTime){
            mDamageTrigger.damageAmount = meleeDamage;
            mAnimator.SetTrigger("Attack");
            mRechargedTime = Time.time + meleeCooldownTime;
        }
    }

    public void M_PowerAtack(){
        // Animate melee strike
        if(!meleeSheathed && Time.time >= mRechargedTime){
            mDamageTrigger.damageAmount = Mathf.RoundToInt(meleeDamage * meleePowerAttackDamageMultiplier);
            mAnimator.SetTrigger("Power Attack");
            mRechargedTime = Time.time + meleeCooldownTime * meleePowerAttackCooldownMultiplier;
        }
    }
}
