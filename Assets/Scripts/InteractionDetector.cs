using UnityEngine;
using TMPro;

public class InteractionDetector : MonoBehaviour
{
    private Interactable interactableInRange = null;
    public TextMeshProUGUI interactionText;

    private void Start()
    {
        interactionText.gameObject.SetActive(false);
    }

    // Fixed for Send Messages: no parameters
    public void OnInteract()
    {
        Debug.Log("OnInteract called!");

        if (interactableInRange != null && interactableInRange.CanInteract())
        {
            interactableInRange.Interact();
            Debug.Log("Interacted with: " + interactableInRange.ToString());
        }
        else
        {
            Debug.Log("No interactable in range or cannot interact.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Interactable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionText.gameObject.SetActive(true);
            interactionText.text = "Press E to interact";
            Debug.Log("Interactable in range: " + interactable.ToString());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Interactable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionText.gameObject.SetActive(false);
            Debug.Log("Left interactable range: " + interactable.ToString());
        }
    }
}
