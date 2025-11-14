using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    // This is now private, we won't assign it in the Inspector
    private bool isCorrectAnswer = false;

    // Method to set if this goal represents the correct answer
    public void SetCorrectAnswer(bool correct)
    {
        isCorrectAnswer = correct;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            // Tell the FootballManager if this was correct or not
            FootballManager.Instance.OnGoalScored(isCorrectAnswer);
            Debug.Log($"Ball entered goal. Correct: {isCorrectAnswer}");
        }
    }
}