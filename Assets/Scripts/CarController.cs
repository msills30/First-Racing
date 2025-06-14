using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody theRB;

    public float maxSpeed;

    public float forwardAccel = 8f, reverseAccel = 4;
    private float speedInput;

    public float turnStrength = 180f;
    private float turnInput;

    private bool grounded;

    public Transform groundRayPoint, positionRayPoint;
    public LayerMask whatIsGround;
    public float groundRayLength = 0.75f;


    private float dragOnGround;
    public float gravityMod = 10f;

    public Transform leftFrontWheel, rightFrontWheel;
    public float maxWheelTurn = 25f;

    public ParticleSystem[] dustTrail;
    public float maxEmission = 25f, EmissionFadeSpeed = 20f;
    private float emissionRate;

    public AudioSource engineSound, skidSound;
    public float skidFadeTime = 2f;

    private int nextCheckpoint;
    public int currentLap;

    public float lapTime, bestLapTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // in Godot this is _ready()
    void Start()
    {
        theRB.transform.parent = null;

        dragOnGround = theRB.linearDamping;

        UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;

    }




    // Update is called once per frame
    // in Godot this _process(delta)
    void Update()
    {

        lapTime += Time.deltaTime;

        var ts = System.TimeSpan.FromSeconds(lapTime);
        UIManager.instance.currentLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);

        speedInput = 0f;
        if (Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * forwardAccel;

        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAccel;
        }

        turnInput = Input.GetAxis("Horizontal");

        //allows control while midair
        //if(grounded Input.GetAxis("Vertical") != 0) 

        //doesn't allow control while midair
        /*if (grounded && Input.GetAxis("Vertical") != 0)
        {

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.linearVelocity.magnitude / maxSpeed), 0f));

        }*/

        //Turning the Wheels
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);

        //transform.position = theRB.position;

        //Control Particle emission
        emissionRate = Mathf.MoveTowards(emissionRate, 0f, EmissionFadeSpeed * Time.deltaTime);

        if (theRB.linearVelocity.magnitude <= 0.5f)
        {
            emissionRate = 0;

        }

        else if (grounded && (Mathf.Abs(turnInput) > 0.5f || (theRB.linearVelocity.magnitude < maxSpeed * 0.4f && theRB.linearVelocity.magnitude != 0)))
        {
            emissionRate = maxEmission;

        }

        // i++ = i+1
        for (int i = 0; i < dustTrail.Length; i++)
        {
            var emissionModule = dustTrail[i].emission;

            emissionModule.rateOverTime = emissionRate;

        }


        if (engineSound != null)
        {
            engineSound.pitch = 1f + ((theRB.linearVelocity.magnitude / maxSpeed) * 2f);
        }

        if (skidSound != null)
        {
            if (grounded && (Mathf.Abs(turnInput)) > 0.5f)
            {
                skidSound.volume = 0.5f;
            }
            else
            {
                skidSound.volume = Mathf.MoveTowards(skidSound.volume, 0f, skidFadeTime * Time.deltaTime);
            }
        }

    }




    private void FixedUpdate()
    {
        grounded = false;

        RaycastHit hit;
        Vector3 normalTarget = Vector3.zero;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            normalTarget = hit.normal;

        }
        //this smooths the transition when going up a ramp
        if (Physics.Raycast(positionRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            //we divid by two because the two raycast and we are averaging it. So if we had four raycast, one per wheel, we divide by four.
            normalTarget = (normalTarget + hit.normal) / 2f;
        }



        //when on ground rotate to match  the normal
        if (grounded)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation;
        }

        //accelerates the car
        if (grounded)
        {
            theRB.linearDamping = dragOnGround;
            theRB.AddForce(transform.forward * speedInput * 1000);
        }
        else
        {
            theRB.linearDamping = 0.1f;

            theRB.AddForce(-Vector3.up * gravityMod * 100f);
        }


        if (theRB.linearVelocity.magnitude > maxSpeed)
        {
            theRB.linearVelocity = theRB.linearVelocity.normalized * maxSpeed;
        }



        //Remove the Debug when you are finished with making the game
        //Debug.Log(theRB.linearVelocity.magnitude);


        transform.position = theRB.position;

        if (grounded && Input.GetAxis("Vertical") != 0)
        {

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.linearVelocity.magnitude / maxSpeed), 0f));

        }

    }

    public void CheckPointHit(int cpNumber)
    {
        //Debug.Log(cpNumber);
        if (cpNumber == nextCheckpoint)
        {
            nextCheckpoint++;

            if (nextCheckpoint == RaceManager.instance.allCheckPoints.Length)
            {
                nextCheckpoint = 0; 
                lapCompleted();
            }
        }
    }

    public void lapCompleted()
    {
        currentLap++;

        if (lapTime < bestLapTime || bestLapTime == 0)
        {
            bestLapTime = lapTime;
        }

        lapTime = 0;

        
        var ts = System.TimeSpan.FromSeconds(bestLapTime);
        UIManager.instance.bestLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);


        UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;
    }
}
