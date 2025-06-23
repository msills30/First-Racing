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

    public int nextCheckpoint;
    public int currentLap;

    public float lapTime, bestLapTime;

    public float resetCoolDown =2f;
    private float resetCounter;

    public bool isAI;
    public int currentTarget;
    private Vector3 targetPoint;
    public float aiAccelerateSpeed = 1f, aiTurnSpeed = 0.8f, aiReachPointRange = 5f, aiPointVariance = 3f;
    private float aiSpeedInput, aiSpeedMod;
    public float aiMaxTurn = 15f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // in Godot this is _ready()
    void Start()
    {
        theRB.transform.parent = null;

        dragOnGround = theRB.linearDamping;

        if (isAI)
        {
            targetPoint = RaceManager.instance.allCheckPoints[currentTarget].transform.position;
            RandomizedAITarget();

            aiSpeedMod = Random.Range(.8f, 1f);
        }

        UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;

        resetCounter = resetCoolDown;

    }




    // Update is called once per frame
    // in Godot this _process(delta)
    void Update()
    {
        if (!RaceManager.instance.isStarting)
        {
            lapTime += Time.deltaTime;
            if (!isAI)
            {

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


                if (resetCounter > 0)
                {
                    resetCounter -= Time.deltaTime;
                }

                if (Input.GetKeyDown(KeyCode.R) && resetCounter <= 0)
                    {
                        ResetToTrack();

                    }
            }
            //This for the AI
            else
            {
                targetPoint.y = transform.position.y;

                if (Vector3.Distance(transform.position, targetPoint) < aiReachPointRange)
                {
                    /*currentTarget++;
                    if (currentTarget >= RaceManager.instance.allCheckPoints.Length)
                    {
                        currentTarget = 0;

                    targetPoint = RaceManager.instance.allCheckPoints[currentTarget].transform.position;

                    RandomizedAITarget();
                    }*/
                    SetNextAITarget();


                }

                Vector3 targetDire = targetPoint - transform.position;
                float angle = Vector3.Angle(targetDire, transform.forward);

                Vector3 LocalPos = transform.InverseTransformPoint(targetPoint);
                if (LocalPos.x < 0f)
                {
                    angle = -angle;
                }

                turnInput = Mathf.Clamp(angle / aiMaxTurn, -1f, 1f);

                if (Mathf.Abs(angle) < aiMaxTurn)
                {
                    aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, 1f, aiAccelerateSpeed);
                }
                else
                {
                    aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, aiTurnSpeed, aiAccelerateSpeed);
                }


                speedInput = aiSpeedInput * forwardAccel * aiSpeedMod;

            }

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

        if (grounded && speedInput != 0)
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


        if (isAI)
        {
            if (cpNumber == currentTarget)
            {
                SetNextAITarget();

                RandomizedAITarget();
            }
        }
    }

    public void SetNextAITarget()
    {
        currentTarget++;
        if (currentTarget >= RaceManager.instance.allCheckPoints.Length)
        {
            currentTarget = 0;
        }

        targetPoint = RaceManager.instance.allCheckPoints[currentTarget].transform.position;
        RandomizedAITarget();
    }

    public void lapCompleted()
    {
        currentLap++;

        if (lapTime < bestLapTime || bestLapTime == 0)
        {
            bestLapTime = lapTime;
        }

        lapTime = 0;

        if (currentLap <= RaceManager.instance.totalLaps)
        {
            if (!isAI)
            {
                var ts = System.TimeSpan.FromSeconds(bestLapTime);
                UIManager.instance.bestLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);


                UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;
            }
        }
        else
        {
            if (!isAI)
            {
                isAI = true;
                aiSpeedMod = 1f;

                targetPoint = RaceManager.instance.allCheckPoints[currentTarget].transform.position;
                RandomizedAITarget();
                 var ts = System.TimeSpan.FromSeconds(bestLapTime);
                UIManager.instance.bestLapTimeText.text = string.Format("{0:00}m{1:00}.{2:000}s", ts.Minutes, ts.Seconds, ts.Milliseconds);

                RaceManager.instance.FinishRace();
            }
        }

 
    }

    public void RandomizedAITarget()

    {
        //targetPoint = targetPoint + new Vector3 this what += does
        targetPoint += new Vector3(Random.Range(-aiPointVariance, aiPointVariance), 0f, Random.Range(-aiPointVariance, aiPointVariance));
    }

    void ResetToTrack()
    {
        int pointToGoTo = nextCheckpoint - 1;
        if (pointToGoTo < 0)
        {
            pointToGoTo = RaceManager.instance.allCheckPoints.Length - 1;
        }

        // Freeze physics
        theRB.isKinematic = true;

        // Move both the Rigidbody and the GameObject
        transform.position = RaceManager.instance.allCheckPoints[pointToGoTo].transform.position;
        transform.rotation = RaceManager.instance.allCheckPoints[pointToGoTo].transform.rotation;

        theRB.position = transform.position;
        theRB.rotation = transform.rotation;
        theRB.linearVelocity = Vector3.zero;
        theRB.angularVelocity = Vector3.zero;

        // Re-enable physics
        theRB.isKinematic = false;

        resetCoolDown = resetCounter;
        
    }
}
