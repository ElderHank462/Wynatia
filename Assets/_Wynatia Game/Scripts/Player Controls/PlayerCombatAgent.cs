using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerCombatAgent : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    
    public GameObject mainHandContainer;
    public GameObject offHandContainer;
    public GameObject rangedContainer;
    public GameObject ammunitionContainer;
    Animator mainHandAnimator;
    Animator offHandAnimator;
    Animator rangedAnimator;
    public AnimationClip readyRangedWeaponAnimationClip;

    DamageTrigger mainHandDamageTrigger;
    DamageTrigger offHandDamageTrigger;
    DamageTrigger ammunitionDamageTrigger;

    [Header("This gameObject must be named 'CombatAgent'. Container gameObjects must be named \n 'mainHandContainer,' 'offHandContainer,' and 'rangedContainer'.")]
    // public MeleeWeapon meleeWeapon;

    public bool weaponSheathed = true;

    public int mainHandDamage;
    public int offHandDamage;
    public int rangedDamage;

    public float mainHandPowerAttackDamageMultiplier;
    public float offHandPowerAttackDamageMultiplier;

    public float mainHandCooldownTime;
    public float offHandCooldownTime;
    public float rangedCooldownTime;

    public float mainHandPowerAttackCooldownMultiplier;
    public float offHandPowerAttackCooldownMultiplier;

    [SerializeField] private TextMeshProUGUI ammunitionDisplay;

    float mainHandRechargedTime = 0;
    float offHandRechargedTime = 0;
    float rangedRechargedTime = 0;

    bool attackingLastFrame = false;

    private PlayerEquipment playerEquipment;
    ReticleController reticleController;

    void Start(){
        playerInput = FindObjectOfType<PlayerInput>();

        playerInput.actions["Sheathe Weapon"].performed += _ => SheatheUnsheathe();
        playerInput.actions["Attack"].performed += _ => M_MainAttack();
        playerInput.actions["Off-Hand Attack"].performed += _ => M_OffAttack();
        playerInput.actions["Melee Power Attack"].performed += _ => M_MainPowerAtack();
        playerInput.actions["Melee Off-Hand Power Attack"].performed += _ => M_OffPowerAtack();
        playerInput.actions["Ranged Attack"].started += _ => ReadyRangedWeapon();
        playerInput.actions["Ranged Attack"].performed += _ => RangedAttack();
        playerInput.actions["Ranged Attack"].canceled += _ => CancelRangedAttack();
    
        mainHandContainer = transform.Find("mainHandContainer").gameObject;
        offHandContainer = transform.Find("offHandContainer").gameObject;
        rangedContainer = transform.Find("rangedContainer").gameObject;

        mainHandAnimator = mainHandContainer.GetComponent<Animator>();
        offHandAnimator = offHandContainer.GetComponent<Animator>();

        rangedAnimator = rangedContainer.GetComponent<Animator>();

        playerEquipment = FindObjectOfType<PlayerEquipment>();
        reticleController = FindObjectOfType<ReticleController>();
    }

    void DetermineAttackType(){
        if(playerEquipment.weaponR){
            if(playerEquipment.weaponR.meleeWeaponScriptableObject){
                M_MainAttack();
            }
            else{
                RangedAttack();
            }
        }
    }


    public void Setup(){
        mainHandContainer = transform.Find("mainHandContainer").gameObject;
        offHandContainer = transform.Find("offHandContainer").gameObject;
        rangedContainer = transform.Find("rangedContainer").gameObject;
        // ammunitionContainer = transform.Find("ammunitionContainer").gameObject;

        mainHandAnimator = mainHandContainer.GetComponent<Animator>();
        offHandAnimator = offHandContainer.GetComponent<Animator>();

        rangedAnimator = rangedContainer.GetComponent<Animator>();
        
        if(ES3.KeyExists("meleeWeaponsSheathed")){
            weaponSheathed = ES3.Load<bool>("meleeWeaponsSheathed");
        }

        mainHandAnimator.SetBool("Sheathed", weaponSheathed);
        offHandAnimator.SetBool("Sheathed", weaponSheathed);

        UpdateCombatAgentVariables();
        UpdateAnimators();
    }

    void OnApplicationQuit(){
        // For some reason, this saves the opposite of the weapon's actual state, hence the '!'
        ES3.Save("meleeWeaponsSheathed", !weaponSheathed);
    }
    
    public void SheatheUnsheathe(){
        weaponSheathed = !weaponSheathed;

        UpdateCombatAgentVariables();
        UpdateAnimators();
    }

    public void UpdateCombatAgentVariables(){
        mainHandDamageTrigger = null;
        offHandDamageTrigger = null;
        ammunitionDamageTrigger = null;
        
        mainHandDamageTrigger = mainHandContainer.GetComponentInChildren<DamageTrigger>();
        offHandDamageTrigger = offHandContainer.GetComponentInChildren<DamageTrigger>();
        ammunitionDamageTrigger = ammunitionContainer.GetComponentInChildren<DamageTrigger>();

        playerEquipment = FindObjectOfType<PlayerEquipment>();

        playerInput = FindObjectOfType<PlayerInput>();

        if(playerEquipment.weaponR){
            if(mainHandDamageTrigger && playerEquipment.weaponR.meleeWeaponScriptableObject){
                mainHandDamage = playerEquipment.weaponR.meleeWeaponScriptableObject.baseDamage;
                mainHandCooldownTime = playerEquipment.weaponR.meleeWeaponScriptableObject.cooldownTime;
                mainHandPowerAttackDamageMultiplier = playerEquipment.weaponR.meleeWeaponScriptableObject.powerAttackDamageMultiplier;
                mainHandPowerAttackCooldownMultiplier = playerEquipment.weaponR.meleeWeaponScriptableObject.powerAttackCooldownMultiplier;

                mainHandDamageTrigger.active = false;
                mainHandDamageTrigger.damageAmount = mainHandDamage;
            }
            else if(playerEquipment.weaponR.rangedWeaponScriptableObject){
                rangedDamage = playerEquipment.weaponR.rangedWeaponScriptableObject.baseDamage;
                rangedCooldownTime = playerEquipment.weaponR.rangedWeaponScriptableObject.cooldownTime;

                // rangedDamageTrigger.active = false;
                // rangedDamageTrigger.damageAmount = rangedDamage;

                float drawTime = playerEquipment.weaponR.rangedWeaponScriptableObject.drawTime;
            }

        }

        if(offHandDamageTrigger && playerEquipment.weaponL){

            offHandDamage = playerEquipment.weaponL.meleeWeaponScriptableObject.baseDamage;
            offHandCooldownTime = playerEquipment.weaponL.meleeWeaponScriptableObject.cooldownTime;
            offHandPowerAttackDamageMultiplier = playerEquipment.weaponL.meleeWeaponScriptableObject.powerAttackDamageMultiplier;
            offHandPowerAttackCooldownMultiplier = playerEquipment.weaponL.meleeWeaponScriptableObject.powerAttackCooldownMultiplier;
            
            offHandDamageTrigger.active = false;
            offHandDamageTrigger.damageAmount = offHandDamage;
        }

        mainHandAnimator = mainHandContainer.GetComponent<Animator>();
        offHandAnimator = offHandContainer.GetComponent<Animator>();
        rangedAnimator = rangedContainer.GetComponent<Animator>();

    }

    public void UpdateAnimators(){
        // One handed weapon in main hand
        if(playerEquipment.weaponR && !playerEquipment.weaponR.twoHanded && !playerEquipment.weaponL){
            SetAnimatorsSheathedParameterAndInputAction(weaponSheathed, weaponSheathed, true);
            ammunitionDisplay.gameObject.SetActive(false);
        }
        // Dual wielding
        else if(playerEquipment.weaponR && playerEquipment.weaponL){
            SetAnimatorsSheathedParameterAndInputAction(weaponSheathed, weaponSheathed, true);
            ammunitionDisplay.gameObject.SetActive(false);
        }
        else if(playerEquipment.weaponR && playerEquipment.weaponR.twoHanded){
            // Two handed melee weapon
            if(playerEquipment.weaponR.meleeWeaponScriptableObject){
                SetAnimatorsSheathedParameterAndInputAction(weaponSheathed, true, true);
                ammunitionDisplay.gameObject.SetActive(false);
            }
            // Ranged weapon
            else{
                SetAnimatorsSheathedParameterAndInputAction(true, true, weaponSheathed);
                ammunitionDisplay.gameObject.SetActive(!weaponSheathed);
            }
        }
        // Fists
        else{
            SetAnimatorsSheathedParameterAndInputAction(weaponSheathed, weaponSheathed, true);
            ammunitionDisplay.gameObject.SetActive(false);
        }

        if(ammunitionDisplay.gameObject.activeSelf){
            // Also display count
            if(playerEquipment.ammunition)
                ammunitionDisplay.SetText(playerEquipment.ammunition.itemName);
            else
                ammunitionDisplay.SetText("No ammunition equipped");
        }
    }

    void SetAnimatorsSheathedParameterAndInputAction(bool m, bool o, bool r){
        mainHandAnimator.SetBool("Sheathed", m);
        offHandAnimator.SetBool("Sheathed", o);
        rangedAnimator.SetBool("Sheathed", r);

        playerInput = FindObjectOfType<PlayerInput>();

        playerInput.actions["Attack"].Disable();
        playerInput.actions["Melee Power Attack"].Disable();
        playerInput.actions["Off-Hand Attack"].Disable();
        playerInput.actions["Melee Off-Hand Power Attack"].Disable();
        playerInput.actions["Ranged Attack"].Disable();

        if(!m){
            playerInput.actions["Attack"].Enable();
            playerInput.actions["Melee Power Attack"].Enable();
        }
        if(!o){
            playerInput.actions["Off-Hand Attack"].Enable();
            playerInput.actions["Melee Off-Hand Power Attack"].Enable();
        }
        if(!r){
            playerInput.actions["Ranged Attack"].Enable();
        }

        // Debug.Log("Input actions statuses");
        // Debug.Log("Attack: " + playerInput.actions["Attack"].enabled);
        // Debug.Log("Melee Power Attack: " + playerInput.actions["Melee Power Attack"].enabled);
        // Debug.Log("Off-Hand Attack: " + playerInput.actions["Off-Hand Attack"].enabled);
        // Debug.Log("Melee Off-Hand Power Attack: " + playerInput.actions["Melee Off-Hand Power Attack"].enabled);
        // Debug.Log("Ranged Attack: " + playerInput.actions["Ranged Attack"].enabled);
    }

    void ReadyRangedWeapon(){
        if(!Pause.PauseManagement.paused && !weaponSheathed){
             if(playerEquipment.ammunition){
                Debug.Log("Readying ranged weapon");
                 rangedAnimator.speed = readyRangedWeaponAnimationClip.length / playerEquipment.weaponR.rangedWeaponScriptableObject.drawTime;
                 rangedAnimator.SetTrigger("Ready");

                 // Instantiate arrow in ammunitionContainer
                 // Disable model collider, etc.
                 playerEquipment.InstantiateWeapon(playerEquipment.ammunition.worldObject, ammunitionContainer.transform);
             }
             else{
                 Debug.Log("You must equip some ammunition before you can attack with this weapon");
             }
        }
    }

    void CancelRangedAttack(){
        if(!Pause.PauseManagement.paused && playerEquipment.ammunition && !weaponSheathed){
            Debug.Log("Canceled ranged attack");
            rangedAnimator.speed = 1;
            rangedAnimator.SetTrigger("Cancel");

            // Empty ammunitionContainer
            foreach(Transform child in ammunitionContainer.transform){
                Destroy(child.gameObject);
            }
        }
    }

    void RangedAttack(){
        if(!Pause.PauseManagement.paused && playerEquipment.ammunition && !weaponSheathed){
            Debug.Log("Ranged attack!");
            rangedAnimator.speed = 1;
            rangedAnimator.SetTrigger("Fire");

            // Unparent ammunition from container
            GameObject ammunition = ammunitionContainer.transform.GetChild(0).gameObject;
            ammunitionContainer.transform.GetChild(0).SetParent(null);

            // Re-enable its colliders
            foreach(Transform child in ammunition.transform){
                if(child.GetComponent<Collider>())
                    child.GetComponent<Collider>().enabled = true;
            }

            // Shoot it!
            // * Add a script to ammunition so that when they hit an object, if it's not an enemy the layer will return to normal and the item can be picked up again
            ammunition.GetComponent<Rigidbody>().isKinematic = false;
            ammunition.GetComponent<Rigidbody>().AddForce(ammunition.transform.forward * 70, ForceMode.Impulse);

            // Remove one arrow from inventory
            
            
        }
    }

    public void M_MainAttack(){
        
        if(!mainHandDamageTrigger){
            UpdateCombatAgentVariables();
        }
        
        // Animate melee strike
        if(!weaponSheathed && Time.time >= mainHandRechargedTime && !Pause.PauseManagement.paused){
            mainHandDamageTrigger.damageAmount = mainHandDamage;

            mainHandAnimator.SetTrigger("Attack");
            mainHandAnimator.SetBool("Attacking", true);

            mainHandRechargedTime = Time.time + mainHandCooldownTime;
            offHandRechargedTime = Time.time + mainHandCooldownTime;

            reticleController.SetReticle((int)ReticleController.Reticle.X);

            // Activate damage trigger
            StartCoroutine(SetDamageTrigger(mainHandAnimator, mainHandDamageTrigger));
        }

    }

    public void M_OffAttack(){
        if(!offHandDamageTrigger){
            UpdateCombatAgentVariables();
        }

        // This is checking for both main and off hand weapons to be present in preparation for the future introduction of hand to hand combat 
        // where if there isn't a weapon in either hand, the off hand attack will still go through
        if(!weaponSheathed && Time.time >= offHandRechargedTime && !Pause.PauseManagement.paused && playerEquipment.weaponL && playerEquipment.weaponR){
            offHandDamageTrigger.damageAmount = offHandDamage;

            offHandAnimator.SetTrigger("Attack");
            offHandAnimator.SetBool("Attacking", true);

            offHandRechargedTime = Time.time + offHandCooldownTime;
            mainHandRechargedTime = Time.time + offHandCooldownTime;

            reticleController.SetReticle((int)ReticleController.Reticle.X);

            // Activate damage trigger
            StartCoroutine(SetDamageTrigger(offHandAnimator, offHandDamageTrigger));
        }
    }

    public void M_MainPowerAtack(){
        if(!mainHandDamageTrigger){
            UpdateCombatAgentVariables();
        }

        // Animate melee strike
        if(!weaponSheathed && Time.time >= mainHandRechargedTime && !Pause.PauseManagement.paused){
            mainHandDamageTrigger.damageAmount = Mathf.RoundToInt(mainHandDamage * mainHandPowerAttackDamageMultiplier);

            mainHandAnimator.SetTrigger("Power Attack");
            mainHandAnimator.SetBool("Attacking", true);

            mainHandRechargedTime = Time.time + mainHandCooldownTime * mainHandPowerAttackCooldownMultiplier;
            offHandRechargedTime = Time.time + mainHandCooldownTime * mainHandPowerAttackCooldownMultiplier;

            reticleController.SetReticle((int)ReticleController.Reticle.X);
            // Activate damage trigger
            StartCoroutine(SetDamageTrigger(mainHandAnimator, mainHandDamageTrigger));
        }
    }

    public void M_OffPowerAtack(){
        if(!offHandDamageTrigger){
            UpdateCombatAgentVariables();
        }

        // Animate melee strike
        if(!weaponSheathed && Time.time >= offHandRechargedTime && !Pause.PauseManagement.paused && playerEquipment.weaponL && playerEquipment.weaponR){
            offHandDamageTrigger.damageAmount = Mathf.RoundToInt(offHandDamage * offHandPowerAttackDamageMultiplier);

            offHandAnimator.SetTrigger("Power Attack");
            offHandAnimator.SetBool("Attacking", true);

            offHandRechargedTime = Time.time + offHandCooldownTime * offHandPowerAttackCooldownMultiplier;
            mainHandRechargedTime = Time.time + offHandCooldownTime * offHandPowerAttackCooldownMultiplier;

            reticleController.SetReticle((int)ReticleController.Reticle.X);
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

        reticleController.SetReticle((int)ReticleController.Reticle.Dot);
        anim.SetBool("On Cooldown", false);
    }
}
