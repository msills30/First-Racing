using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;

    public CheckPoints[] allCheckPoints;

    public int totalLaps;


    public CarController playerCar;

    //list allows changes within given array
    public List<CarController> allAICars = new List<CarController>();
    public int playerPosition;

    public float timeInBetweenPosCheck = 0.2f;
    private float posCheckCounter;

    public float aiDefaultSpeed = 15f, playerDefaultSpeed = 15f, rubberBandSpeedMod = 2.5f, rubberBandAccel = 0.5f;

    public bool isStarting;
    public float timeBetweenStartCount = 1f;
    private float startCounter;
    public int countdownCurrent = 3;

    public int playerStartPosition, aiNumberToSpawn;
    public Transform[] startPoints;
    public List<CarController> carsToSpawn = new List<CarController>();

    public bool raceCompleted;

    public string raceCompleteScene;


    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        totalLaps = RaceInfoManager.instance.numOfLaps;
        aiNumberToSpawn = RaceInfoManager.instance.numOfAI;


        for (int i = 0; i < allCheckPoints.Length; i++)
        {
            allCheckPoints[i].cpNumber = i;
        }
        //we use count not length because we are using a list of cars not an array; arrays keeps things, cars in the case, fixed. The cars position is not fixed hence why we use list.
        int aiCount = allAICars.Count;

        isStarting = true;
        startCounter = timeBetweenStartCount;

        UIManager.instance.countDownText.text = countdownCurrent + "";

        playerStartPosition = Random.Range(0, aiNumberToSpawn + 1);


        playerCar = Instantiate(RaceInfoManager.instance.racerToUse, startPoints[playerStartPosition].position, startPoints[playerStartPosition].rotation);
        playerCar.isAI = false;
        //the audio listener is turned off so we need to turn it back on.
        playerCar.GetComponent<AudioListener>().enabled = true;

        CameraSwitcher.instance.SetTarget(playerCar);

        //playerCar.transform.position = startPoints[playerStartPosition].position;
        //playerCar.theRB.position = startPoints[playerStartPosition].position;

        for (int i = 0; i < aiNumberToSpawn + 1; i++)
        {
            if (i != playerStartPosition)
            {
                int selectedCar = Random.Range(0, carsToSpawn.Count);

                allAICars.Add(Instantiate(carsToSpawn[selectedCar], startPoints[i].position, startPoints[i].rotation));

                if (carsToSpawn.Count >= aiNumberToSpawn - i)
                {
                    carsToSpawn.RemoveAt(selectedCar);
                }

            }
        }
        UIManager.instance.currentPlaceText.text = (playerStartPosition + 1) + "";

    }

    // Update is called once per frame
    void Update()
    {

        if (isStarting)
        {
            startCounter -= Time.deltaTime;
            if (startCounter <= 0)
            {
                countdownCurrent--;
                startCounter = timeBetweenStartCount;

                UIManager.instance.countDownText.text = countdownCurrent + "";

                if (countdownCurrent == 0)
                {
                    isStarting = false;

                    UIManager.instance.countDownText.gameObject.SetActive(false);
                    UIManager.instance.goText.gameObject.SetActive(true);
                }
            }
        }
        else
        {

            posCheckCounter -= Time.deltaTime;
            if (posCheckCounter <= 0)
            {

                playerPosition = 1;

                foreach (CarController aiCar in allAICars)
                {
                    if (aiCar.currentLap > playerCar.currentLap)
                    {
                        playerPosition++;
                    }
                    else if (aiCar.currentLap == playerCar.currentLap)
                    {
                        if (aiCar.nextCheckpoint > playerCar.nextCheckpoint)
                        {
                            playerPosition++;
                        }
                        else if (aiCar.nextCheckpoint == playerCar.nextCheckpoint)
                        {
                            if (Vector3.Distance(aiCar.transform.position, allCheckPoints[aiCar.nextCheckpoint].transform.position) < Vector3.Distance(playerCar.transform.position, allCheckPoints[aiCar.nextCheckpoint].transform.position))
                            {
                                playerPosition++;
                            }

                        }
                    }
                }
                posCheckCounter = timeInBetweenPosCheck;


                UIManager.instance.currentPlaceText.text = playerPosition + "" ;
            }

            //Manage Rubber Band, slow down or speed up cars depending on whose winning.
            if (playerPosition == 1)
            {
                foreach (CarController aiCar in allAICars)
                {
                    aiCar.maxSpeed = Mathf.MoveTowards(aiCar.maxSpeed, aiDefaultSpeed + rubberBandSpeedMod, rubberBandAccel * Time.deltaTime);
                }

                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed - rubberBandSpeedMod, rubberBandAccel * Time.deltaTime);
            }
            else
            {
                foreach (CarController aiCar in allAICars)
                {
                    aiCar.maxSpeed = Mathf.MoveTowards(aiCar.maxSpeed, aiDefaultSpeed - (rubberBandSpeedMod * ((float)playerPosition / ((float)allAICars.Count + 1))), rubberBandAccel * Time.deltaTime);
                }
                //We use (float) to convert int, playerPosition and allAICars, into floats
                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed + (rubberBandSpeedMod * ((float)playerPosition / ((float)allAICars.Count + 1))), rubberBandAccel * Time.deltaTime);
            }
        }
    }

    public void FinishRace()
    {
        raceCompleted = true;

        //switch a substitute for if then statements
        switch (playerPosition)
        {
            case 1:
                UIManager.instance.raceResultText.text = "You're in " + playerPosition + "st place";

                if (RaceInfoManager.instance.trackToUnlock != "")
                {
                    if (!PlayerPrefs.HasKey(RaceInfoManager.instance.trackToUnlock + "_unlocked"))
                    {
                    PlayerPrefs.SetInt(RaceInfoManager.instance.trackToUnlock + "_unlocked", 1);
                    UIManager.instance.trackUnlockedMessage.SetActive(true);
                    }

                }
                break;

            case 2:
                UIManager.instance.raceResultText.text = "You're in " + playerPosition + "nd place";
                break;

            case 3:
                UIManager.instance.raceResultText.text = "You're in " + playerPosition + "rd place";
                break;

            default:
                UIManager.instance.raceResultText.text = "You're in " + playerPosition + "th place";
                break;
        }

        UIManager.instance.resultScreen.SetActive(true);

    }

    public void ExitRace()
    {
        SceneManager.LoadScene(raceCompleteScene);
    }

}

