using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


//Script Originally From: https://www.youtube.com/watch?v=Mfim9MlgYWY&ab_channel=JustinPBarnett
public class vrJumping : MonoBehaviour
{
    // Public variables that can be set in the inspector
    public XRNode inputSource; // Which hand/controller to use for input
    public float maxJumpForce = 10f; // Maximum force to jump with
    public float jumpGrav = -9.81f; // Gravity while jumping
    public float chargeTime = 1f; // How long to charge jump for
    public bool canJump = true; // Whether the player can currently jump

    // Private variables that are used internally
    private CharacterController character; // The character controller component
    private float fallingSpeed; // Current falling speed while jumping
    private InputDevice device; // The input device being used
    private bool chargingJump = false; // Whether the player is currently charging a jump
    private float chargeStartTime; // Time at which charging started
    private bool triggerValue = false; // Current state of the trigger button
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>(); // Get the character controller component on this gameobject
    }
    // Update is called once per frame
    void Update()
    {
        device = InputDevices.GetDeviceAtXRNode(inputSource); // Get the input device for this hand/controller
        device.TryGetFeatureValue(CommonUsages.triggerButton, out triggerValue); // Get the state of the trigger button on the device
        // If the player can jump and the trigger button is pressed, start charging the jump
        if (canJump && triggerValue)
        {
            chargingJump = true;
            chargeStartTime = Time.time;
            Debug.Log("Trigger button pressed!");
        }
        // If the player is charging a jump and the trigger button is released, jump with the appropriate force
        else if (chargingJump && !triggerValue)
        {
            chargingJump = false;
            float chargeTimeElapsed = Time.time - chargeStartTime;
            float jumpForce = Mathf.Lerp(0f, maxJumpForce, chargeTimeElapsed / chargeTime); // Lerp the jump force based on how long the player charged for
            fallingSpeed = 0;
            character.Move(Vector3.up * jumpForce * 25f * Time.fixedDeltaTime); // Move the character controller up with the jump force
            canJump = false; // Player can't jump again until they touch the ground
        }
    }
    // FixedUpdate is called a fixed number of times per second (for physics calculations)
    private void FixedUpdate()
    {
        // If the player is jumping, apply gravity
        if (!canJump)
        {
            fallingSpeed += jumpGrav * Time.fixedDeltaTime;
            character.Move(Vector3.up * 0.5f * fallingSpeed * Time.fixedDeltaTime); // Move the character controller down with the falling speed
        }
        // If the player touches the ground, they can jump again
        if (character.isGrounded)
        {
            canJump = true;
            fallingSpeed = 0;
        }
    }
}
