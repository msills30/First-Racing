using UnityEngine;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher instance;

    public GameObject[] cameras;
    private int currentCam;

    public CameraController topDownCamera;
    public CinemachineCamera cineCam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentCam++;

            if (currentCam >= cameras.Length)
            {
                currentCam = 0;
            }

            for (int i = 0; i < cameras.Length; i++)
            {
                if (i == currentCam)
                {
                    cameras[i].SetActive(true);
                }
                else
                {
                    cameras[i].SetActive(false);
                }
            }
        }
    }

    public void SetTarget(CarController playerCar)
    {
        topDownCamera.target = playerCar;
        cineCam.Follow = playerCar.transform;
        cineCam.LookAt = playerCar.transform;
    }
}
