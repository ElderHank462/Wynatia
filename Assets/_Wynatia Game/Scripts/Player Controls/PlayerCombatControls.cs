using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatControls : MonoBehaviour
{
    PlayerInput playerInput;
    PlayerCombatAgent combatAgent;
    
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        combatAgent = GetComponentInChildren<PlayerCombatAgent>();

        playerInput.actions["Sheathe Weapon"].performed += _ => SheatheUnsheathe();
        playerInput.actions["Attack"].performed += _ => combatAgent.M_MainAttack();;
        playerInput.actions["Off-Hand Attack"].performed += _ => combatAgent.M_OffAttack();
        playerInput.actions["Melee Power Attack"].performed += _ => combatAgent.M_MainPowerAtack();
        playerInput.actions["Melee Off-Hand Power Attack"].performed += _ => combatAgent.M_OffPowerAtack();

    }

    void SheatheUnsheathe(){
        combatAgent.M_SheatheUnsheathe();
    }

    void MeleeAttack(){
        
    }

    void MeleePowerAttack(){
        
    }
}
