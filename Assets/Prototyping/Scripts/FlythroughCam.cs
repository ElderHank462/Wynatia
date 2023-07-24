using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlythroughCam : MonoBehaviour
{
    public InputAction movement;
    public InputAction look;

    public float moveSpeed = 5;
    public float rotateSpeed = 25;


    Vector3 movementVector = Vector3.zero;
    Vector3 rotationVector = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        movement.Enable();
        look.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        //MouseLook
        var l = look.ReadValue<Vector2>();

        rotationVector += new Vector3(-l.y * rotateSpeed, l.x * rotateSpeed, 0);
        transform.eulerAngles = rotationVector;


        //Movement
        var m = movement.ReadValue<Vector3>().normalized;

        // movementVector = new Vector3(-m.x * moveSpeed, 0, m.y * moveSpeed);
        
        // movementVector.x *= transform.forward.x;
        // movementVector.y *= transform.forward.y;
        // movementVector.z *= transform.forward.z;
        // Debug.Log("transform.forward: " + transform.forward);
        movementVector = Vector3.zero;
        movementVector += transform.forward * m.z * moveSpeed;
        movementVector += transform.right * m.x * moveSpeed;
        movementVector += transform.up * m.y * moveSpeed;
        
        transform.Translate(movementVector * Time.deltaTime, Space.World);

    }
}
