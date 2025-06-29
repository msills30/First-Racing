using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public AudioSource musicPlayer;
    public AudioClip[] potentialMusic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicPlayer.clip = potentialMusic[Random.Range(0, potentialMusic.Length)];
        musicPlayer.Play();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
