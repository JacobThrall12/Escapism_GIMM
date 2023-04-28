using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Required to use XR Grab Interactable
using UnityEngine.UI;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using TMPro;
public class LookScript : MonoBehaviour
{
    public TextMeshProUGUI instructionsSwap;
    [SerializeField] private GameObject enemyObject; // The enemy object to interact with
    [SerializeField] private float cooldownDuration = 5f; // Cooldown period between swapping
    private bool isOnCooldown = false; // Whether the script is on cooldown or not
    [SerializeField] private GameObject enemyPrefab; // The enemy prefab to instantiate
    [SerializeField] private GameObject xrOriginPrefab; // The XR origin prefab to instantiate
    [SerializeField] private float maxTimer = 3f; // The time it takes to swap when looking at the enemy for this long
    [SerializeField] private GameObject progressBar; // The progress bar UI element
    private XRGrabInteractable interactable; // The XR Grab Interactable component attached to the enemy object
    private Slider progressBarSlider; // The slider component attached to the progress bar UI element
    private float timer; // The timer for how long the player has been looking at the enemy object
    private bool isHit; // Whether the player's raycast has hit the enemy object or not
    private XROrigin playerRig; // The player's XR origin component
    public XRNode inputSource; // The input source of the VR controller (left or right hand)
    private InputDevice device; // The input device of the VR controller
    private bool triggerValue = false; // Whether the trigger button on the VR controller is being pressed or not

    public LayerMask IgnoreLayer;
    void Start()
    {
        timer = 0f;
        isHit = false;
        playerRig = GetComponent<XROrigin>();
    }

    void Update()
    {
        device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerValue);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, ~IgnoreLayer))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                if (!isHit)
                {
                    interactable = hit.collider.GetComponent<XRGrabInteractable>();
                    if (interactable != null)
                    {
                        Debug.Log("Looking at enemy");
                        progressBarSlider = Instantiate(progressBar, transform).GetComponentInChildren<Slider>();
                        progressBarSlider.maxValue = maxTimer;
                    }
                    isHit = true;
                    timer = 0f;
                }
                timer += Time.deltaTime;
                progressBarSlider.value = timer;
                if (timer >= maxTimer)
                {
                    gameObject.GetComponent<vrJumping>().enabled = true;
                    //instructionsSwap.text = "Look at an Enemy and Pull Left Trigger at the same time";
                }
                if (timer >= maxTimer && triggerValue)
                {
                    StartCoroutine(SwapPlaces());
                    Debug.Log("Starting Coroutine");
                    //gameObject.GetComponent<vrJumping>().enabled = true;
                    gameObject.GetComponent<shrinkVR>().enabled = true;
                }
            }
            else
            {
                if (isHit)
                {
                    timer = 0f;
                    progressBarSlider.value = timer;
                    Destroy(progressBarSlider.gameObject);
                    isHit = false;
                }
            }
        }
        else
        {
            if (isHit)
            {
                timer = 0f;
                progressBarSlider.value = timer;
                Destroy(progressBarSlider.gameObject);
                isHit = false;
            }
        }
    }

    //Future testing: Give player and Enemy a burst of speed to move them past each other to the right spots
    
    // Coroutine for swapping places
    private IEnumerator SwapPlaces()
    {
        if (isOnCooldown)
        {
            yield break;
        }
        isOnCooldown = true;
        Debug.Log("Swap!");
        // Get the current position and rotation of the player
        Vector3 playerPos = playerRig.transform.position;
        Quaternion playerRot = playerRig.transform.rotation;
        // Find the enemy object by tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length > 0)
        {
            GameObject enemy = enemies[0];
            // Instantiate a new XROrigin at the enemy's position
            GameObject xrOrigin = Instantiate(xrOriginPrefab, enemy.transform.position, enemy.transform.rotation);
            // Instantiate a new Enemy at the player's position
            GameObject newEnemy = Instantiate(enemyPrefab, playerPos, playerRot);
            // Turn off physics on the enemy and newEnemy
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            Rigidbody newEnemyRb = newEnemy.GetComponent<Rigidbody>();
            enemyRb.isKinematic = true;
            newEnemyRb.isKinematic = true;
            // Calculate the positions where the objects will meet
            Vector3 midpoint = (playerPos + enemy.transform.position) / 2f;
            Vector3 playerTargetPos = midpoint + (midpoint - enemy.transform.position);
            Vector3 enemyTargetPos = midpoint + (midpoint - playerPos);
            // Move the objects towards each other
            float t = 0f;
            float moveDuration = 1f;
            while (t < moveDuration)
            {
                t += Time.deltaTime;
                float normalizedTime = t / moveDuration;
                enemy.transform.position = Vector3.Lerp(enemy.transform.position, enemyTargetPos, normalizedTime);
                newEnemy.transform.position = Vector3.Lerp(playerPos, playerTargetPos, normalizedTime);
                yield return null;
            }
            // Set the final positions and rotations of the objects
            enemy.transform.position = enemyTargetPos;
            enemy.transform.rotation = playerRot;
            newEnemy.transform.position = playerTargetPos;
            newEnemy.transform.rotation = enemy.transform.rotation;
            // Turn physics back on for the enemy and newEnemy
            enemyRb.isKinematic = false;
            newEnemyRb.isKinematic = false;
            // Destroy the old XROrigin and Enemy objects
            Destroy(gameObject);
            Destroy(enemy);
            //Destroy(xrOrigin);
            
            yield return new WaitForSeconds(cooldownDuration);
            isOnCooldown = false;
        }
    }
}
