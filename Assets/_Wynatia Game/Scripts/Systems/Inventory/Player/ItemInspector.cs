using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ItemInspector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler

{
    public InputAction rotate = new InputAction("rotate", binding: "<Mouse>/delta");
    public InputAction zoom = new InputAction("zoom", binding: "<Mouse>/scroll");
    public Transform itemSpawnPoint;
    public float rotateSpeed = 1;
    public float zoomSpeed = 1;

    public float maxZoom = 2;
    public float minZoom = 0;

    private float startingZ = 3;
    private Vector3 startingEulers = new Vector3(-25, 25, 0);

    void Awake(){
        rotate.performed += ctx => RotateObject(rotate.ReadValue<Vector2>());
        zoom.performed += ctx => ZoomObject(zoom.ReadValue<Vector2>());

        startingZ = itemSpawnPoint.localPosition.z;
        startingEulers = itemSpawnPoint.localEulerAngles;
    }

    public void SetupItemInspector(GameObject g){
        foreach (Transform child in itemSpawnPoint)
        {
            Destroy(child.gameObject);
        }
        itemSpawnPoint.localPosition = new Vector3(0, 0, startingZ);
        itemSpawnPoint.localEulerAngles = startingEulers;
        GameObject itemModel = Instantiate(g, itemSpawnPoint);
        itemModel.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void OnPointerDown(PointerEventData eventData){
        rotate.Enable();
    }

    public void OnPointerUp(PointerEventData eventData){
        rotate.Disable();
    }

    public void OnPointerEnter(PointerEventData eventData){
        zoom.Enable();
    }

    public void OnPointerExit(PointerEventData eventData){
        zoom.Disable();
    }

    void RotateObject(Vector2 delta){
        delta *= rotateSpeed;
        itemSpawnPoint.Rotate(delta.y, -delta.x, 0, Space.World);
    }

    void ZoomObject(Vector2 input){
        float zoomAmount = -input.y * 0.01f * zoomSpeed;
        float projectedZ = itemSpawnPoint.localPosition.z + zoomAmount;

        if(projectedZ < startingZ - maxZoom){
            itemSpawnPoint.localPosition = new Vector3(0, 0, startingZ - maxZoom);
        }
        else if(projectedZ > startingZ){
            itemSpawnPoint.localPosition = new Vector3(0, 0, startingZ);
        }
        else{
            itemSpawnPoint.Translate(new Vector3(0, 0, zoomAmount), Space.World);
        }
        
    }
}
