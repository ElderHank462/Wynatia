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

        playerInput.actions["sheathe"].performed += _ => SheatheUnsheathe();
        playerInput.actions["meleeAttack"].performed += _ => MeleeAttack();
        playerInput.actions["meleePowerAttack"].performed += _ => MeleePowerAttack();

        combatAgent = Camera.main.transform.Find("CombatAgent").GetComponent<PlayerCombatAgent>();
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
