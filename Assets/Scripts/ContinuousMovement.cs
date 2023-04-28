using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;


//Script Originally From: https://www.youtube.com/watch?v=Mfim9MlgYWY&ab_channel=JustinPBarnett

public class ContinuousMovement : MonoBehaviour
{
    public XRNode inputSource;  // The input device to use for movement
    public float speed = 1;  // The movement speed
    public LayerMask groundLayer;  // The layer(s) that represent the ground
    public float gravity = -9.81f;  // The gravity force to apply when falling
    public float fallingSpeed;  // The current falling speed
    private XROrigin origin;  // The XROrigin component attached to this GameObject
    private Vector2 inputAxis;  // The input axis values for movement
    public CharacterController character;  // The CharacterController component attached to this GameObject
    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();  // Get the CharacterController component
        origin = GetComponent<XROrigin>();  // Get the XROrigin component
    }
    // Update is called once per frame
    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);  // Get the input device
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);  // Get the input axis values
    }
    private void FixedUpdate()
    {
        Quaternion headYaw = Quaternion.Euler(0, origin.Camera.transform.eulerAngles.y, 0);  // Get the camera's current yaw rotation
        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);  // Create a movement direction vector based on the input axis values
        character.Move(direction * Time.fixedDeltaTime * speed);  // Move the character in the specified direction at the specified speed

        bool isGrounded = GroundCheck();  // Check if the character is on the ground
        if (isGrounded)
        {
            Debug.Log("On tha ground");  // Print a debug message
            fallingSpeed = 0;  // Reset the falling speed
            GetComponent<vrJumping>().canJump = true;  // Allow the character to jump again
        }
        else
        {
            Debug.Log("Falling");  // Print a debug message
            fallingSpeed += gravity * Time.fixedDeltaTime;  // Apply gravity to the falling speed
        }
        character.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);  // Apply the falling speed to the character's position
    }
    bool GroundCheck()
    {
        // Get the VR origin transform
        Transform vrOrigin = origin.transform;

        // Calculate the ray start position in world space
        Vector3 rayStart = vrOrigin.position;

        // Calculate the ray direction in world space
        Vector3 rayDirection = -vrOrigin.up;

        // Cast a sphere along the ray to check for ground
        float rayLength = character.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(rayStart, character.radius, rayDirection, out RaycastHit hitInfo, rayLength, groundLayer);

        // Return true if the ray hit the ground
        return hasHit;
    }
}
