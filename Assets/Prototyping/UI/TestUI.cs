using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TestUI : MonoBehaviour
{
    void OnEnable(){
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        Button button = root.Q<Button>("Button");

        button.clicked += () => Debug.Log("hello world");
    }
}
