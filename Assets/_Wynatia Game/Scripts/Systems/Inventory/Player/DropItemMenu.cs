using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DropItemMenu : MonoBehaviour
{
    public Slider dropQuantitySlider;
    public TextMeshProUGUI quantityText;

    public void Setup(int itemQuantity){
        if(itemQuantity > 1){
            dropQuantitySlider.gameObject.SetActive(true);
            quantityText.gameObject.SetActive(true);
            dropQuantitySlider.maxValue = itemQuantity;
            dropQuantitySlider.value = 1;
            SetText(dropQuantitySlider.value);
        }
        else{
            dropQuantitySlider.maxValue = itemQuantity;
            dropQuantitySlider.value = 1;
            dropQuantitySlider.gameObject.SetActive(false);
            quantityText.gameObject.SetActive(false);
        }
        
    }

    public void SetText(float num){
        quantityText.SetText(Mathf.RoundToInt(num).ToString());
    }
}
