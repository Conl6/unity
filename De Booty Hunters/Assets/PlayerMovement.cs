using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //movement
    public float movementSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;
    //keybinding
    public KeyCode jumpKey = KeyCode.Space;
    //ground check
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;


    public Transform Orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    public Rigidbody rb;
   
    void Start()
    {
        rb= GetComponent<Rigidbody>();
        rb.freezeRotation= true;

        readyToJump = true;
    }

    void Update()
    {
        //Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        speedControl();

        //handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
        }
        else
        { 
            rb.drag = 0;
        }
    }
    private void FixedUpdate()
    {
        Move();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(resetJump), jumpCooldown);
        }
    }

    private void Move()
    {
        // Movement Direction
        moveDirection = Orientation.forward * verticalInput + Orientation.right * horizontalInput;
        //on ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed, ForceMode.Force);
        }
        //in air
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * airMultiplier, ForceMode.Force);
        }
    }
    
    private void speedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f,rb.velocity.z);
        if(flatVel.magnitude > movementSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    private void Jump()
    {
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void resetJump()
    {
        readyToJump = true;
    }

}
