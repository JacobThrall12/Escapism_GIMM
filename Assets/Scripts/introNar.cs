using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class introNar : MonoBehaviour
{
    public AudioClip moo;
    // Start is called before the first frame update
    void Start()
    {
         GetComponent<AudioSource>().playOnAwake = false;
         GetComponent<AudioSource>().clip = moo;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
           GetComponent<AudioSource>().Play();
            Debug.Log("Entered");
        }
    }
}
