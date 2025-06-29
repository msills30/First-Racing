using UnityEngine;
using UnityEngine.UI;

public class RaceInfoManager : MonoBehaviour
{

    public static RaceInfoManager instance;

    public string trackToLoad;
    public CarController racerToUse;
    public int numOfAI;
    public int numOfLaps;

    public bool enteredRace;
    public Sprite trackSprite, racerSprite;

    public string trackToUnlock;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }



}
