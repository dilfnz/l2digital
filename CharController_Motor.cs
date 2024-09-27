using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharController_Motor : MonoBehaviour {

    // Public variables for character movement and camera settings
    public float speed = 3.5f;
    public float sprintSpeed = 5.0f;
    public float crouchSpeed = 1.5f;
    public float sensitivity = 70.0f;
    public float WaterHeight = 15.5f;
    public float crouchHeight = 1.0f;
    public float normalHeight = 2.0f;
    public float sprintDuration = 3.0f; 
    public Camera playerCamera;
    public float normalFOV = 60.0f;
    public float sprintFOV = 70.0f;
    public float bobbingSpeed = 0.18f;
    public float bobbingAmount = 0.2f;
    public float groundedRayDistance = 0.1f;
    public float slopeLimit = 45f;
    public bool isSprinting = false;
    public LayerMask groundLayer;

    // Private variables for internal use
    private CharacterController character;
    private float moveFB, moveLR;
    private float rotX, rotY;
    public bool webGLRightClickRotation = true;
    private float gravity = -10.0f;
    private Vector3 moveDirection = Vector3.zero;
    private bool isCrouching = false;
    private float currentSpeed;
    private float sprintTimer = 0.0f;
    private float defaultYPos = 0;
    private float timer = 0;
    private bool isGrounded;
    private Vector3 groundNormal;
    
    // Initialization
    void Start() {
        character = GetComponent<CharacterController>();
        currentSpeed = speed;
        if (Application.isEditor) {
            webGLRightClickRotation = false;
            sensitivity = sensitivity * 1.5f;
        }
        defaultYPos = playerCamera.transform.localPosition.y;
        playerCamera.fieldOfView = normalFOV;
        Cursor.lockState = CursorLockMode.Locked;   // keep confined to center of screen
    }
    // Main update loop
    void Update() {
        
        CheckGrounded();

        
        HandleMovementSpeed();
        // Calculate movement vector
        Vector3 horizontalMovement = transform.TransformDirection(new Vector3(Input.GetAxis("Horizontal") * currentSpeed, 0, Input.GetAxis("Vertical") * currentSpeed));
        // Get mouse input for rotation
        rotX = Input.GetAxis("Mouse X") * sensitivity;
        rotY = Input.GetAxis("Mouse Y") * sensitivity;

        CheckForWaterHeight();
        // Handle jumping and gravity
        if (isGrounded) {
            // Apply slope movement
            horizontalMovement = Vector3.ProjectOnPlane(horizontalMovement, groundNormal);

            
            moveDirection.y = -2f;

            
        } else {
            // Apply gravity when in air
            moveDirection.y += gravity * Time.deltaTime;
        }

        // Set horizontal movement        
        moveDirection.x = horizontalMovement.x;
        moveDirection.z = horizontalMovement.z;

        HandleCrouching();
        ApplyCameraEffects();

        // Apply camera rotation        
        if (webGLRightClickRotation) {
            if (Input.GetKey(KeyCode.Mouse0)) {
                CameraRotation(playerCamera.gameObject, rotX, rotY);
            }
        } else {
            CameraRotation(playerCamera.gameObject, rotX, rotY);
        }

        
        character.Move(moveDirection * Time.deltaTime);
    }
    // Check if the character is grounded
    void CheckGrounded() {
        isGrounded = false;
        RaycastHit hit;
        Vector3 rayStart = transform.position + character.center;

        if (Physics.SphereCast(rayStart, character.radius, Vector3.down, out hit, 
                               character.height / 2f - character.radius + groundedRayDistance, groundLayer)) {
            isGrounded = true;
            groundNormal = hit.normal;

            
            if (Vector3.Angle(groundNormal, Vector3.up) > slopeLimit) {
                isGrounded = false;
            }
        }
    }

    void CameraRotation(GameObject cam, float rotX, float rotY) {
        transform.Rotate(0, rotX * Time.deltaTime, 0);
        cam.transform.Rotate(-rotY * Time.deltaTime, 0, 0);
    }
    // Adjust gravity based on water height
    void CheckForWaterHeight() {
        if (transform.position.y < WaterHeight) {
            gravity = 0f;
        } else {
            gravity = -20.0f;
        }
    }
    // Handle sprinting and movement speed changes
    void HandleMovementSpeed() {
        if (Input.GetKey(KeyCode.LeftShift) && !isCrouching) {
            if (!isSprinting || sprintTimer < sprintDuration) {
                currentSpeed = sprintSpeed;
                sprintTimer += Time.deltaTime;
                isSprinting = true;
            } else {
                currentSpeed = speed; 
                isSprinting = false;
            }
        } else {
            if (isSprinting) {
                sprintTimer = 0; 
                isSprinting = false;
            }
            currentSpeed = speed;
        }
    }
    // Apply camera effects like FOV changes and head bobbing
    void ApplyCameraEffects() {
        if (isSprinting) {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, sprintFOV, Time.deltaTime * 5);
        } else {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, normalFOV, Time.deltaTime * 5);
        }

        if (character.velocity.magnitude > 0.1f && character.isGrounded) {
            timer += Time.deltaTime * bobbingSpeed;
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * bobbingAmount,
                playerCamera.transform.localPosition.z
            );
        } else {
            timer = 0;
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                Mathf.Lerp(playerCamera.transform.localPosition.y, defaultYPos, Time.deltaTime * 5),
                playerCamera.transform.localPosition.z
            );
        }
    }
    // Handle crouching input
    void HandleCrouching() {
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            Crouch();
        }
        if (Input.GetKeyUp(KeyCode.LeftControl)) {
            StandUp();
        }
    }
    // Crouch the character
    void Crouch() {
        if (!isCrouching) {
            character.height = crouchHeight;
            currentSpeed = crouchSpeed;
            isCrouching = true;
        }
    }
    // Stand the character up
    void StandUp() {
        if (isCrouching) {
            character.height = normalHeight;
            currentSpeed = speed;
            isCrouching = false;
        }
    }
}