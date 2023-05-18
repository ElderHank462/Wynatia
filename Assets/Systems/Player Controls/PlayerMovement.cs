using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Transform tr;
    Transform cam;

    Vector3 moveInput;
    Vector3 rot;
    Vector3 rotEulers = Vector3.zero;
    //Walk speed is in meters per second
    public float walkSpeed = 1f;
    public float rotSpeed = 1f;
    public float mouseSmoothingSpeed = 1f;

    public int lookClampUp = -90;
    public int lookClampDown = 90;
    
    void Start(){
        tr = GetComponent<Transform>();
        cam = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update(){
        
        Vector3 moveVector = Quaternion.Euler(0, tr.eulerAngles.y, 0) * moveInput;
        tr.position += moveVector * walkSpeed * Time.deltaTime;


        //Old method
        // tr.Rotate(new Vector3(0, rot.y * rotSpeed * Time.deltaTime, 0));

        //New method (apply smoothing later)
        Quaternion newRotation = Quaternion.Euler(0, rotEulers.y + rot.y * rotSpeed * Time.deltaTime, 0);
        tr.rotation = Quaternion.Slerp(tr.rotation, newRotation, mouseSmoothingSpeed * Time.deltaTime);

        
        float camRotate = rot.x * rotSpeed * Time.deltaTime;
        newRotation = Quaternion.Euler(Mathf.Clamp(-(rotEulers.x - camRotate), lookClampUp, lookClampDown), 0, 0);

        //Smoothly rotate to new rotation
        cam.localRotation = Quaternion.Slerp(cam.localRotation, newRotation, mouseSmoothingSpeed * Time.deltaTime);

        if(rotEulers.x > (lookClampDown + 1)){
            rotEulers.x = lookClampDown + 1;
        }
        else if(rotEulers.x < (lookClampUp - 1)){
            rotEulers.x = lookClampUp - 1;
        }







    }

    public void OnMovement(InputValue value){
        var v = value.Get<Vector2>();
        
        moveInput = new Vector3(v.x, 0, v.y).normalized;
    }

    public void OnLook(InputValue value){
        var v = value.Get<Vector2>();

        rot = new Vector3(v.y, v.x, 0);
        rotEulers += rot * rotSpeed * Time.deltaTime;
        // Debug.Log("camEulers: " + camEulers);
    }

}
