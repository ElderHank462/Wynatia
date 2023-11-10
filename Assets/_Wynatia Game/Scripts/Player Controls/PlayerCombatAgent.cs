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

    [SerializeField] float ammunitionAppliedForce = 10f;

    void Start(){
        playerInput = FindObjectOfType<PlayerInput>();

        playerInput.actions["Sheathe Weapon"].performed += _ => SheatheUnsheathe();
        playerInput.actions["Attack"].performed += _ => M_MainAttack();
        playerInput.actions["Off-Hand Attack"].performed += _ => M_OffAttack();
        playerInput.actions["Melee Power Attack"].performed += _ => M_MainPowerAtack();
        playerInput.actions["Melee Off-Hand Power Attack"].performed += _ => M_OffPowerAtack();
        playerInput.actions["Ranged Attack"].started += _ => RangedWeaponFunction(ReadyRangedWeapon);
        playerInput.actions["Ranged Attack"].performed += _ => RangedWeaponFunction(RangedAttack);
        playerInput.actions["Ranged Attack"].canceled += _ => RangedWeaponFunction(CancelRangedAttack);
        playerInput.actions["Cancel Ranged Attack"].performed += _ => RangedWeaponFunction(CancelRangedAttack);
    
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
        if(playerEquipment.weaponR){
            if(playerEquipment.weaponR.meleeWeaponScriptableObject){
                weaponSheathed = mainHandAnimator.GetBool("Sheathed");
            }
            else if(playerEquipment.weaponR.rangedWeaponScriptableObject){
                weaponSheathed = rangedAnimator.GetBool("Sheathed");
            }
        }
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
        
        mainHandDamageTrigger = mainHandContainer.GetComponentInChildren<DamageTrigger>();
        offHandDamageTrigger = offHandContainer.GetComponentInChildren<DamageTrigger>();
        // ammunitionDamageTrigger = ammunitionContainer.GetComponentInChildren<DamageTrigger>();

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

    public void RepairCombatAgentAfterMenuClose(){
        UpdateAnimators();
        CancelRangedAttack();
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

                // This didn't work :/
                // if(ammunitionContainer.transform.childCount != 0)
                //     rangedAnimator.SetTrigger("Cancel");
            }
        }
        // Fists
        else{
            SetAnimatorsSheathedParameterAndInputAction(weaponSheathed, weaponSheathed, true);
            ammunitionDisplay.gameObject.SetActive(false);
        }

        if(ammunitionDisplay.gameObject.activeSelf){
            // Also display count
            if(playerEquipment.ammunition){
                ammunitionDisplay.SetText(playerEquipment.ammunition.itemName + " ("+ FindObjectOfType<PlayerInventory>().ReturnItemCount(playerEquipment.ammunition) + ")");
            }
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
        playerInput.actions["Cancel Ranged Attack"].Disable();

        playerInput.actions["Sheathe Weapon"].Enable();

        if(!m){
            playerInput.actions["Attack"].Enable();
            playerInput.actions["Melee Power Attack"].Enable();
            playerInput.actions["Sheathe Weapon"].Enable();
        }
        if(!o){
            playerInput.actions["Off-Hand Attack"].Enable();
            playerInput.actions["Melee Off-Hand Power Attack"].Enable();
        }
        if(!r){
            playerInput.actions["Ranged Attack"].Enable();
        }

        Debug.Log("rangedAttack interactions: " + FindObjectOfType<PlayerInput>().actions["Ranged Attack"].bindings[0].effectiveInteractions);

        // Debug.Log("Input actions statuses");
        // Debug.Log("Attack: " + playerInput.actions["Attack"].enabled);
        // Debug.Log("Melee Power Attack: " + playerInput.actions["Melee Power Attack"].enabled);
        // Debug.Log("Off-Hand Attack: " + playerInput.actions["Off-Hand Attack"].enabled);
        // Debug.Log("Melee Off-Hand Power Attack: " + playerInput.actions["Melee Off-Hand Power Attack"].enabled);
        // Debug.Log("Ranged Attack: " + playerInput.actions["Ranged Attack"].enabled);
    }

    void RangedWeaponFunction(System.Action function){
        // If ready to fire
        // Call function
        if(ReadyToFire()){

            function();
        }
    }
    bool ReadyToFire(){
        if(!Pause.PauseManagement.paused && !weaponSheathed && Time.time >= rangedRechargedTime){
            return true;
        }
        return false;
    }

    void ReadyRangedWeapon(){
        if(playerEquipment.ammunition && ammunitionContainer.transform.childCount == 0){
            
            // Debug.Log("Readying ranged weapon; ammunitionContainer childCount: " + ammunitionContainer.transform.childCount);

            playerInput.actions["Cancel Ranged Attack"].Enable();
            playerInput.actions["Sheathe Weapon"].Disable();

            rangedAnimator.speed = readyRangedWeaponAnimationClip.length / playerEquipment.weaponR.rangedWeaponScriptableObject.drawTime;
            rangedAnimator.SetBool("Fire", false);
            rangedAnimator.SetBool("Cancel", false);
            rangedAnimator.SetBool("Ready", true);

            // Instantiate arrow in ammunitionContainer
            // Disable model collider, etc.
            playerEquipment.InstantiateWeapon(playerEquipment.ammunition.worldObject, ammunitionContainer.transform);
        }
        else{
            Debug.Log("You must equip some ammunition before you can attack with this weapon");
        }
        
    }

    void CancelRangedAttack(){
        if(ammunitionContainer.transform.childCount != 0){
            Debug.Log("Canceled ranged attack");
            rangedAnimator.speed = 1;
            // rangedAnimator.SetTrigger("Cancel");
            rangedAnimator.SetBool("Cancel", true);

            playerInput.actions["Cancel Ranged Attack"].Disable();
            playerInput.actions["Sheathe Weapon"].Enable();

            // Empty ammunitionContainer
            foreach(Transform child in ammunitionContainer.transform){
                Destroy(child.gameObject);
            }
        }
    }

    void RangedAttack(){
        if(ammunitionContainer.transform.childCount != 0){
            Debug.Log("Ranged attack! interactions: " + FindObjectOfType<PlayerInput>().actions["Ranged Attack"].bindings[0].effectiveInteractions);
            rangedAnimator.speed = 1;
            // rangedAnimator.SetTrigger("Fire");
            rangedAnimator.SetBool("Fire", true);

            playerInput.actions["Cancel Ranged Attack"].Disable();
            playerInput.actions["Sheathe Weapon"].Enable();

            // Unparent ammunition from container
            GameObject ammunition = ammunitionContainer.transform.GetChild(0).gameObject;
            ammunitionContainer.transform.GetChild(0).SetParent(null);

            // Shoot it!
            List<Collider> cols = new List<Collider>();
            cols.Add(playerEquipment.GetComponent<Collider>());
            cols.AddRange(playerEquipment.GetComponentsInChildren<Collider>());
            ammunition.GetComponent<Projectile>().Setup(cols, rangedDamage + playerEquipment.ammunition.ammunitionScriptableObject.damage);
            ammunition.GetComponent<Rigidbody>().isKinematic = false;

            Vector3 target;
            RaycastHit hit;
            
            // Point projecile towards where player is looking
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 500)){
                target = hit.point;
            }
            else{
                target = Camera.main.transform.position + (Camera.main.transform.forward * 500);
            }

            ammunition.transform.LookAt(target);
            ammunition.GetComponent<Rigidbody>().AddForce(ammunition.transform.forward * ammunitionAppliedForce, ForceMode.Impulse);

            // Remove one arrow from inventory
            FindObjectOfType<PlayerInventory>().DecrementItemCount(playerEquipment.ammunition);
            if(FindObjectOfType<PlayerInventory>().ReturnItemCount(playerEquipment.ammunition) <= 0){
                playerEquipment.UnequipSlot(ref playerEquipment.ammunition);
            }

            if(playerEquipment.ammunition){
                ammunitionDisplay.SetText(playerEquipment.ammunition.itemName + " ("+ FindObjectOfType<PlayerInventory>().ReturnItemCount(playerEquipment.ammunition) + ")");
            }
            else
                ammunitionDisplay.SetText("No ammunition equipped");
            
            reticleController.SetReticle((int)ReticleController.Reticle.X);
            rangedRechargedTime = Time.time + rangedCooldownTime;
            StartCoroutine(ManageCooldown(rangedAnimator));
        }
    }

    public void M_MainAttack(){
        
        if(!mainHandDamageTrigger){
            UpdateCombatAgentVariables();
        }
        
        // Animate melee strike
        if(!weaponSheathed && Time.time >= mainHandRechargedTime && !Pause.PauseManagement.paused && playerEquipment.weaponR){
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
        dt.active = false;
        StartCoroutine(ManageCooldown(anim));
    }

    IEnumerator ManageCooldown(Animator anim){
        anim.SetBool("On Cooldown", true);
        if(anim == mainHandAnimator){
            yield return new WaitUntil(() => Time.time >= mainHandRechargedTime);
        }
        else if(anim == offHandAnimator){
            yield return new WaitUntil(() => Time.time >= offHandRechargedTime);
        }
        else if(anim == rangedAnimator){
            yield return new WaitUntil(() => Time.time >= rangedRechargedTime);
        }

        reticleController.SetReticle((int)ReticleController.Reticle.Dot);
        anim.SetBool("On Cooldown", false);
    }
}
