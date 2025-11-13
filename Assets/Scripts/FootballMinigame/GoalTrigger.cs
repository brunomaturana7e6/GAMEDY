using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            // Notify GameManager when a goal is scored
            FootballManager.Instance.OnGoalScored();
        }
    }
}