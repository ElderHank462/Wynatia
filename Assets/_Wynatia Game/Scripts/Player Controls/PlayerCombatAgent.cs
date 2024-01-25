using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
// Documentation: https://github.com/ElderHank462/Wynatia/wiki/PlayerCombatAgent
public class PlayerCombatAgent : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    public GameObject rightHandContainer;
    public GameObject leftHandContainer;
    public GameObject bothHandsContainer;
    public GameObject rangedContainer;
    public GameObject ammunitionContainer;
    Animator rightHandAnimator;
    Animator leftHandAnimator;
    Animator bothHandsAnimator;
    Animator rangedAnimator;
    public AnimationClip readyRangedWeaponAnimationClip;

    DamageTrigger mainDamageTrigger;
    DamageTrigger altDamageTrigger;
    DamageTrigger bothHandsDamageTrigger;

    // public MeleeWeapon meleeWeapon;

    public bool weaponSheathed = true;

    public int mainDamage;
    public int altDamage;
    public int rangedDamage;

    public float mainPowerAttackDamageMultiplier;
    public float altPowerAttackDamageMultiplier;

    public float mainCooldownTime;
    public float altCooldownTime;
    public float rangedCooldownTime;

    public float mainPowerAttackCooldownMultiplier;
    public float altPowerAttackCooldownMultiplier;

    [SerializeField] private TextMeshProUGUI ammunitionDisplay;

    float mainRechargedTime = 0;
    float altRechargedTime = 0;
    float rangedRechargedTime = 0;


    private PlayerEquipment playerEquipment;
    private PlayerCharacter playerCharacter;
    ReticleController reticleController;

    [SerializeField] float ammunitionAppliedForce = 7f;

    void Start(){

        AssignReferences();

        playerInput.actions["Sheathe Weapon"].performed += _ => SheatheUnsheathe();
        playerInput.actions["Attack"].performed += _ => M_MainAttack();
        playerInput.actions["Off-Hand Attack"].performed += _ => M_OffAttack();
        playerInput.actions["Melee Power Attack"].performed += _ => M_MainPowerAtack();
        playerInput.actions["Melee Off-Hand Power Attack"].performed += _ => M_OffPowerAtack();

        // playerInput.actions["Ranged Attack"].started += _ => Debug.Log("ranged attack: started");
        // playerInput.actions["Ranged Attack"].performed += _ => Debug.Log("ranged attack: performed");
        // playerInput.actions["Ranged Attack"].canceled += _ => Debug.Log("ranged attack: canceled");
        // playerInput.actions["Cancel Ranged Attack"].performed += _ => Debug.Log("cancel ranged attack: performed");
        playerInput.actions["Ranged Attack"].started += _ => RangedWeaponFunction(ReadyRangedWeapon);
        playerInput.actions["Ranged Attack"].performed += _ => RangedWeaponFunction(RangedAttack);
        playerInput.actions["Ranged Attack"].canceled += _ => RangedWeaponFunction(CancelRangedAttack);
        playerInput.actions["Cancel Ranged Attack"].performed += _ => RangedWeaponFunction(CancelRangedAttack);


    }

    void AssignReferences(){
        playerInput = FindObjectOfType<PlayerInput>();
        playerEquipment = FindObjectOfType<PlayerEquipment>();
        playerCharacter = FindObjectOfType<PlayerCharacter>();
        reticleController = FindObjectOfType<ReticleController>();

        rightHandAnimator = rightHandContainer.GetComponent<Animator>();
        leftHandAnimator = leftHandContainer.GetComponent<Animator>();
        bothHandsAnimator = bothHandsContainer.GetComponent<Animator>();
        rangedAnimator = rangedContainer.GetComponent<Animator>();
    }



    public void Setup(){

        AssignReferences();
        
        if(ES3.KeyExists("meleeWeaponsSheathed")){
            if(!ES3.KeyExists("overwriteSaveData")){
                weaponSheathed = ES3.Load<bool>("meleeWeaponsSheathed");
            }
        }

        UpdateCombatAgentVariables();
        UpdateAnimators();
    }

    void OnApplicationQuit(){
        // For some reason, this saves the opposite of the weapon's actual state, hence the '!'
        ES3.Save("meleeWeaponsSheathed", !weaponSheathed);
        ES3.DeleteKey("overwriteSaveData");
    }
    
    public void SheatheUnsheathe(){
        weaponSheathed = !weaponSheathed;

        UpdateCombatAgentVariables();
        UpdateAnimators();
    }

    public void UpdateCombatAgentVariables(){
        mainDamageTrigger = null;
        altDamageTrigger = null;
        
        // mainDamageTrigger = rightHandContainer.GetComponentInChildren<DamageTrigger>();
        // altDamageTrigger = leftHandContainer.GetComponentInChildren<DamageTrigger>();
        

        AssignReferences();

#region Assigning DamageTriggers
        if(playerEquipment.rightHand){
            if(playerEquipment.rightHand.meleeWeaponScriptableObject){
                mainDamageTrigger = rightHandContainer.GetComponentInChildren<DamageTrigger>();
                if(mainDamageTrigger)
                    mainDamageTrigger.active = false;
            }

            if(playerEquipment.leftHand){
                if(playerEquipment.leftHand.meleeWeaponScriptableObject){
                    altDamageTrigger = leftHandContainer.GetComponentInChildren<DamageTrigger>();
                    if(altDamageTrigger)
                        altDamageTrigger.active = false;
                }
            }
        }
        else if(playerEquipment.bothHands){
            if(playerEquipment.bothHands.meleeWeaponScriptableObject){
                mainDamageTrigger = bothHandsContainer.GetComponentInChildren<DamageTrigger>();
                mainDamageTrigger.active = false;
            }
        }
    #endregion
#region Assigning Variables
        if(playerEquipment.rightHand){
            if(mainDamageTrigger && playerEquipment.rightHand.meleeWeaponScriptableObject){
                mainDamage = playerEquipment.rightHand.meleeWeaponScriptableObject.baseDamage;
                mainCooldownTime = playerEquipment.rightHand.meleeWeaponScriptableObject.cooldownTime;
                mainPowerAttackDamageMultiplier = playerEquipment.rightHand.meleeWeaponScriptableObject.powerAttackDamageMultiplier;
                mainPowerAttackCooldownMultiplier = playerEquipment.rightHand.meleeWeaponScriptableObject.powerAttackCooldownMultiplier;

                // mainDamageTrigger.active = false;
                mainDamageTrigger.damageAmount = mainDamage;
            }
            else if(playerEquipment.rightHand.rangedWeaponScriptableObject){
                rangedDamage = playerEquipment.rightHand.rangedWeaponScriptableObject.baseDamage;
                rangedCooldownTime = playerEquipment.rightHand.rangedWeaponScriptableObject.cooldownTime;

                float drawTime = playerEquipment.rightHand.rangedWeaponScriptableObject.drawTime;

            }

        }
        else if(playerEquipment.bothHands){
            if(mainDamageTrigger && playerEquipment.bothHands.meleeWeaponScriptableObject){
                mainDamage = playerEquipment.bothHands.meleeWeaponScriptableObject.baseDamage;
                mainCooldownTime = playerEquipment.bothHands.meleeWeaponScriptableObject.cooldownTime;
                mainPowerAttackDamageMultiplier = playerEquipment.bothHands.meleeWeaponScriptableObject.powerAttackDamageMultiplier;
                mainPowerAttackCooldownMultiplier = playerEquipment.bothHands.meleeWeaponScriptableObject.powerAttackCooldownMultiplier;

                // mainDamageTrigger.active = false;
                mainDamageTrigger.damageAmount = mainDamage;
            }
            else if(playerEquipment.bothHands.rangedWeaponScriptableObject){
                rangedDamage = playerEquipment.bothHands.rangedWeaponScriptableObject.baseDamage;
                rangedCooldownTime = playerEquipment.bothHands.rangedWeaponScriptableObject.cooldownTime;

                float drawTime = playerEquipment.bothHands.rangedWeaponScriptableObject.drawTime;

            }
        }

        if(altDamageTrigger && playerEquipment.leftHand){

            altDamage = playerEquipment.leftHand.meleeWeaponScriptableObject.baseDamage;
            altCooldownTime = playerEquipment.leftHand.meleeWeaponScriptableObject.cooldownTime;
            altPowerAttackDamageMultiplier = playerEquipment.leftHand.meleeWeaponScriptableObject.powerAttackDamageMultiplier;
            altPowerAttackCooldownMultiplier = playerEquipment.leftHand.meleeWeaponScriptableObject.powerAttackCooldownMultiplier;
            
            // altDamageTrigger.active = false;
            altDamageTrigger.damageAmount = altDamage;
        }

    #endregion

    }

