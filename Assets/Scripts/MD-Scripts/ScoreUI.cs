using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private string scorePrefix = "SCORE: ";
    private ScoreSystem _scoreSystem;

    private void Awake()
    {
        if (scoreText == null)
        {
            scoreText = GetComponent<TMP_Text>();
        }
    }

    private void OnEnable()
    {
        BindToScoreSystem();
        UpdateScoreText();
    }

    private void Update()
    {
        if (_scoreSystem == null)
        {
            BindToScoreSystem();
        }
    }

    private void OnDisable()
    {
        if (_scoreSystem != null)
        {
            _scoreSystem.OnScoreChanged -= ScoreSystem_OnScoreChanged;
        }
    }

    private void ScoreSystem_OnScoreChanged(object sender, System.EventArgs e)
    {
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText == null)
        {
            Debug.LogWarning("ScoreUI could not find a TMP_Text reference.");
            return;
        }

        int score = _scoreSystem != null ? _scoreSystem.GetScore() : 0;
        scoreText.text = scorePrefix + score;
    }

    private void BindToScoreSystem()
    {
        if (_scoreSystem != null)
        {
            return;
        }

        _scoreSystem = ScoreSystem.Instance != null ? ScoreSystem.Instance : FindObjectOfType<ScoreSystem>();
        if (_scoreSystem != null)
        {
            _scoreSystem.OnScoreChanged += ScoreSystem_OnScoreChanged;
        }
    }
}
