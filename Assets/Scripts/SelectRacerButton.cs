using UnityEngine;
using UnityEngine.UI;
public class SelectRacerButton : MonoBehaviour
{
    public Image racerImage;

    public CarController racerToSet;

    public void SelectRacer()
    {
        RaceInfoManager.instance.racerToUse = racerToSet;
        RaceInfoManager.instance.racerSprite = racerImage.sprite;

        MainMenu.instance.selectRacerImage.sprite = racerImage.sprite;
        MainMenu.instance.CloseSelectRacer();
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
