using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SewerAudioTrigger : MonoBehaviour
{
    public AudioSource SewerNoise;

    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            SewerNoise.Play();
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            SewerNoise.Stop();
        }
    }
}
