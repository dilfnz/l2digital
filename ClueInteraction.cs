using UnityEngine;
using UnityEngine.UI;

public class ClueInteraction : MonoBehaviour
{
    // Public variables for UI elements and interaction settings
    public GameObject noteUI;
    public Text noteText;
    public string clueMessage;
    public float interactionRange = 3f;
   
    // Private variables to track player proximity and note visibility
    private bool isPlayerNearby = false;
    private bool isNoteActive = false;

    // Update is called once per frame
    void Update()
    {
        // Check for player input to show/hide the note
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (!isNoteActive)
            {
                ShowNote();
            }
            else
            {
                HideNote();
            }
        }
    }

    // Detect when the player enters the interaction range
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
        }
    }

    // Detect when the player leaves the interaction range
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
        }
    }

    // Display the note UI and set its content
    void ShowNote()
    {
        noteUI.SetActive(true);
        noteText.text = clueMessage;
        isNoteActive = true;
    }

    // Hide the note UI
    void HideNote()
    {
        noteUI.SetActive(false);
        isNoteActive = false;
        Destroy(gameObject); // Remove the clue item after the player presses 'E' again
    }
}