using UnityEngine;

public class CancelDisplay : MonoBehaviour
{

    public float timetoDisable;
    // Update is called once per frame
    void Update()
    {
        timetoDisable -= Time.deltaTime;
        if (timetoDisable <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
