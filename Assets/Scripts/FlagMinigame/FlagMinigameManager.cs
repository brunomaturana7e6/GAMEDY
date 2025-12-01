using UnityEngine;
using TMPro;

public class FlagMinigameManager : MonoBehaviour
{
    [System.Serializable]
    public struct CountryFlag
    {
        public string countryName;
        public Material flagMaterial;
    }

    [Header("Data")]
    public CountryFlag[] allFlags;

    [Header("UI")]
    public TMP_Text countryText;
    public TMP_Text scoreText;
    public GameObject gameOverPanel;
    public GameObject crosshair;

    [Header("Flag Objects")]
    public Renderer[] flagRenderers;  // 4 flag planes in the world

    private Shooter shooter;
    private int correctIndex;
    private int score = 0;
    private bool gameActive = false;

    void Start()
    {
        shooter = Camera.main.GetComponent<Shooter>();
        shooter.OnFlagHit += OnFlagHit;

        StartMinigame();
    }

    public void StartMinigame()
    {
        gameActive = true;
        gameOverPanel.SetActive(false);
        crosshair.SetActive(true);
        score = 0;
        scoreText.text = "Score: 0";

        GenerateNewFlags();
    }

    void GenerateNewFlags()
    {
        int correctCountryIndex = Random.Range(0, allFlags.Length);
        CountryFlag correctCountry = allFlags[correctCountryIndex];

        countryText.text = correctCountry.countryName;

        correctIndex = Random.Range(0, 4);

        for (int i = 0; i < 4; i++)
        {
            FlagTarget t = flagRenderers[i].GetComponent<FlagTarget>();
            if (t == null) t = flagRenderers[i].gameObject.AddComponent<FlagTarget>();

            if (i == correctIndex)
            {
                flagRenderers[i].material = correctCountry.flagMaterial;
                t.IsCorrect = true;
            }
            else
            {
                int randomWrong;
                do
                {
                    randomWrong = Random.Range(0, allFlags.Length);
                } while (randomWrong == correctCountryIndex);

                flagRenderers[i].material = allFlags[randomWrong].flagMaterial;
                t.IsCorrect = false;
            }
        }
    }

    void OnFlagHit(FlagTarget target)
    {
        if (!gameActive) return;

        if (target.IsCorrect)
        {
            score++;
            scoreText.text = "Score: " + score;
            GenerateNewFlags();
        }
        else
        {
            EndMinigame();
        }
    }

    void EndMinigame()
    {
        gameActive = false;
        gameOverPanel.SetActive(true);
        crosshair.SetActive(false);
    }
}
