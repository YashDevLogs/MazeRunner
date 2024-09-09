using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class VehicleController : MonoBehaviour
{
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [SerializeField] private VisualEffect rearLeftTireSmoke;
    [SerializeField] private VisualEffect rearRightTireSmoke;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [SerializeField] private float maxMotorTorque = 1500f;  
    [SerializeField] private float maxSteeringAngle = 30f;  
    [SerializeField] private float brakeForce = 3000f;      

    public float fuel = 100f;
    [SerializeField] private float fuelConsumptionRate = 1f;
    [SerializeField] private float fuelRefillAmount = 30f;
    private bool isOutOfFuel = false;
    [SerializeField] private Slider fuelSliderUI;
    private Rigidbody rb;

    private bool isSmokePlaying = false;

    [SerializeField] private Transform centerOfMass;

    [SerializeField] private GameManager gameManager;

    void Start()
    {
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

            if (Mathf.Abs(Input.GetAxis("Vertical")) > 0)
            {
                ConsumeFuel();
                PlaySmokeVFX(true);
            }
            else
            {
                PlaySmokeVFX(false);
            }
        }
        else if (!isOutOfFuel)
        {
            StopVehicle();
            PlaySmokeVFX(false);
            gameManager.GameOver();
            isOutOfFuel = true;
        }

        ServiceLocator.instance.UIService.UpdateFuelSlider(fuel / 100f);

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

        rearLeftWheelCollider.motorTorque = motorInput * maxMotorTorque;
        rearRightWheelCollider.motorTorque = motorInput * maxMotorTorque;

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

        frontLeftWheelCollider.steerAngle = steeringAngle;
        frontRightWheelCollider.steerAngle = steeringAngle;
    }

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

        wheelCollider.GetWorldPose(out pos, out rot);

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
        fuel = Mathf.Min(fuel, 100f);  // Cap fuel at 100
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

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the vehicle has collided with a wall
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("Collided with wall");
            // Play the vehicle damage sound
            ServiceLocator.instance.SoundManager.PlaySound(SoundType.VehicleDamage);
        }
    }
}
