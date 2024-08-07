using UnityEngine;
using TMPro;

public class CountDownTime : MonoBehaviour
{
    public TMP_Text displayTime;

    private float countdownTime = 120f; // Initial countdown time in seconds
    private float currentTime; // Current time left in the countdown

    public void StartCounter()
    {
        currentTime = countdownTime;
        displayTime.text = "02:00";
    }

    void Update()
    {
        // Check if the timer has not reached 0 yet
        if (currentTime > 0f)
        {
            // Decrease the timer by deltaTime
            currentTime -= Time.deltaTime;

            // Ensure the timer doesn't go negative
            if (currentTime < 0f)
            {
                currentTime = 0f;
            }

            // Display the remaining time (optional)
            displayTime.text = FormatTime(currentTime);

            //Debug.Log("Time Left: " + FormatTime(currentTime));
        }
        else
        {
            // Countdown has reached 0, handle the end of the countdown here
            displayTime.text = "";
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
