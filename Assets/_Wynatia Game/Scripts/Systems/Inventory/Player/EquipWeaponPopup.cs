using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipWeaponPopup : MonoBehaviour
{
    // public GameObject threeOptionPanel;
    // public GameObject fourOptionPanel;

    // public Button threeA;
    // public Button threeB;

    // public Button fourA;
    // public Button fourB;
    // public Button fourC;

    // private PlayerEquipment pE;
    // private Item weaponToEquip;


    // public enum Layout{
    //     Equip_MainBoth,
    //     Equip_MainOff,
    //     Equip_Off__Replace_Main,
    //     Replace_MainOffBoth,
    //     Replace_MainOff
    // }

    // public void Setup(Layout layout, Item item){
    //     pE = FindObjectOfType<PlayerEquipment>();
    //     weaponToEquip = item;

    //     threeOptionPanel.SetActive(false);
    //     fourOptionPanel.SetActive(false);
        
    //     switch(layout){
    //         // case Layout.Equip_MainOff: SetupEquipMO(false);
    //         // break;

    //         case Layout.Equip_MainBoth: SetupEquipMB();
    //         break;
            
    //         case Layout.Replace_MainOff: SetupReplaceMO(false);
    //         break;
            
    //         case Layout.Replace_MainOffBoth: SetupReplaceMO(true);
    //         break;
            
    //         case Layout.Equip_Off__Replace_Main: SetupEquipOffReplaceMain();
    //         break;
    //     }
    // }

    // void SetupEquipMB(){
    //     threeA.onClick.RemoveAllListeners();
    //     threeB.onClick.RemoveAllListeners();

    //     threeA.onClick.AddListener(delegate {pE.EquipWeaponMainHand(weaponToEquip); ClosePopup();});
    //     threeB.onClick.AddListener(delegate {pE.EquipWeaponMainHand(weaponToEquip); pE.EquipWeaponOffHand(weaponToEquip); ClosePopup();});

    //     string textA = "Main Hand";
    //     string textB = "One to Each Hand";
    //     threeA.GetComponentInChildren<TextMeshProUGUI>().SetText(textA);
    //     threeB.GetComponentInChildren<TextMeshProUGUI>().SetText(textB);

    //     threeOptionPanel.SetActive(true);
        
    // }

    // void SetupReplaceMO(bool includeBoth){
    //     if(!includeBoth){
    //         threeA.onClick.RemoveAllListeners();
    //         threeB.onClick.RemoveAllListeners();

    //         threeA.onClick.AddListener(delegate {pE.UnequipWeaponMainHand(); pE.EquipWeaponMainHand(weaponToEquip); ClosePopup();});
    //         threeB.onClick.AddListener(delegate {pE.UnequipWeaponOffHand(); pE.EquipWeaponOffHand(weaponToEquip); ClosePopup();});
                
    //         string textA = "Main Hand (Replace)";
    //         string textB = "Off Hand (Replace)";
    //         threeA.GetComponentInChildren<TextMeshProUGUI>().SetText(textA);
    //         threeB.GetComponentInChildren<TextMeshProUGUI>().SetText(textB);

    //         threeOptionPanel.SetActive(true);
    //     }
    //     else{
    //         fourA.onClick.RemoveAllListeners();
    //         fourB.onClick.RemoveAllListeners();
    //         fourC.onClick.RemoveAllListeners();

    //         fourA.onClick.AddListener(delegate {pE.UnequipWeaponMainHand(); pE.EquipWeaponMainHand(weaponToEquip); ClosePopup();});
    //         fourB.onClick.AddListener(delegate {pE.UnequipWeaponOffHand(); pE.EquipWeaponOffHand(weaponToEquip); ClosePopup();});
    //         fourC.onClick.AddListener(delegate {pE.UnequipWeaponMainHand(); pE.UnequipWeaponOffHand(); 
    //             pE.EquipWeaponMainHand(weaponToEquip); pE.EquipWeaponOffHand(weaponToEquip); ClosePopup();});

    //         string textA = "Main Hand (Replace)";
    //         string textB = "Off Hand (Replace)";
    //         string textC = "One to Each Hand (Replace)";
    //         fourA.GetComponentInChildren<TextMeshProUGUI>().SetText(textA);
    //         fourB.GetComponentInChildren<TextMeshProUGUI>().SetText(textB);
    //         fourC.GetComponentInChildren<TextMeshProUGUI>().SetText(textC);

    //         fourOptionPanel.SetActive(true);
    //     }
    // }

    // void SetupEquipOffReplaceMain(){
    //     threeA.onClick.RemoveAllListeners();
    //     threeB.onClick.RemoveAllListeners();

    //     threeA.onClick.AddListener(delegate {pE.UnequipWeaponMainHand(); pE.EquipWeaponMainHand(weaponToEquip); ClosePopup();});
    //     threeB.onClick.AddListener(delegate {pE.EquipWeaponOffHand(weaponToEquip); ClosePopup();});

    //     string textA = "Main Hand (Replace)";
    //     string textB = "Off Hand";
    //     threeA.GetComponentInChildren<TextMeshProUGUI>().SetText(textA);
    //     threeB.GetComponentInChildren<TextMeshProUGUI>().SetText(textB);

    //     threeOptionPanel.SetActive(true);
    // }

    // void ClosePopup(){
    //     gameObject.SetActive(false);
    // }
}
