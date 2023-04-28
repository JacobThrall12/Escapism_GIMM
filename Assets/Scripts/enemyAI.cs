using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class enemyAI : MonoBehaviour
{

  public AudioSource hurt;

  public AudioClip enemyDeath;
    public GameObject Player;
    public float Distance;

    public bool isAngered;

    public NavMeshAgent _agent;

    public int enemyHealth = 5;

    //public int enemyDamage;
    
    public static int numEnemies;

    public int playerHealth;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<AudioSource>().playOnAwake = false;
        
    }

    // Update is called once per frame
    void Update()
    {
       Distance = Vector3.Distance(Player.transform.position, this.transform.position);

       if(Distance <=50){
        isAngered = true;
       } 
       if(Distance > 50){
        isAngered = false;
       }

       if(isAngered){
         _agent.isStopped = false;
        _agent.SetDestination(Player.transform.position);
       }
       if(!isAngered){
        _agent.isStopped = true;
       }
       LevelDone();
    }

     void OnCollisionEnter(Collision other){
        if(other.gameObject.tag == "Weapon"){

          
          enemyHealth--;
          Debug.Log(numEnemies);
          Debug.Log("EnemyHP" + enemyHealth);
          hurt.Play();
          // AudioSource.PlayClipAtPoint(enemyDeath, transform.position, 1.5f);
          
        }
        if(other.gameObject.tag == "Dagger"){
          enemyHealth = 0;
          Destroy(other.gameObject);
        }
       
        if(enemyHealth <= 0){
          AudioSource.PlayClipAtPoint(enemyDeath, Player.transform.position, 10.5f);
          EnemyDeath();
        }
    }
    void LevelDone(){
    if(numEnemies >= 5 ){
      SceneManager.LoadScene("Intro");
      numEnemies = 0;
    }
    }
    void EnemyDeath(){
      
      numEnemies++;
      Destroy(gameObject);
    }
}
