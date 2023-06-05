using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CyclethroughMovement : MonoBehaviour
{
    public InputAction movement;
    public InputAction nextObject;
    public InputAction previousObject;

    public Transform[] thingsToMove;

    public float moveSpeed = 5;
    public float rotateSpeed = 1;


    Vector3 movementVector = Vector3.zero;
    Vector3 rotationVector = Vector3.zero;

    Transform activeTransform;
    int currentTransform = 0;
    
    // Start is called before the first frame update
    void OnEnable()
    {
        movement.Enable();
        nextObject.Enable();
        previousObject.Enable();

        nextObject.performed += _ => IncreaseIndex();
        previousObject.performed += _ => DecreaseIndex();
        nextObject.performed += _ => SetActiveTransform();
        previousObject.performed += _ => SetActiveTransform();

        activeTransform = thingsToMove[0];
    }

    void OnDisable(){
        movement.Disable();
        nextObject.Disable();
        previousObject.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        
        
        //Movement
        var m = movement.ReadValue<Vector3>();
        


        movementVector = Vector3.zero;
        movementVector += activeTransform.forward * m.normalized.z * moveSpeed;
        movementVector += activeTransform.up * m.normalized.y * moveSpeed;

        activeTransform.Rotate(new Vector3(0, m.x * rotateSpeed * Time.deltaTime, 0));
        
        activeTransform.Translate(movementVector * Time.deltaTime, Space.World);

    }

    void SetActiveTransform()
    {
        activeTransform = thingsToMove[currentTransform];
    }

    void IncreaseIndex(){
        if(currentTransform < (thingsToMove.Length - 1)){
            currentTransform++;
        }
        else{
            currentTransform = 0;
        }
    }

    void DecreaseIndex(){
        if(currentTransform > 0){
            currentTransform--;
        }
        else{
            currentTransform = (thingsToMove.Length - 1);
        }
    }
}
