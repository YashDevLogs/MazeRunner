using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class VehicleController : MonoBehaviour
{
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    public VisualEffect rearLeftTireSmoke;
    public VisualEffect rearRightTireSmoke;

    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;

    public float maxMotorTorque = 1500f;  // Max torque the motor can apply to the wheels
    public float maxSteeringAngle = 30f;  // Max angle the front wheels can steer
    public float brakeForce = 3000f;      // Force applied when braking

    public float fuel = 100f;             // Starting fuel (100%)
    public float fuelConsumptionRate = 1f;  // How much fuel is consumed per second while moving
    public float fuelRefillAmount = 30f;  // How much fuel is added when a gas canister is collected
    [SerializeField] private Slider fuelSliderUI;
    private Rigidbody rb;

    private bool isSmokePlaying = false;

    public Transform centerOfMass;

    [SerializeField] private GameManager gameManager;

    void Start()
    {
        // Assign Rigidbody and adjust center of mass
        rb = GetComponent<Rigidbody>();
        if (centerOfMass != null)
        {
            rb.centerOfMass = centerOfMass.localPosition;
        }
    }

    void Update()
    {
        if (fuel > 0)
        {
            HandleMotor();
            HandleSteering();
            UpdateWheels();

            // Consume fuel while moving
            if (Mathf.Abs(Input.GetAxis("Vertical")) > 0)
            {
                ConsumeFuel();
                PlaySmokeVFX(true); // Start smoke when moving
            }
            else
            {
                PlaySmokeVFX(false); // Stop smoke when not moving
            }
        }
        else
        {
            StopVehicle();
            PlaySmokeVFX(false); // Stop smoke if out of fuel
            gameManager.GameOver();
        }


        fuelSliderUI.value = fuel / 100f;

    }

    private void PlaySmokeVFX(bool isPlaying)
    {
        if (isPlaying && !isSmokePlaying)
        {
            rearLeftTireSmoke.Play();
            rearRightTireSmoke.Play();
            isSmokePlaying = true;
        }
        else if (!isPlaying && isSmokePlaying)
        {
            rearLeftTireSmoke.Stop();
            rearRightTireSmoke.Stop();
            isSmokePlaying = false;
        }
    }

    // Handles forward/backward acceleration and braking
    private void HandleMotor()
    {
        float motorInput = Input.GetAxis("Vertical");
        float brakeInput = Input.GetKey(KeyCode.Space) ? brakeForce : 0f;

        // Apply torque to rear wheels for driving
        rearLeftWheelCollider.motorTorque = motorInput * maxMotorTorque;
        rearRightWheelCollider.motorTorque = motorInput * maxMotorTorque;

        // Apply brake force to all wheels
        rearLeftWheelCollider.brakeTorque = brakeInput;
        rearRightWheelCollider.brakeTorque = brakeInput;
        frontLeftWheelCollider.brakeTorque = brakeInput;
        frontRightWheelCollider.brakeTorque = brakeInput;
    }

    // Handles steering
    private void HandleSteering()
    {
        float steeringInput = Input.GetAxis("Horizontal");
        float steeringAngle = maxSteeringAngle * steeringInput;

        // Apply steering angle to the front wheels
        frontLeftWheelCollider.steerAngle = steeringAngle;
        frontRightWheelCollider.steerAngle = steeringAngle;
    }

    // Update the visual wheels to match the physics wheels
    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;

        // Get the world position and rotation of the collider
        wheelCollider.GetWorldPose(out pos, out rot);

        // Set the wheel visual to match the collider
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    // Fuel consumption
    private void ConsumeFuel()
    {
        fuel -= fuelConsumptionRate * Time.deltaTime;
        fuel = Mathf.Max(fuel, 0); // Ensure fuel doesn't go negative
    }

    // Stop the vehicle when out of fuel
    private void StopVehicle()
    {
        rearLeftWheelCollider.motorTorque = 0;
        rearRightWheelCollider.motorTorque = 0;
        frontLeftWheelCollider.steerAngle = 0;
        frontRightWheelCollider.steerAngle = 0;
    }

    // Refill fuel when a gas canister is collected
    public void Refuel()
    {
        fuel += fuelRefillAmount;
        fuel = Mathf.Min(fuel, 100f); // Max fuel cap is 100
        Debug.Log("Refueled! Fuel: " + fuel);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MazeStart"))
        {
            gameManager.StartMaze();
        }
        else if (other.gameObject.CompareTag("MazeEnd"))
        {
            gameManager.EndMaze();
        }
    }
}
