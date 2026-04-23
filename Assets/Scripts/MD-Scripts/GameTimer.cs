using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private float startTimeSeconds = 60f;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private string timerPrefix = "TIME: ";

    private float _timeRemaining;
    private bool _hasEnded;

    private void Start()
    {
        // Ensure gameplay starts unpaused when scene loads.
        Time.timeScale = 1f;
        _timeRemaining = Mathf.Max(0f, startTimeSeconds);
        UpdateTimerText();
    }

    private void Update()
    {
        if (_hasEnded)
        {
            return;
        }

        _timeRemaining -= Time.deltaTime;
        if (_timeRemaining <= 0f)
        {
            _timeRemaining = 0f;
            _hasEnded = true;
            UpdateTimerText();
            Time.timeScale = 0f;
            return;
        }

        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
        {
            return;
        }

        int wholeSeconds = Mathf.CeilToInt(_timeRemaining);
        timerText.text = timerPrefix + wholeSeconds;
    }
}
