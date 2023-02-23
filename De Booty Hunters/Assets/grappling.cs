using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovementAdvanced pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool isgrappling;
    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponent<PlayerMovementAdvanced>();

    }
    private void LateUpdate()
    {
        if (isgrappling) lr.SetPosition(0, gunTip.position);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(grappleKey)) startgrapple();
        if (grapplingCdTimer > 0) grapplingCdTimer -= Time.deltaTime;
    }
    void startgrapple()
    {
        if (grapplingCdTimer > 0) return;
        isgrappling = true;
        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(execute_grapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(stop_grappling), grappleDelayTime);
        }
        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
        
    }
    void execute_grapple()
    {

    }
    void stop_grappling()
    {
        isgrappling = false;
        grapplingCdTimer = grapplingCd;
        lr.enabled = false;
    }
}
