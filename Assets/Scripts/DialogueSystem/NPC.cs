using StarterAssets;
using System.Collections;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour, Interactable
{
    public NPCDialogue dialogueData;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText, nameText;

    private int dialogueIndex;
    private bool isTyping, isDialogueActive;

    private ThirdPersonController playerController;

    [Header("Alternate Dialogue (after finishing minigame)")]
    public NPCDialogue completedDialogue;

    [Header("Dialogue Camera")]
    public Cinemachine.CinemachineVirtualCamera dialogueCam;
    public Transform dialogueSpot; // where the player should stand during dialogue


    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    // FIXED: start dialogue when not active, advance when active
    public void Interact()
    {
        if (FootballManager.Instance != null && FootballManager.Instance.GameCompleted)
        {
            // switch to completed dialogue
            if (completedDialogue != null)
            {
                dialogueData = completedDialogue;
            }
        }

        // If no data or no UI assigned, do nothing (safe guard)
        if (dialogueData == null || dialoguePanel == null || dialogueText == null || nameText == null)
            return;

        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        if (dialogueData == null || dialogueData.dialogueLines == null || dialogueData.dialogueLines.Length == 0)
            return;

        isDialogueActive = true;
        dialogueIndex = 0;

        nameText.SetText(dialogueData.npcName);

        dialoguePanel.SetActive(true);

        // Disable player movement
        playerController = Object.FindAnyObjectByType<ThirdPersonController>();
        if (playerController != null)
            playerController.enabled = false;

        // Switch camera
        if (dialogueCam != null)
            dialogueCam.Priority = 20; // higher than third-person cam

        // Teleport player to dialogue spot
        if (dialogueSpot != null)
            playerController.transform.SetPositionAndRotation(dialogueSpot.position, dialogueSpot.rotation);

        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        if (isTyping)
        {
            // Skip typing animation and show full line
            StopAllCoroutines();
            dialogueText.SetText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
            return;
        }

        // Move to next line if possible
        dialogueIndex++;
        if (dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");
        string line = dialogueData.dialogueLines[dialogueIndex];

        foreach (char letter in line)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        // Auto progress if configured
        if (dialogueData.autoProgressLines != null
            && dialogueIndex < dialogueData.autoProgressLines.Length
            && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueText.SetText("");
        dialoguePanel.SetActive(false);

        // Re-enable player movement
        if (playerController != null)
            playerController.enabled = true;

        // Return camera to third-person
        if (dialogueCam != null)
            dialogueCam.Priority = 0; // lower than main cam
    }
}
