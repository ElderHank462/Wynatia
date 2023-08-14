using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropItemMenu : MonoBehaviour
{
    public Slider dropQuantitySlider;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI confirmationText;

    public void Setup(int itemQuantity, string itemName){
        confirmationText.SetText("Are you sure you want to drop " + itemName + "?");

        if(itemQuantity > 1){
            dropQuantitySlider.gameObject.SetActive(true);
            quantityText.gameObject.SetActive(true);
            dropQuantitySlider.maxValue = itemQuantity;
            dropQuantitySlider.value = 1;
            SetQuantityText(dropQuantitySlider.value);
        }
        else{
            dropQuantitySlider.maxValue = itemQuantity;
            dropQuantitySlider.value = 1;
            dropQuantitySlider.gameObject.SetActive(false);
            quantityText.gameObject.SetActive(false);
        }
        
    }

    public void SetQuantityText(float num){
        quantityText.SetText(Mathf.RoundToInt(num).ToString());
    }
}
