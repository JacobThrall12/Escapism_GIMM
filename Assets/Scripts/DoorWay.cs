using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorWay : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other){
        if
        (this.gameObject.tag == "Impel_Door" && other.gameObject.tag == "Player"){
            SceneManager.LoadScene("Imperu_ShrinkDemo");
            Debug.Log("Entered");
        }
        else if
         (this.gameObject.tag == "Office_Door" && other.gameObject.tag == "Player"){
            SceneManager.LoadScene("LiminalSpace");
            Debug.Log("Entered");
        }
    }
}
