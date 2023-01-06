using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControler : MonoBehaviour
{

    public float xSens;
    public float ySens;

    public Transform orientation;

    float xRotation;
    float yRotation;


    // Start is called before the first frame update
    void Start()
    {
        //Locks cursor to the center on the screen
        Cursor.lockState = CursorLockMode.Locked;
        //Hides cursor
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSens;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySens;

        yRotation += mouseX;
        xRotation -= mouseY;

        //adds rotation limit to x axis
        xRotation = Math.Clamp(xRotation, -90f, 90f);

        //roation cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}