// TODO finish implementing two handed weapons with PCA
    public void RepairCombatAgentAfterMenuClose(){
        UpdateAnimators();
        CancelRangedAttack();
    }

    public void UpdateAnimators(){
        Item emptyHand = playerCharacter.unarmedStrike;

        if(!playerEquipment.bothHands){
            // Single one handed weapon in right hand
            if(playerEquipment.leftHand == emptyHand && playerEquipment.rightHand != emptyHand){
                // Debug.Log(weaponSheathed);
                SetAnimatorsSheathedParameterAndInputAction(weaponSheathed, true, true, true, true);
            }
            // Dual wielding
            else if(playerEquipment.leftHand != emptyHand && playerEquipment.leftHand.type == Item.ItemType.Weapon){
                SetAnimatorsSheathedParameterAndInputAction(weaponSheathed, weaponSheathed, true, true, true);
            }
            // Shield in left hand, one handed weapon in right hand
            else if(playerEquipment.leftHand.type == Item.ItemType.Shield){
                SetAnimatorsSheathedParameterAndInputAction(weaponSheathed, weaponSheathed, weaponSheathed, true, true);
            }
            // Empty handed
            else if(playerEquipment.leftHand == emptyHand && playerEquipment.rightHand == emptyHand){

                SetAnimatorsSheathedParameterAndInputAction(weaponSheathed, weaponSheathed, true, true, true);
            }
        }
        else{
            // Two handed melee weapon
            if(playerEquipment.bothHands.meleeWeaponScriptableObject){
                SetAnimatorsSheathedParameterAndInputAction(true, true, true, weaponSheathed, true);
            }
            // Ranged weapon
            else if(playerEquipment.bothHands.rangedWeaponScriptableObject){
                SetAnimatorsSheathedParameterAndInputAction(true, true, true, true, weaponSheathed);
            }
        }
        
        #region old code
        // // Dual wielding
        // if(playerEquipment.rightHand && playerEquipment.leftHand != FindObjectOfType<PlayerCharacter>().unarmedStrike){
        //     SetAnimatorsSheathedParameterAndInputAction(weaponSheathed, weaponSheathed, true, true);
        //     ammunitionDisplay.gameObject.SetActive(false);
        // }
        // // Single one handed weapon being wielded in the main hand
        // else if(playerEquipment.rightHand && playerEquipment.leftHand == FindObjectOfType<PlayerCharacter>().unarmedStrike){
        //     SetAnimatorsSheathedParameterAndInputAction(weaponSheathed, true, true, true);
        //     ammunitionDisplay.gameObject.SetActive(false);
        // }
        // else if(playerEquipment.rightHand && playerEquipment.rightHand.twoHanded){
        //     // Two handed melee weapon
        //     if(playerEquipment.rightHand.meleeWeaponScriptableObject){
        //         SetAnimatorsSheathedParameterAndInputAction(true, true, false, true);
        //         ammunitionDisplay.gameObject.SetActive(false);
        //     }
        //     // Ranged weapon
        //     else{
        //         SetAnimatorsSheathedParameterAndInputAction(true, true, true, weaponSheathed);
        //         Debug.Log("Time: " + Time.time + "; preparing to set ammunitionDisplay visibility to: " + !weaponSheathed);
        //         ammunitionDisplay.gameObject.SetActive(!weaponSheathed);
        //         Debug.Log("Time: " + Time.time + "; ammunitionDisplay visibility: " + !weaponSheathed);

        //     }
        // }
        // // Fists
        // else{
        //     SetAnimatorsSheathedParameterAndInputAction(weaponSheathed, weaponSheathed, true, true);
        //     ammunitionDisplay.gameObject.SetActive(false);
        // }
#endregion
        UpdateAmmunitionDisplay();
    }

    public void UpdateAmmunitionDisplay(){
        if(playerEquipment.bothHands){
            if(playerEquipment.bothHands.rangedWeaponScriptableObject){
                if(weaponSheathed){
                    ammunitionDisplay.gameObject.SetActive(false);
                }
                else{
                    // Also display count
                    if(playerEquipment.ammunition){
                        ammunitionDisplay.SetText(playerEquipment.ammunition.itemName + " ("+ FindObjectOfType<PlayerInventory>().ReturnItemCount(playerEquipment.ammunition) + ")");
                    }
                    else{
                        ammunitionDisplay.SetText("No ammunition equipped");
                    }
            
                    ammunitionDisplay.gameObject.SetActive(true);
                }
            }
        }
        else{
            ammunitionDisplay.gameObject.SetActive(false);
        }

    }
