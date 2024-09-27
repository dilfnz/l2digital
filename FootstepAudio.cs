using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    public AudioClip[] defaultFootstepClips;
    public AudioClip[] woodenFootstepClips;
    public AudioSource audioSource;
    public float normalStepInterval = 0.5f;
    public float sprintStepInterval = 0.3f;
    
    private float currentStepInterval;
    private float stepTimer = 0f;
    private CharacterController characterController;

    void Start()
    {
        
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        if (characterController == null)
        {
            Debug.LogError("CharacterController not found. Make sure it's on the same GameObject.");
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource not found. Make sure it's on the same GameObject.");
        }

        currentStepInterval = normalStepInterval;
    }

    void Update()
    {
        if (characterController == null || audioSource == null) return;

        // Update the current step interval based on Shift key press
        currentStepInterval = Input.GetKey(KeyCode.LeftShift) ? sprintStepInterval : normalStepInterval;

        // Check if the character is moving
        if (characterController.velocity.magnitude > 0.1f)
        {
            stepTimer += Time.deltaTime;
            if (stepTimer >= currentStepInterval)
            {
                PlayFootstep();
                stepTimer = 0f;
            }
        }
        else
        {
            // Reset the step timer if the character stops moving
            stepTimer = 0f;
        }
    }

    void PlayFootstep()
    {
        // Cast the ray from a point slightly above the player's feet, and cast downward
        Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 2f))
        {
            AudioClip selectedClip = null;

            // Check if the hit surface is on the Wooden layer
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wooden"))
            {
                if (woodenFootstepClips.Length > 0)
                {
                    selectedClip = woodenFootstepClips[Random.Range(0, woodenFootstepClips.Length)];
                }
            }
            else
            {
                if (defaultFootstepClips.Length > 0)
                {
                    selectedClip = defaultFootstepClips[Random.Range(0, defaultFootstepClips.Length)];
                }
            }

            // Play the selected clip
            if (selectedClip != null)
            {
                audioSource.PlayOneShot(selectedClip);
            }
        }
    }
}