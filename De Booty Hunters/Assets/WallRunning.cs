using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    //WallRunning
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    public float wallRunForce;
    public float wallClimbSpeed;
    public float maxWallRunTime;
    private float wallRunTime;

    //input
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;
    private bool upwardsRunning;
    private bool downwardsRunning;
    private float horizontalInput;
    private float verticalInput;

    //detection
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    //refrence
    public Transform orientation;
    private Rigidbody rb;
    private PlayerMovement pm;

    private void Start()
    {
       rb = GetComponent<Rigidbody>();
       pm = GetComponent<PlayerMovement>();

    }
    private void Update()
    {
        wallCheck();
        State();
    }

    private void FixedUpdate()
    {
        if(pm.wallRunning)
        {
            wallRunningMovement();
        }
    }

    private void wallCheck()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);

        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool aboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void State()
    {
        //Input Detection
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);

        // State 1 - wallRunngin
        if (wallLeft || wallRight && verticalInput > 0 && aboveGround())
        {
            if (!pm.wallRunning)
            {
                startWallRun();
            }
        }
        // state 3 - none
        else
        {
            if (pm.wallRunning)
                stopWallRun();
        }
        

    }

    private void startWallRun()
    {
        pm.wallRunning = true;
    }

    private void wallRunningMovement()
    {
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.y, 0f, rb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForard = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForard).magnitude > (orientation.forward - -wallForard).magnitude)
        {
            wallForard = -wallForard;
        }

        rb.AddForce(wallForard * wallRunForce, ForceMode.Force);

        // upwards/downwards force
        if (upwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        if (downwardsRunning)
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);

        if (!(wallLeft && horizontalInput > 0) || !(wallRight && horizontalInput< 0))
        rb.AddForce(-wallNormal * 10, ForceMode.Force);
        rb.AddForce(-wallNormal * 10, ForceMode.Force);
    }

    private void stopWallRun()
    {
        pm.wallRunning = false;
    }
}
