using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glider : MonoBehaviour
{
    [Header("Movement")]
    public float GlideForce;

    public float StartGlideSpeed;
    public float maxGlideSpeed;

    [Header("Keybindings")]
    public KeyCode glideKey = KeyCode.Q;

    private float horizontalInput;
    private float verticalInput;

    [Header("Gravity")]
    bool useGravity;
    public float GravityCounterForce;

    [Header("Refernece")]
    public Transform orientation;
    private PlayerMovementAdvanced pm;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovementAdvanced>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");


        if (Input.GetKeyDown(glideKey) && !pm.grounded)
        {
            StartGlide();
        }
       
        if (useGravity)
        {
            rb.AddForce(transform.up * GravityCounterForce, ForceMode.Force);
        }
    }
    private void StartGlide()
    {
        pm.gliding = true;

    //camera effects
    }

    private void GlideMovement()
    {
        rb.useGravity = useGravity;

        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(inputDirection.normalized * GlideForce, ForceMode.Force);

    }

    private void EndGlide()
    {
        pm.gliding = false;

    //remove camera effects
    }
}
