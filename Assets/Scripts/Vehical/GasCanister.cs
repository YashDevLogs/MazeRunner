
using UnityEngine;

public class GasCanister : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Collided with Player");
            VehicleController vehicle = other.GetComponent<VehicleController>();
            if (vehicle != null)
            {
                ServiceLocator.instance.SoundManager.PlaySound(SoundType.FuelPicked);
                Debug.Log("Got Vehical Controller");
                vehicle.Refuel(); // Refill the fuel
                Destroy(gameObject); // Destroy the canister after collection
            }
        }
    }
}
