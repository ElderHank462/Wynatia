using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Transform tr;
    Transform cam;
    PlayerInput playerInput;
    public GameObject pauseMenu;

    Vector3 moveInput;
    Vector3 rot;
    Vector3 rotEulers = Vector3.zero;

    //Walk speed is in meters per second
    public float walkSpeed = 1f;
    public float runSpeed = 5f;
    public float sprintSpeed = 10f;
    public bool walking = false;
    bool inMenu = false;

    public float rotSpeed = 1f;
    public float mouseSmoothingSpeed = 1f;

    public int lookClampUp = -90;
    public int lookClampDown = 90;
    
    void Start(){
        tr = GetComponent<Transform>();
        cam = Camera.main.transform;
        playerInput = GetComponent<PlayerInput>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerInput.actions["walkToggle"].started += context => walking = !walking;
        playerInput.actions["menu"].started += context => ChangeMenuState();
        playerInput.actions["close"].started += context => ChangeMenuState();
    }

    void Update(){
        
        Vector3 moveVector = Quaternion.Euler(0, tr.eulerAngles.y, 0) * moveInput;
        float speed;

        if(walking)
            speed = walkSpeed * Time.deltaTime;
        else
            speed = runSpeed * Time.deltaTime;
        
        if(playerInput.actions["sprint"].IsPressed())
            //Move the transform at sprint speed
            tr.position += moveVector * sprintSpeed * Time.deltaTime;
        else
            //Move the transform at walk speed
            tr.position += moveVector * speed;

        // Rotate player
        Quaternion newRotation = Quaternion.Euler(0, rotEulers.y + rot.y * rotSpeed * Time.deltaTime, 0);
        tr.rotation = Quaternion.Slerp(tr.rotation, newRotation, mouseSmoothingSpeed * Time.deltaTime);
        
        float camRotate = rot.x * rotSpeed * Time.deltaTime;
        newRotation = Quaternion.Euler(Mathf.Clamp(-(rotEulers.x - camRotate), lookClampUp, lookClampDown), 0, 0);

        //Smoothly rotate to new rotation
        cam.localRotation = Quaternion.Slerp(cam.localRotation, newRotation, mouseSmoothingSpeed * Time.deltaTime);

        //Avoid rotEulers from going beyond the clamp values
        if(rotEulers.x > (lookClampDown + 1)){
            rotEulers.x = lookClampDown + 1;
        }
        else if(rotEulers.x < (lookClampUp - 1)){
            rotEulers.x = lookClampUp - 1;
        }
    }

    //Enables and disables the manu UI and corresponding action maps
    public void ChangeMenuState(){
        inMenu = !inMenu;

        if(inMenu){
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            playerInput.SwitchCurrentActionMap("menu");
            pauseMenu.SetActive(true);
            //PauseMenu housekeeping function that sets all rebind text
            GameObject.FindGameObjectWithTag("RebindManager").GetComponent<RebindManager>().UpdateAllRebindText();
        }
        else{
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerInput.SwitchCurrentActionMap("player_controls");
            pauseMenu.SetActive(false);
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
