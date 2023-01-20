using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dashing : MonoBehaviour
{
    public float dashforce;
    public float dashdurabillity;
    public float dashupforce;
    Transform orientation;

    public float dashcd;
    public float cdtimer;
    PlayerMovementAdvanced pm;

    public KeyCode daskkey = KeyCode.Q;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovementAdvanced>();
    }
    void dash()
    {
        pm.dashing = true;
        Vector3 applyforce = orientation.forward * dashforce + orientation.up * dashupforce;
        delaydash = applyforce;
        Invoke(nameof(delaydash), 0.025f);
        

        Invoke(nameof(resetdash), dashdurabillity);
    }
    Vector3 delaydash;
    private void resetdash()
    {
        pm.dashing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(daskkey))
        {
            dash();
        }
    }
}
