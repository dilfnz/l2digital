using UnityEngine;

public class MicrophoneInput : MonoBehaviour
{
    // AudioClip object to capture microphone input
    private AudioClip microphoneClip;
    
    // Name of the microphone device currently in use
    private string microphoneName;
    
    // Sensitivity of the microphone input, acting as a multiplier for the loudness value
    public float sensitivity = 100;
    
    // This variable will store the current loudness level of the input
    public float loudness = 0;
    
    // Maximum radius that the sound collider can grow to, based on loudness
    public float maxRadius = 10f;
    
    // Speed at which the collider radius transitions to its target size, for smoother visual changes
    public float smoothingSpeed = 5f;

    // A SphereCollider that will dynamically grow/shrink depending on the loudness of the input
    private SphereCollider soundCollider;

    void Start()
    {
        // Check if there is any microphone available; if not, log an error and stop execution
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("No microphone detected");
            return;
        }

        // Get the name of the first available microphone (assuming there is at least one)
        microphoneName = Microphone.devices[0];
        
        // Start recording from the selected microphone with a continuous loop of 1 second
        // The sample rate is set based on the system's audio settings
        microphoneClip = Microphone.Start(microphoneName, true, 1, AudioSettings.outputSampleRate);
        
        // If the microphone fails to start, log an error and stop further execution
        if (microphoneClip == null)
        {
            Debug.LogError("Failed to start microphone");
            return;
        }

        // Add a SphereCollider component to the GameObject this script is attached to
        // This collider will grow and shrink based on the loudness detected from the microphone
        soundCollider = gameObject.AddComponent<SphereCollider>();
        
        // Set the collider to act as a trigger, meaning it won't physically collide but can detect proximity
        soundCollider.isTrigger = true;
    }

    void Update()
    {
        // Get the average loudness of the current microphone input
        loudness = GetAverageLoudness();
        
        // Output the current loudness to the console for debugging purposes
        Debug.Log("Current loudness: " + loudness);
        
        // Update the size of the sound collider based on the detected loudness
        UpdateSoundCollider();
    }

    // This method calculates the average loudness from the microphone input
    float GetAverageLoudness()
    {
        // If the microphone clip is not available, return 0 (indicating no sound detected)
        if (microphoneClip == null) return 0;

        // Create an array to store audio sample data; we're using a sample size of 128
        float[] data = new float[128];
        
        // Get the current position of the microphone input and adjust for the sample size
        int micPosition = Microphone.GetPosition(microphoneName) - (128 + 1);
        
        // If the position is out of bounds, return 0 to avoid errors
        if (micPosition < 0) return 0;
        
        // Get the audio data from the microphone's AudioClip starting from the current position
        microphoneClip.GetData(data, micPosition);

        // Variable to calculate the sum of absolute values of audio data (loudness measure)
        float average = 0;
        
        // Loop through the audio data array and sum the absolute values to get the overall loudness
        for (int i = 0; i < data.Length; i++)
        {
            average += Mathf.Abs(data[i]);
        }
        
        // Divide by the array length to get the average, then multiply by sensitivity to scale the result
        average /= data.Length;

        return average * sensitivity;
    }

    // This method updates the radius of the sound collider based on the current loudness level
    void UpdateSoundCollider()
    {
        // If the sound collider is missing or not initialized, exit the function early
        if (soundCollider == null) return;

        // Calculate the target radius of the collider based on loudness, ensuring it stays within 0 and maxRadius
        float targetRadius = Mathf.Clamp(loudness * maxRadius, 0, maxRadius);
        
        // Smoothly interpolate the collider's radius towards the target radius for a smoother visual effect
        float currentRadius = Mathf.Lerp(soundCollider.radius, targetRadius, Time.deltaTime * smoothingSpeed);
        
        // Apply the new radius to the sound collider
        soundCollider.radius = currentRadius;
    }
}
