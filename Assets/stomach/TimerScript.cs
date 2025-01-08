using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float startTime = 60f;
    private float timeRemaining;
    private bool timerRunning = false;
    public GameController gameController;

    void Start()
    {
        timeRemaining = startTime;
        UpdateTimerText();
    }

    void Update()
    {
        if (timerRunning && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerText();

            // Check if all pieces are snapped
            if (gameController != null && gameController.AreAllPiecesSnapped())
            {
                StopTimer();
                gameController.ShowWinMessage();
                return;
            }

            // Display "You Lose!" message when timer reaches zero
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                timerRunning = false;
                gameController.ShowLoseMessage();
            }
        }
    }

    public void StartTimer()
    {
        timerRunning = true;
        timeRemaining = startTime;
        UpdateTimerText();
    }

    private void StopTimer()
    {
        timerRunning = false;
    }

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = $"Time: {minutes:00}:{seconds:00}";
    }
}
