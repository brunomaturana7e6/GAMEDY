using UnityEngine;

public class EnterCar : MonoBehaviour
{
    [SerializeField] private Transform playerHideLocation;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera carCamera;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Car"))
        {
            playerCamera.enabled = false;
            carCamera.enabled = true;
            transform.position = playerHideLocation.position;
        }
    }
}
