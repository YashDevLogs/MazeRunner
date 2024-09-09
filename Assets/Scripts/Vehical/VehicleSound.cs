using UnityEngine;

public class VehicleSound : MonoBehaviour
{
    [SerializeField] private float minSpeed = 1f;  // Minimum speed for driving sound
    [SerializeField] private float maxSpeed = 100f; // Maximum speed for pitch scaling
    [SerializeField] private float currentSpeed;

    private Rigidbody carRb;
    private AudioSource carAudio; // Engine sound source
    [SerializeField] private AudioSource idleAudio; // Idle sound source (attach another AudioSource for idle sound)

    [SerializeField] private float minPitch = 0.5f; // Minimum pitch for low speed
    [SerializeField] private float maxPitch = 2f;   // Maximum pitch for high speed
    private float pitchFromCar;

    [SerializeField] private float movementThreshold = 0.1f; // Speed threshold to consider the vehicle moving

    void Start()
    {
        carAudio = GetComponent<AudioSource>(); // Get the engine sound source
        carRb = GetComponent<Rigidbody>();      // Get the Rigidbody of the vehicle
    }

    void Update()
    {
        EngineSound();
    }

    void EngineSound()
    {
        currentSpeed = carRb.velocity.magnitude; // Get current vehicle speed
        pitchFromCar = currentSpeed / maxSpeed;  // Calculate pitch based on speed

        // Check if the player is pressing W or S (forward or backward)
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            // Ensure the engine sound is playing and stop the idle sound
            if (!carAudio.isPlaying)
            {
                carAudio.Play();
                idleAudio.Stop();
            }

            // Adjust engine pitch based on speed
            if (currentSpeed < minSpeed)
            {
                carAudio.pitch = minPitch;  // Set to minimum pitch when below minimum speed
            }
            else if (currentSpeed < maxSpeed)
            {
                carAudio.pitch = Mathf.Lerp(minPitch, maxPitch, pitchFromCar);  // Scale pitch
            }
            else
            {
                carAudio.pitch = maxPitch;  // Cap pitch at maximum speed
            }
        }
        else
        {
            // Player is not pressing W or S: play idle sound and stop engine sound
            if (!idleAudio.isPlaying)
            {
                idleAudio.Play();
                carAudio.Stop();
            }
        }
    }
}
