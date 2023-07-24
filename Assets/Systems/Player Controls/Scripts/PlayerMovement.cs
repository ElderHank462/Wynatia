using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Transform tr;
    Transform cam;
    CharacterController charController;
    PlayerInput playerInput;
    public GameObject pauseMenu;

    Vector3 moveInput;
    Vector3 rot;
    Vector3 camEulers;
    Vector3 velocity = Vector3.zero;


    //Walk speed is in meters per second
    public float walkSpeed = 1f;
    public float runSpeed = 5f;
    public float sprintSpeed = 10f;
    public float jumpPower = 1f;
    public float jumpCooldown = 3f;
    bool jumpOnCooldown = false;
    bool triggerJump = false;
    public float gravity = -9.8f;
    public bool walking = false;

    public float rotSpeed = 45f;

    public int lookClampUp = -90;
    public int lookClampDown = 90;
    bool inMenu = false;
    
    void Start(){
        tr = GetComponent<Transform>();
        cam = Camera.main.transform;
        charController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        camEulers = cam.localEulerAngles;


        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerInput.actions["walkToggle"].started += context => walking = !walking;
        playerInput.actions["menu"].started += context => ChangeMenuState();
        playerInput.actions["close"].started += context => ChangeMenuState();
        playerInput.actions["jump"].started += context => Jump();
        
        if(!inMenu)
            Time.timeScale = 1;
    }

    void FixedUpdate(){
        if(Time.timeScale != 0){
            Vector3 moveVector = Quaternion.Euler(0, tr.eulerAngles.y, 0) * moveInput * Time.deltaTime;
            float speed;

            if(walking)
                speed = walkSpeed;
            else
                speed = runSpeed;
            
            if(playerInput.actions["sprint"].IsPressed())
                //Move the transform at sprint speed
                moveVector *= sprintSpeed;
            else
                //Move the transform at walk speed
                moveVector *= speed;

            charController.Move(moveVector);



            if(triggerJump){
                velocity.y = jumpPower * Time.deltaTime;
                triggerJump = false;
            }
            else{
                velocity.y += gravity * Time.deltaTime;
            }

            charController.Move(velocity);

            if(charController.isGrounded && velocity.y < 0){
                velocity.y = 0;
            }
        }

    }

    void LateUpdate(){
        // camEulers *= Time.deltaTime;
        
        if(camEulers.x >= lookClampDown){
            cam.localEulerAngles = camEulers;
            rot.x = 0;
        }
        if(camEulers.x <= lookClampUp){
            cam.localEulerAngles = camEulers;
            rot.x = 0;
        }
        
        cam.Rotate(new Vector3(rot.x, 0, 0) * Time.deltaTime, Space.Self);
        tr.Rotate(new Vector3(0, rot.y, 0) * Time.deltaTime, Space.Self);
    }

    void Jump(){
        if(velocity.y == 0 && !jumpOnCooldown && Time.timeScale != 0){
            // Jump
            triggerJump = true;
            jumpOnCooldown = true;

            StartCoroutine(JumpCooldown());
        }
    }

    IEnumerator JumpCooldown(){
        yield return new WaitForSeconds(jumpCooldown);
        jumpOnCooldown = false;
    }

    //Enables and disables the menu UI and corresponding action maps
    public void ChangeMenuState(){
        inMenu = !inMenu;

        if(inMenu){
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            playerInput.SwitchCurrentActionMap("menu");
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
            //PauseMenu housekeeping function that sets all rebind text
            GameObject.FindGameObjectWithTag("RebindManager").GetComponent<RebindManager>().UpdateAllRebindText();
        }
        else{
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerInput.SwitchCurrentActionMap("player_controls");
            pauseMenu.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void OnMovement(InputValue value){
        var v = value.Get<Vector2>();
        
        moveInput = new Vector3(v.x, 0, v.y).normalized;
    }

    public void OnLook(InputValue value){
        var v = value.Get<Vector2>();

        rot = new Vector3(-1 * v.y, v.x, 0) * rotSpeed;
        camEulers.x += rot.x * Time.deltaTime;
        camEulers.x = Mathf.Clamp(camEulers.x, lookClampUp, lookClampDown);
    }
}
