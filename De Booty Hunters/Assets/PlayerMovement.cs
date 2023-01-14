using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //movement
    private float movementSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallRunSpeed;

    private float desiredMovementSpeed;
    private float lastDesiredMovemetSpeed;

    public float groundDrag;
    
    //jumping
    public int jumps;
    public int maxJumps;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    //crouching
    public float crouchSpeed;
    public float crouchHeight;
    private float startHeight;


    //keybinding
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    
    //ground check
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    //slope handler
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

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
        crouching,
        sliding,
        wallRunning,
        air,
    }

    public bool sliding;
    public bool wallRunning;

    void Start()
    {
        rb= GetComponent<Rigidbody>();
        rb.freezeRotation= true;

        readyToJump = true;

        startHeight = transform.localScale.y;
    }

    void Update()
    {
        //Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        speedControl();
        stateHandler();
        //simple double jump
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
        //Disable gravity whilst on a slope
        rb.useGravity = !OnSlope();
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
        // start crouch
        if (Input.GetKey(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
              
            rb.AddForce(Vector3.down * 0.1f, ForceMode.Impulse);
            
        }
        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startHeight, transform.localScale.z);

        }
    }

    private void stateHandler()
    {
        // mode - wallRunning
        if (wallRunning)
        {
            state = MovementStat.wallRunning;
            desiredMovementSpeed = wallRunSpeed;

        }
        if (sliding)
        {
            state = MovementStat.sliding;

            if(OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMovementSpeed = slideSpeed;
            }
            else
            {
                desiredMovementSpeed = sprintSpeed;
            }
        }
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementStat.sprinting;
            desiredMovementSpeed = sprintSpeed;
        } else if (grounded)
        {
            state = MovementStat.walking;
            desiredMovementSpeed = walkSpeed;
        }
        else if (grounded && Input.GetKey(crouchKey))
        {
            state = MovementStat.crouching;
            desiredMovementSpeed = crouchSpeed;
        }
        else
        {
            state = MovementStat.air;
        }
        if (Mathf.Abs(desiredMovementSpeed - lastDesiredMovemetSpeed) > 4f && movementSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            movementSpeed = desiredMovementSpeed;
        }
        lastDesiredMovemetSpeed = desiredMovementSpeed;
    }
    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float diffrence = Mathf.Abs(desiredMovementSpeed - movementSpeed);
        float startValue = movementSpeed;
        while (time < diffrence)
        {
            movementSpeed = Mathf.Lerp(startValue, desiredMovementSpeed, time / diffrence);
            time += Time.deltaTime;
            yield return null;
        }
        movementSpeed = desiredMovementSpeed;
    }
    private void Move()
    {
        // Movement Direction
        moveDirection = Orientation.forward * verticalInput + Orientation.right * horizontalInput;
        // on slope
        if(OnSlope())
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * movementSpeed * 10f, ForceMode.Force);
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        //on ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * 5f, ForceMode.Force);
        }
        //in air
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * movementSpeed * 5f * airMultiplier, ForceMode.Force);
        }
    }  
    
    private void speedControl()
    {
        //limit speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > movementSpeed)
            {
                rb.velocity = rb.velocity.normalized * movementSpeed;
            }
        
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                //limit on ground & air
            if (flatVel.magnitude > movementSpeed)
            { 
                    Vector3 limitedVel = flatVel.normalized * movementSpeed;
                    rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }
    }
    private void Jump()
    {
        exitingSlope = true;
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        jumps++;
        
        exitingSlope = false;
    }
    private void resetJump()
    {
        readyToJump = true;
    }

    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
