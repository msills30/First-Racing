using UnityEngine;

public class CarCrashSound : MonoBehaviour
{

    public AudioSource soundToPlay;

    public int groundLayerNo = 8;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != groundLayerNo)
        {
            soundToPlay.Stop();
            soundToPlay.pitch = Random.Range(0.7f, 1.2f);
            soundToPlay.Play();
        }


    }
   //I wanted to see if you just put this in the sphere script the answer is yes, so why have it???
   /* private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            Debug.Log("Hit cp" + other.GetComponent<CheckPoints>().cpNumber);
        }
    }*/

    
}
