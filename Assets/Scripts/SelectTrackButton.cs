using UnityEngine;
using UnityEngine.UI;

public class SelectTrackButton : MonoBehaviour
{

    public string trackSceneName;

    public Image trackImage;

    public int raceLap = 4;

    public GameObject unlockedText;

    private bool isLocked;

    public string trackToUnlockedOnWin;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!PlayerPrefs.HasKey(trackSceneName + "_unlocked"))
        {
            isLocked = true;
            trackImage.color = Color.gray;
            unlockedText.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectTrack()
    {
        if (!isLocked)
        {
            RaceInfoManager.instance.trackToLoad = trackSceneName;
            RaceInfoManager.instance.numOfLaps = raceLap;
            RaceInfoManager.instance.trackSprite = trackImage.sprite;

            MainMenu.instance.selectTrackImage.sprite = trackImage.sprite;

            MainMenu.instance.CloseSelectTrack();

            RaceInfoManager.instance.trackToUnlock = trackToUnlockedOnWin;
        }
        
    }
}
