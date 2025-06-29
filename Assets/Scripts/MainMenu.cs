using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public static MainMenu instance;

    public GameObject raceSetupPanel, trackSelectPanel, racerSelectPanel;

    public Image selectTrackImage , selectRacerImage;

    private void Awake()
    {
        instance =this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (RaceInfoManager.instance.enteredRace)
        {
            selectTrackImage.sprite = RaceInfoManager.instance.trackSprite;
            selectRacerImage.sprite = RaceInfoManager.instance.racerSprite;

            OpenRaceSetup();
        }

        PlayerPrefs.SetInt(RaceInfoManager.instance.trackToLoad + "_unlocked", 1);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Keys.Deleted");
            //PlayerPrefs.SetInt(RaceInfoManager.instance.trackToLoad + "_unlocked", 1);
        }
#endif
    }


    public void StartGame()
    {
        SceneManager.LoadScene(RaceInfoManager.instance.trackToLoad);

        RaceInfoManager.instance.enteredRace = true;

    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }

    public void OpenRaceSetup()
    {
        raceSetupPanel.SetActive(true);

    }

    public void CloseRaceSetup()
    {
        raceSetupPanel.SetActive(false);
    }

    public void OpenSelectTrack()
    {
        trackSelectPanel.SetActive(true);
        CloseRaceSetup();
    }

    public void CloseSelectTrack()
    {
        trackSelectPanel.SetActive(false);
        OpenRaceSetup();
    }

    public void OpenSelectRacer()
    {
        racerSelectPanel.SetActive(true);
        CloseRaceSetup();
        
    }

    public void CloseSelectRacer()
    {
        racerSelectPanel.SetActive(false);
        OpenRaceSetup();
    }

}