// Shield boolean may prove unnecessary, it currently isn't referenced anywhere
// May be able to consolidate the functionality of the twohanded boolean into the right hand bool as well
// since all the processing is going to be done in the attack functions
    void SetAnimatorsSheathedParameterAndInputAction(bool m, bool o, bool s, bool t, bool r){
        rightHandAnimator.SetBool("Sheathed", m);
        leftHandAnimator.SetBool("Sheathed", o);
        bothHandsAnimator.SetBool("Sheathed", t);
        rangedAnimator.SetBool("Sheathed", r);

        playerInput = FindObjectOfType<PlayerInput>();

        playerInput.actions["Attack"].Disable();
        playerInput.actions["Melee Power Attack"].Disable();
        playerInput.actions["Off-Hand Attack"].Disable();
        playerInput.actions["Melee Off-Hand Power Attack"].Disable();
        playerInput.actions["Ranged Attack"].Disable();
        playerInput.actions["Cancel Ranged Attack"].Disable();

        playerInput.actions["Sheathe Weapon"].Enable();

        if(!m || !t){
            playerInput.actions["Attack"].Enable();
            playerInput.actions["Melee Power Attack"].Enable();
        }
        if(!o){
            // TODO have the off hand attack functions check to see whether left hand is holding shield and do either weapon
            // or shield actions accordingly
            playerInput.actions["Off-Hand Attack"].Enable();
            playerInput.actions["Melee Off-Hand Power Attack"].Enable();
        }
        if(!r){
            playerInput.actions["Ranged Attack"].Enable();
        }
#region Debugging messages
        // Debug.Log("rangedAttack interactions: " + FindObjectOfType<PlayerInput>().actions["Ranged Attack"].bindings[0].effectiveInteractions);

        // Debug.Log("Input actions statuses");
        // Debug.Log("Attack: " + playerInput.actions["Attack"].enabled);
        // Debug.Log("Melee Power Attack: " + playerInput.actions["Melee Power Attack"].enabled);
        // Debug.Log("Off-Hand Attack: " + playerInput.actions["Off-Hand Attack"].enabled);
        // Debug.Log("Melee Off-Hand Power Attack: " + playerInput.actions["Melee Off-Hand Power Attack"].enabled);
        // Debug.Log("sheathe status: " + playerInput.actions["Sheathe Weapon"].enabled);
    #endregion
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
        // Added ammunition count check because occasionally (not reproduceably) the player could use ammo infinitely
        // and the ammunition would run into the negatives
        // Don't know if this fixes it because I can't reproduce the issue, but if it comes up again then I guess this fix doesn't cut it
        if(playerEquipment.ammunition && ammunitionContainer.transform.childCount == 0 && FindObjectOfType<PlayerInventory>().ReturnItemCount(playerEquipment.ammunition) > 0){
            

            playerInput.actions["Cancel Ranged Attack"].Enable();
            playerInput.actions["Sheathe Weapon"].Disable();

            rangedAnimator.speed = readyRangedWeaponAnimationClip.length / playerEquipment.bothHands.rangedWeaponScriptableObject.drawTime;
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
            // Debug.Log("Canceled ranged attack");
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
// FIXME ranged attacks when reloading game cause arrow to instantly spawn with colliders enabled and pickup disabled
// maybe input actions are being called out of order because some are enabled when they shouldn't be?
    void RangedAttack(){
        if(ammunitionContainer.transform.childCount != 0){
            // Debug.Log("Ranged attack! interactions: " + FindObjectOfType<PlayerInput>().actions["Ranged Attack"].bindings[0].effectiveInteractions);
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
            FindObjectOfType<PlayerInventory>().DecrementItemCount(playerEquipment.ammunition, true);
            if(FindObjectOfType<PlayerInventory>().ReturnItemCount(playerEquipment.ammunition) <= 0){
                // Debug.Log("a");
                AssignReferences();
                playerEquipment.UnequipSlot(ref playerEquipment.ammunition);
                // Debug.Log("d");
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
        
        if(!mainDamageTrigger){
            UpdateCombatAgentVariables();
        }

        Animator anim;

        if(playerEquipment.rightHand){
            anim = rightHandAnimator;
        }
        else if(playerEquipment.bothHands){
            anim = bothHandsAnimator;
        }
        else{
            return;
        }
        
        // Animate melee strike
        if(!weaponSheathed && Time.time >= mainRechargedTime && !Pause.PauseManagement.paused){
            
            mainDamageTrigger.damageAmount = mainDamage;

            anim.SetTrigger("Attack");
            anim.SetBool("Attacking", true);

            mainRechargedTime = Time.time + mainCooldownTime;
            altRechargedTime = Time.time + mainCooldownTime;

            reticleController.SetReticle((int)ReticleController.Reticle.X);

            // Activate damage trigger
            StartCoroutine(SetDamageTrigger(anim, mainDamageTrigger));

        }

    }

    public void M_OffAttack(){
        if(!altDamageTrigger){
            UpdateCombatAgentVariables();
        }



        // This is checking for both main and off hand weapons to be present in preparation for the future introduction of hand to hand combat 
        // where if there isn't a weapon in either hand, the off hand attack will still go through
        if(!weaponSheathed && Time.time >= altRechargedTime && !Pause.PauseManagement.paused && playerEquipment.leftHand && playerEquipment.rightHand){
            altDamageTrigger.damageAmount = altDamage;

            leftHandAnimator.SetTrigger("Attack");
            leftHandAnimator.SetBool("Attacking", true);

            altRechargedTime = Time.time + altCooldownTime;
            mainRechargedTime = Time.time + altCooldownTime;

            reticleController.SetReticle((int)ReticleController.Reticle.X);

            // Activate damage trigger
            StartCoroutine(SetDamageTrigger(leftHandAnimator, altDamageTrigger));
        }
    }

    public void M_MainPowerAtack(){
        if(!mainDamageTrigger){
            UpdateCombatAgentVariables();
        }

        Animator anim;

        if(playerEquipment.rightHand){
            anim = rightHandAnimator;
        }
        else if(playerEquipment.bothHands){
            anim = bothHandsAnimator;
        }
        else{
            return;
        }

        // Animate melee strike
        if(!weaponSheathed && Time.time >= mainRechargedTime && !Pause.PauseManagement.paused){
            mainDamageTrigger.damageAmount = Mathf.RoundToInt(mainDamage * mainPowerAttackDamageMultiplier);

            anim.SetTrigger("Power Attack");
            anim.SetBool("Attacking", true);

            mainRechargedTime = Time.time + mainCooldownTime * mainPowerAttackCooldownMultiplier;
            altRechargedTime = Time.time + mainCooldownTime * mainPowerAttackCooldownMultiplier;

            reticleController.SetReticle((int)ReticleController.Reticle.X);
            // Activate damage trigger
            StartCoroutine(SetDamageTrigger(anim, mainDamageTrigger));
        }
    }

    public void M_OffPowerAtack(){
        if(!altDamageTrigger){
            UpdateCombatAgentVariables();
        }

        // Animate melee strike
        if(!weaponSheathed && Time.time >= altRechargedTime && !Pause.PauseManagement.paused && playerEquipment.leftHand && playerEquipment.rightHand){
            altDamageTrigger.damageAmount = Mathf.RoundToInt(altDamage * altPowerAttackDamageMultiplier);

            leftHandAnimator.SetTrigger("Power Attack");
            leftHandAnimator.SetBool("Attacking", true);

            altRechargedTime = Time.time + altCooldownTime * altPowerAttackCooldownMultiplier;
            mainRechargedTime = Time.time + altCooldownTime * altPowerAttackCooldownMultiplier;

            reticleController.SetReticle((int)ReticleController.Reticle.X);
            // Activate damage trigger
            StartCoroutine(SetDamageTrigger(leftHandAnimator, altDamageTrigger));
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
        if(anim == rightHandAnimator || anim == bothHandsAnimator){
            yield return new WaitUntil(() => Time.time >= mainRechargedTime);
        }
        else if(anim == leftHandAnimator){
            yield return new WaitUntil(() => Time.time >= altRechargedTime);
        }
        else if(anim == rangedAnimator){
            yield return new WaitUntil(() => Time.time >= rangedRechargedTime);
        }

        reticleController.SetReticle((int)ReticleController.Reticle.Dot);
        anim.SetBool("On Cooldown", false);
    }
}
