using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camPosition : MonoBehaviour
{
    //moves camHolder to empty object on player
    public Transform cameraPosition;
    // Update is called once per frame
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
