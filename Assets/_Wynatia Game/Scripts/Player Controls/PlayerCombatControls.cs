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
        playerInput.actions["Melee Attack"].performed += _ => MeleeAttack();
        playerInput.actions["Melee Power Attack"].performed += _ => MeleePowerAttack();

    }

    void SheatheUnsheathe(){
        combatAgent.M_SheatheUnsheathe();
    }

    void MeleeAttack(){
        combatAgent.M_Attack();
    }

    void MeleePowerAttack(){
        combatAgent.M_PowerAtack();
    }
}
