using UnityEngine;

public class QuestionZoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !FootballManager.Instance.IsQuestionActive)
        {
            FootballManager.Instance.ShowNewQuestion();
            Debug.Log("Player entered question zone, new question shown.");
        }
    }
}