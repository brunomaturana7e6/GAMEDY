using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Dialogue", menuName = "Dialogue/NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcName;
    public string[] dialogueLines;
    public float typingSpeed = 0.05f;
    public bool[] autoProgressLines; // If true, dialogue auto progresses after line
    public float autoProgressDelay = 1.5f; // Delay before auto progressing
}
