using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    public TMP_Text lapCounterText, bestLapTimeText, currentLapTimeText, currentPlaceText, countDownText, goText, raceResultText;

    public GameObject resultScreen, pauseScreen, trackUnlockedMessage;

    public bool isPaused;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnPause();
        }
    }

    public void PauseUnPause()
    {
        isPaused = !isPaused;
        pauseScreen.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }
        else
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }


    }

    public void ExitRace()
    {
        Time.timeScale = 1f;
        RaceManager.instance.ExitRace();
        AudioListener.pause = false;

    }

    public void QuitRace()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

}
