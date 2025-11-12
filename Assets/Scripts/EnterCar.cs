using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnterCar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private CarController carController;
    [SerializeField] private Transform exitPoint; // Where the player appears when exiting the car

    [Header("Cinemachine Cameras")]
    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private CinemachineVirtualCamera carCam;

    private bool canEnter = false;
    private bool inCar = false;

    private void Update()
    {
        if (canEnter && Input.GetKeyDown(KeyCode.E) && !inCar)
        {
            EnterVehicle();
            Debug.Log("Entered the vehicle.");
        }
        else if (inCar && Input.GetKeyDown(KeyCode.E))
        {
            ExitVehicle();
            Debug.Log("Exited the vehicle.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
            canEnter = true;
        Debug.Log("Player is near the vehicle.");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
            canEnter = false;
        Debug.Log("Player left the vehicle area.");
    }

    private void EnterVehicle()
    {
        // Hide the visible mesh but keep the GameObject active
        foreach (var renderer in player.GetComponentsInChildren<Renderer>())
            renderer.enabled = false;
        foreach (var collider in player.GetComponentsInChildren<Collider>())
            collider.enabled = false;

        carController.EnableDriving();
        inCar = true;

        playerCam.Priority = 5;
        carCam.Priority = 10;
    }

    private void ExitVehicle()
    {
        carController.DisableDriving();

        // Reactivate visibility
        foreach (var renderer in player.GetComponentsInChildren<Renderer>())
            renderer.enabled = true;
        foreach (var collider in player.GetComponentsInChildren<Collider>())
            collider.enabled = true;

        player.transform.position = exitPoint.position;
        inCar = false;

        playerCam.Priority = 10;
        carCam.Priority = 5;
    }
}
