using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //movement
    private float movementSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    public int jumps;
    public int maxJumps;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    //keybinding
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    
    //ground check
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;


    public Transform Orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    private Rigidbody rb;

    public MovementStat state;
    public enum MovementStat
    {
        walking,
        sprinting,
        air,
    }
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
        stateHandler();

        if (jumps == maxJumps)
        {
            readyToJump = false;
        }

        //handle drag & double jump
        if (grounded)
        {
            rb.drag = groundDrag;
            jumps = 0;
            readyToJump = true;
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
        if (Input.GetKeyDown(jumpKey) && readyToJump)
        {
            Jump();

            Invoke(nameof(resetJump), jumpCooldown);
        }
    }

    private void stateHandler()
    {
        if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementStat.sprinting;
            movementSpeed = sprintSpeed;
        } else if (grounded)
        {
            state = MovementStat.walking;
            movementSpeed = walkSpeed;
        }
        else
        {
            state = MovementStat.air;
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
        jumps++;
    }
    private void resetJump()
    {
        readyToJump = true;
    }

}
