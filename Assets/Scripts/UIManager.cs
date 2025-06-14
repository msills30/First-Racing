using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    public TMP_Text lapCounterText, bestLapTimeText, currentLapTimeText;

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
        
    }
}
