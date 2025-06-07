using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody theRB;

    public float maxSpeed;

    public float forwardAccel = 8f, reverseAccel = 4;

    private float speedInput;

     public float turnStrength = 180f;

    private float turnInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // in Godot this is _ready()
    void Start()
    {
        theRB.transform.parent = null;
    }

    // Update is called once per frame
    // in Godot this _process(delta)
    void Update()
    {
        speedInput = 0f;
        if(Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * forwardAccel;
            
        } else if(Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAccel;
        }

        turnInput = Input.GetAxis("Horizontal");

         if(Input.GetAxis("Vertical") != 0)
         {

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude / maxSpeed),0f));

         }



    
    transform.position = theRB.position;
    }

    

   


    private void FixedUpdate()
    {
        theRB.AddForce(transform.forward * speedInput * 1000); 
    }
}
