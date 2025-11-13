using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class FootballManager : MonoBehaviour
{
    public static FootballManager Instance { get; private set; }

    [Header("Setup")]
    [SerializeField] private Transform spawnPoint; // Where the ball spawns
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private TextMeshProUGUI goalText;
    [SerializeField] private GameObject[] obstacleGroups; // Each group = level

    private int currentLevel = 0;
    private GameObject currentBall;

    // Input Action for respawn
    private InputAction respawnBallAction;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Setup input action for respawning the ball
        respawnBallAction = new InputAction("RespawnBall", binding: "<Keyboard>/f");
        respawnBallAction.performed += ctx => RespawnBall();
        respawnBallAction.Enable();
    }

    private void Start()
    {
        // Check if there's already a ball in the scene
        GameObject existingBall = GameObject.FindGameObjectWithTag("Ball");

        if (existingBall != null)
        {
            currentBall = existingBall;
            Debug.Log("Using existing ball in the scene.");
        }
        else
        {
            // Spawn one ball at start
            currentBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log("No existing ball found. Spawning a new one.");
        }

        // Start the first level without spawning another ball
        StartLevel(0, spawnBall: false);
    }

    public void OnGoalScored()
    {
        StartCoroutine(HandleGoalSequence());
    }

    private IEnumerator HandleGoalSequence()
    {
        // Show "GOAL!"
        goalText.gameObject.SetActive(true);

        // Destroy current ball temporarily
        if (currentBall) Destroy(currentBall);

        // Wait 2 seconds
        yield return new WaitForSeconds(2f);
        goalText.gameObject.SetActive(false);

        // Next level
        currentLevel++;
        StartLevel(currentLevel); // Will spawn a new ball by default
    }

    private void StartLevel(int level, bool spawnBall = true)
    {
        // Disable all obstacle groups first
        foreach (var group in obstacleGroups)
        {
            if (group) group.SetActive(false);
        }

        // Activate current level obstacles
        if (level < obstacleGroups.Length)
        {
            var group = obstacleGroups[level];
            group.SetActive(true);

            // Trigger animations for moving obstacles (if exist)
            foreach (var animator in group.GetComponentsInChildren<Animator>())
            {
                animator.SetTrigger("StartMove");
            }
        }

        // Spawn a new ball only if requested
        if (spawnBall)
        {
            currentBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        }
    }

    // ✅ Method to respawn the ball at the spawn point
    private void RespawnBall()
    {
        if (currentBall)
        {
            Destroy(currentBall);
        }

        currentBall = Instantiate(ballPrefab, spawnPoint.position, Quaternion.identity);
        Debug.Log("Ball respawned at spawn point.");
    }

    private void OnDisable()
    {
        respawnBallAction.Disable();
    }
}
