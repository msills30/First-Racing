using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CarController target;
    private Vector3 offsetDir;

    public float minDistance, maxDistance;
    private float activeDistance;

    public Transform starTargetOffset;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offsetDir = transform.position - starTargetOffset.position;

        activeDistance = minDistance;

        offsetDir.Normalize();
    }

    // Update is called once per frame
    void Update()
    {

        activeDistance = minDistance + ((maxDistance - minDistance) * (target.theRB.linearVelocity.magnitude / target.maxSpeed));

        transform.position = target.transform.position + (offsetDir * activeDistance);
    }
}
