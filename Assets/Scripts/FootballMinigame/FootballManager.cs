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
        if (IsQuestionActive)
            return; // Don't show a new question if one is already active

        if (questions.Count == 0)
        {
            Debug.LogError("No questions in list!");
            return;
        }

        int index = Random.Range(0, questions.Count);
        currentQuestion = questions[index];

        questionText.text = currentQuestion.question;
        answerLeftText.text = currentQuestion.answerLeft;
        answerRightText.text = currentQuestion.answerRight;

        // --- FIX: dynamically tell each goal if it's correct ---
        if (currentQuestion.correctIndex == 0)
        {
            leftGoal.SetCorrectAnswer(true);   // left goal = correct
            rightGoal.SetCorrectAnswer(false); // right goal = wrong
        }
        else
        {
            leftGoal.SetCorrectAnswer(false);  // left goal = wrong
            rightGoal.SetCorrectAnswer(true);  // right goal = correct
        }

        questionText.gameObject.SetActive(true);
        IsQuestionActive = true; // Mark question as active
        Debug.Log($"New question shown. Correct answer index: {currentQuestion.correctIndex}");
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

        LoadLevel(0);
        SpawnBall();
        Debug.Log("Reset to level 0.");
    }

    private void OnDisable()
    {
        respawnBallAction.Disable();
    }
}
