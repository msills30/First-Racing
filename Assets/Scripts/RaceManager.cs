using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;

    public CheckPoints[] allCheckPoints;

    public int totalLaps;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < allCheckPoints.Length; i++)
        {
            allCheckPoints[i].cpNumber = i;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
