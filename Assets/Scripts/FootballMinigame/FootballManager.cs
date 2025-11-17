using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FootballManager : MonoBehaviour
{
    public static FootballManager Instance { get; private set; }

    public bool IsQuestionActive { get; private set; } = false;

    [Header("Setup")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject ballPrefab;

    [Header("UI")]
    [SerializeField] private GameObject questionUI;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI answerLeftText;
    [SerializeField] private TextMeshProUGUI answerRightText;
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("Questions")]
    public List<FootballQuestion> questions = new List<FootballQuestion>();
    private FootballQuestion currentQuestion;
    private List<FootballQuestion> remainingQuestions;

    [Header("Obstacles")]
    [SerializeField] private GameObject[] obstacleGroups;
    private int currentLevel = 0;

    [Header("Goals")]
    [SerializeField] private GoalTrigger leftGoal;
    [SerializeField] private GoalTrigger rightGoal;

    [System.Serializable]
    public class FootballQuestion
    {
        public string question;
        public string answerLeft;
        public string answerRight;
        public int correctIndex; // 0 = Left, 1 = Right
    }

    private GameObject currentBall;

    // Input
    private InputAction respawnBallAction;

    public int CorrectAnswerIndex => currentQuestion.correctIndex;

    public bool GameCompleted { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        respawnBallAction = new InputAction("RespawnBall", binding: "<Keyboard>/f");
        respawnBallAction.performed += ctx => RespawnBall();
        respawnBallAction.Enable();
    }

    private void Start()
    {
        remainingQuestions = new List<FootballQuestion>(questions);

        // Destroy any existing ball in the scene
        GameObject existingBall = GameObject.FindGameObjectWithTag("Ball");
        if (existingBall != null)
        {
            Destroy(existingBall);
            Debug.Log("Existing ball destroyed at start.");
        }

        LoadLevel(0);
        SpawnBall();
    }

    // ---------------------- BALL SPAWN ----------------------

    public void SpawnBall()
    {
        if (currentBall != null)
            Destroy(currentBall);

        currentBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log("Ball spawned.");
    }

    private void RespawnBall()
    {
        SpawnBall();
        Debug.Log("Ball respawned manually.");
    }

    // ---------------------- LEVEL MANAGEMENT ----------------------

    private void LoadLevel(int level)
    {
        currentLevel = level;

        foreach (var group in obstacleGroups)
            group.SetActive(false);

        if (level < obstacleGroups.Length)
        {
            obstacleGroups[level].SetActive(true);

            foreach (var anim in obstacleGroups[level].GetComponentsInChildren<Animator>())
                anim.SetTrigger("StartMove");
        }
        Debug.Log($"Level {level} loaded.");
    }

    public void ShowNewQuestion()
    {
        if (GameCompleted)
        {
            Debug.Log("Game finished. No more questions.");
            return;
        }

        if (IsQuestionActive)
            return; // Don't show a new question if one is already active

        // Refill pool if empty (all questions used in this run)
        if (remainingQuestions.Count == 0)
            remainingQuestions = new List<FootballQuestion>(questions);

        int index = Random.Range(0, remainingQuestions.Count);
        currentQuestion = remainingQuestions[index];

        // Remove question so it won't repeat this round
        remainingQuestions.RemoveAt(index);

        questionText.text = currentQuestion.question;
        answerLeftText.text = currentQuestion.answerLeft;
        answerRightText.text = currentQuestion.answerRight;

        // Assign correct side
        leftGoal.SetCorrectAnswer(currentQuestion.correctIndex == 0);
        rightGoal.SetCorrectAnswer(currentQuestion.correctIndex == 1);

        questionText.gameObject.SetActive(true);
        IsQuestionActive = true; // Mark question as active

        Debug.Log("New question shown. Remaining questions: " + remainingQuestions.Count);
    }

    // ---------------------- GOAL RESULT ----------------------

    public void OnGoalScored(bool isCorrect)
    {
        questionText.gameObject.SetActive(false);
        IsQuestionActive = false; // Question answered, allow new ones

        if (isCorrect)
            StartCoroutine(HandleCorrectGoal());
        else
            StartCoroutine(HandleIncorrectGoal());

        Debug.Log($"Goal scored. Answer was {(isCorrect ? "correct" : "incorrect")}.");
    }

    private IEnumerator HandleCorrectGoal()
    {
        resultText.text = "CORRECT!";
        resultText.color = Color.green;
        resultText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        resultText.gameObject.SetActive(false);

        currentLevel++;

        // ✔️ If finished all levels
        if (currentLevel >= obstacleGroups.Length)
        {
            GameCompleted = true;
            Debug.Log("All levels completed! No more questions will appear.");
            yield break; // Do NOT spawn a new ball, do NOT load levels
        }

        LoadLevel(currentLevel);
        SpawnBall();
        Debug.Log("Advanced to next level.");
    }

    private IEnumerator HandleIncorrectGoal()
    {
        resultText.text = "WRONG!";
        resultText.color = Color.red;
        resultText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        resultText.gameObject.SetActive(false);

        // Reset to level 0
        LoadLevel(0);

        // Reset question pool (NEW)
        remainingQuestions = new List<FootballQuestion>(questions);

        SpawnBall();
        Debug.Log("Reset to level 0. Question pool refilled.");
    }

    private void OnDisable()
    {
        respawnBallAction.Disable();
    }
}
