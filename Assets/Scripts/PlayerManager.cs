using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [Header("Scripts")]
    public GameManager gameManager;

    //Components
    [Header("Classes/Components")]
    Rigidbody rb;
    [HideInInspector] public CharacterController controller;
    public TerrainManager terrainManager;
    public ParticleSystem smokeParticle, flamesParticle;
    Animator animator;
    public Transform cannon;
    public Image fuelBar;


    //Public Constants
    [Header("Changeable Constants")]
    public int speed = 5;
    public int distanceLevel;
    public float goldMultiplier;
    public float goldMultiplierInGame;
    public int jetpackLevel;
    public int thrust;
    public float jetpackFuel;


    //Private variables
    float gravityChange;
    float totalVelocity;
    float timeInAir;
    [HideInInspector] public bool hasCollided = false;
    bool justLeftRamp = false;
    [HideInInspector] public bool isDead;
    bool touchingFloor;
    [HideInInspector] public bool addForce;
    [HideInInspector] public static float xLocation;

    public int xTile, yTile;

    public bool impulseHappened = false;
    public int impulsePower;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        gameObject.AddComponent<CharacterController>();
        controller = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();

        addForce = true;

        jetpackFuel = jetpackLevel * 50;

    }

    private void Update()
    {
        xLocation = transform.position.x;

        if (Input.GetKeyDown(KeyCode.R))
        {
            isDead = true;
            gameManager.restartLevel();
        }

        findPlayerTile();
        if (!isDead)
            jetpack();
    }

    // Update is called once per frame
    void FixedUpdate()
    {



        if (!gameManager.gameStarted)
        {
            rb.isKinematic = true;
            // rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Physics.gravity = new Vector3(0, -100, 0);
            transform.eulerAngles = new Vector3(cannon.localEulerAngles.x, 90, 0);
        }
        else
        {

            if (addForce == true)
            {
                gameManager.cannonSmokeStart();


                rb.isKinematic = false;

                rb.velocity = transform.forward * (50 * distanceLevel + 200);
                addForce = false;

            }

            fuelBar.GetComponent<RectTransform>().sizeDelta = new Vector2(jetpackFuel, 55);

        }


        Vector3 tilt = Input.acceleration;

        tilt = Quaternion.Euler(0, 90, 0) * tilt;

        if (!isDead)
        {

            if (transform.position.x > 280 && !justLeftRamp)
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.constraints = RigidbodyConstraints.FreezeRotationY;
            justLeftRamp = true;
        }

        float playerTilt = map(tilt.z, -1, 1, -30, 30); //Remapping tilt value (-1 to 1) to a value from -30 to 30 (z angle)


        if (transform.position.x > 280 && !hasCollided)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, tilt.z * speed);
            if(Input.GetKey(KeyCode.A))
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, speed);
                playerTilt = 30;
            }
            if (Input.GetKey(KeyCode.D))
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, -speed);
                playerTilt = -30;
            }

            //Debug.Log("x: " + tilt.x + " y: " + tilt.y + " z: " + tilt.z + " player tilt: " + playerTilt);


        }

        gravityChange = map(-Mathf.Abs(gameManager.gradientX), -90, 0, 1.25f, .75f);


            if (transform.position.x > 150 && !hasCollided && transform.position.x < 207)
            {
                //Changes game gravity constant -> -80(constant) * gravityChange(map of gradientX into multiplier 125%-75%) + distanceLevel(level gotten by player) * 5(multiplier)
                //Physics.gravity = new Vector3(0, -80 * gravityChange + (distanceLevel * 5), 0);

                //Physics.gravity = Vector3.down * 200;
            }


            if (transform.position.x < 207)
            {
                totalVelocity = Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.y, 2));
                timeInAir = -(rb.velocity.y / Physics.gravity.y) * 2;
            }
            else
                if (transform.position.x > 215 && !hasCollided)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative; //Changes physics detection. Very important
                controller.center = new Vector3(0, 0, 0.01f); //Changes center of mass so rigidbody will turn upon leaving the ramp
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 90, playerTilt); //Tilts rigidbody as phone is tilted
            }

            if (transform.position.x > 450 && transform.eulerAngles.x > 0 && !hasCollided) //Changes rigidbody constraints
            {
                rb.constraints = RigidbodyConstraints.FreezeRotationX;
                rb.constraints = RigidbodyConstraints.FreezeRotationZ;
            }
        }

        checkLevelRestart();
            

    }

    void checkLevelRestart()
    {
        if (((rb.velocity.magnitude <= 75 && touchingFloor) || rb.velocity.magnitude <= 25) && hasCollided)
        {
            if (!isDead)
            {
                gameManager.Invoke("startDeathWindow", 1f / 4); // Dividing by 4 since Time.timeScale is .25

                gameManager.Invoke("restartLevel", gameManager.restartTime / 4); // Dividing by 4 since Time.timeScale is .25

                Time.timeScale = .25f;

                if (rb.velocity.magnitude <= 1)
                    rb.isKinematic = true;

            }
            isDead = true;

        }

        if (transform.position.y < -60) //Restarts if player falls through map
            gameManager.restartLevel();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Gas Ring"))
        {
            if (jetpackFuel <= 0)
                gameManager.gas.SetActive(true);
            jetpackFuel += 200;
            Debug.Log("GAS");
            Destroy(other.gameObject);
        }
        
        if(other.tag.Equals("Multiplier Ring"))
        {
            goldMultiplierInGame += other.GetComponent<MultiplierRingScript>().inGameMultiplier();
            Debug.Log("Multiplier: + " + goldMultiplierInGame);
            Destroy(other.gameObject);

        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "Tree" || collision.gameObject.tag == "Terrain" || collision.gameObject.tag == "Rock") && !isDead)
        {
            Physics.gravity = new Vector3(0, -200f, 0);
            hasCollided = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.mass = 100;
            controller.center = new Vector3();
        }

        if (collision.gameObject.tag == "Terrain")
            touchingFloor = true; 

    }

    void findPlayerTile()
    {
        int xValue = 0, yValue = 0;
        if (transform.position.x < 0)
            xValue = -1;
        else
            xValue = 0;

        if (transform.position.z < 0)
            yValue = -1;
        else
            yValue = 0;

        xTile = (int)(transform.position.x / terrainManager.terrainSize + xValue);
        yTile = (int)(transform.position.z / terrainManager.terrainSize + yValue);

    }

    void jetpack()
    {
        if(transform.position.x > 210 && (Input.touchCount > 0 || Input.GetMouseButton(0)) && jetpackFuel > 0 && !isDead)
        {
            flamesParticle.Play();
            smokeParticle.Play();

            if (rb.velocity.y < 400)
            rb.AddForce(0, thrust * Time.deltaTime, 0, ForceMode.Acceleration);

            jetpackFuel -= (50 * Time.deltaTime);
        }
        else
        {
            flamesParticle.Stop();
            smokeParticle.Stop();
        }

        if (jetpackFuel <= 0)
            gameManager.gas.SetActive(false);

    }

    public float map(float value, float  low1, float high1, float low2, float high2)
    {
        return low2 + (value - low1) * (high2 - low2) / (high1 - low1);
    }
}
