using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ItemCreatorWindow : EditorWindow
{
    [MenuItem("Editor Windows/Item Creator")]
    public static void ShowWindow(){
        GetWindow<ItemCreatorWindow>();
    }

    private void OnGUI(){
        GUILayout.Label("test label");
    }
}
