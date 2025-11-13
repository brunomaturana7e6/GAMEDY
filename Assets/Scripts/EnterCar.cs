using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnterCar : MonoBehaviour, InputSystem_Actions.ICarEnterActions
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

    private InputSystem_Actions _actions;

    private void Awake()
    {
        _actions = new InputSystem_Actions();
        _actions.CarEnter.SetCallbacks(this);
    }
    private void OnEnable()
    {
        _actions.Enable();
    }
    private void OnDisable()
    {
        _actions.Disable();
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

    public void OnEnter(InputAction.CallbackContext context)
    {
        if (canEnter && !inCar)
        {
            EnterVehicle();
            Debug.Log("Entered the vehicle.");
        }
        else if (inCar)
        {
            ExitVehicle();
            Debug.Log("Exited the vehicle.");
        }
    }
}
