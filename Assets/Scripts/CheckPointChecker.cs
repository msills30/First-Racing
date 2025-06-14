using UnityEngine;

public class CheckPointChecker : MonoBehaviour
{
    public CarController theCar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            //Debug.Log("Hit cp" + other.GetComponent<CheckPoints>().cpNumber);

            theCar.CheckPointHit(other.GetComponent<CheckPoints>().cpNumber);
        }
    }
}
