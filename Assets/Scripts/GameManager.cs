using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour
{
    [Header("Classes")]
    public PlayerManager player;
    public DataController dataController;
    public TerrainManager terrainManager;
    public CloudManager cloudManager;
    public RingManager ringManager;

    [Header("Objects/Components")]
    public Animator gradientAnim;
    public GameObject deathMenu;
    public Canvas menuCanvas;
    public GameObject restartGameButton;
    public Transform gradientBar;
    public GameObject tree;
    public GameObject rock;
    public GameObject[] sand;
    public Image deathScreenTimer;
    public GameObject jetpackLock;
    public GameObject goldImage;
    public GameObject cannon;
    public ParticleSystem cannonSmoke;
    public GameObject gas;

    [Header("Text")]
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI tapToBegin;
    public TextMeshProUGUI tapToRestart;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI distanceIncreaseText;
    public TextMeshProUGUI goldMultiplierText;
    public TextMeshProUGUI deathDistanceText;
    public TextMeshProUGUI deathGoldMultiplierText;
    public TextMeshProUGUI deathGoldText;
    public TextMeshProUGUI jetpackText;

    [Header("Buttons")]
    public Button distanceIncreaseButton;
    public Button goldMultiplierButton;
    public Button startGameButton;
    public Button jetpackUpgrade;

    [Header("Variables")]
    public int distanceIncreasePrice = 500;
    public int goldMultiplierPrice = 500;
    public int jetpackPrice = 1500;
    public float gradientX;
    public int gold;
    public float offsetX, offsetY;
    public bool gameStarted = false;
    public float restartTime = 8;
    public float distance;
    public int finalDistance;
    public bool jetpackLocked = true;



    //Private Variables
    bool goldAdded;
    bool spawnTree;
    List<GameObject> allObstacles;
    private bool startDeathWindowAnim = false;
    float timeElapsed;
    float lerpDuration;
    GameObject obstacle;
    float objectSpawnHeight;




    private void Awake()
    {
        //dataController.LoadPlayerProgress(); //UNCOMMENT WHEN TESTING REALISTIC SIMULATION
        
        if (player.jetpackLevel > 0)
        {
            jetpackLocked = false;

            jetpackLock.SetActive(false);
            jetpackLocked = false;
            jetpackUpgrade.interactable = true;

            if (gold < jetpackPrice)
                jetpackUpgrade.interactable = false;

            jetpackText.text = "Jetpack\n" + "Cost: " + jetpackPrice.ToString() + "\nFuel Level: " + player.jetpackLevel;
            jetpackText.gameObject.SetActive(true);
        }


        offsetX = Random.Range(0, 9999f);
        offsetY = Random.Range(0, 9999f);

        menuCanvas.gameObject.SetActive(true);
    }

    private void Start()
    {
        

        allObstacles = new List<GameObject>();
        restartGameButton.GetComponent<Image>().enabled = false;

        gradientAnim.Play("Gradient Bar"); //Plays gradient animation
        goldAdded = false;
        spawnTree = true;
        goldText.text = gold.ToString();

        obstacle = tree;

        resetMenuTexts();


        if (gold >= distanceIncreasePrice)
            distanceIncreaseButton.interactable = true;
        else
            distanceIncreaseButton.interactable = false;

        if (gold >= goldMultiplierPrice)
            goldMultiplierButton.interactable = true;
        else
            goldMultiplierButton.interactable = false;

        lerpDuration = restartTime;

        restartLevel();
    }

    void Update()
    {
        distance = (player.transform.position.x - 207) / 10;
        
        if (player.transform.position.x > 207) //Turns on/sets distance
        {
            distanceText.gameObject.SetActive(true);
            distanceText.text = distance.ToString("#");

        }

        if (spawnTree && player.transform.position.x > 100 && !player.isDead)
        {
            StartCoroutine(spawnTrees(.1f)); //Spawns tree every __ seconds
            spawnTree = false;
        }

        if(player.isDead && !goldAdded)
        {
            finalDistance = (int)distance;
            if (distance < 0)
                finalDistance = 0;
            gold += (int)(finalDistance * (player.goldMultiplier + player.goldMultiplierInGame));
            PlayerPrefs.SetInt("gold", gold); //Makes gold stay if the game is reset
            goldAdded = true;
        }

        if(player.isDead || !gameStarted)
            goldText.text = gold.ToString();


        if (gameStarted)
        {
            goldImage.SetActive(false);
            menuCanvas.gameObject.SetActive(false);

            if(player.jetpackFuel > 0)
                gas.SetActive(true);
        }
        else
        {
            gas.SetActive(false);
            cannon.transform.localEulerAngles = new Vector3(-player.map(gradientAnim.gameObject.transform.localPosition.x, -90, 90, 0, 90), 0, 0);
            goldImage.SetActive(true);
        }

        if(startDeathWindowAnim)
        {
            deathScreenTimer.fillAmount = Mathf.Lerp(1, 0, timeElapsed / (lerpDuration / 4 - .25f)); // Dividing by 4 since time scale is .25; Subtracting by .25 due to delay of the death menu
            timeElapsed += Time.deltaTime;
            if (timeElapsed >= lerpDuration)
            {
                startDeathWindowAnim = false;
                deathScreenTimer.fillAmount = 1;
                timeElapsed = 0;
            }
        }



        if (gold >= jetpackPrice)
            jetpackUpgrade.interactable = true;
        
    }

    public void startGame() //Initial start of game
    {
        gradientAnim.speed = 0;
        gradientX = gradientAnim.gameObject.transform.localPosition.x;
        gameStarted = true;
        tapToBegin.gameObject.SetActive(false);
        startGameButton.interactable = false;
    }

    IEnumerator spawnTrees(float delay)
    {
        yield return new WaitForSeconds(delay);

        int randomX = Random.Range(350, 700);
        int randomZ = Random.Range(-600, 600);

        if (player.transform.position.x + randomX < 1024)
            obstacle = tree;
        if (player.transform.position.x + randomX >= 1024 && player.transform.position.x < 4096)
            obstacle = rock;
        if (player.transform.position.x + randomX >= 4096)
            obstacle = sand[Random.Range(0, 4)];


        allObstacles.Add(Instantiate(obstacle, new Vector3(player.transform.position.x + randomX, -1000, player.transform.position.z + randomZ), Quaternion.identity));
        spawnTree = true; //Temp variable to implement delay

    }

    public void restartLevel()
    {
        //dataController.LoadPlayerProgress();
        resetMenuTexts();

        if (player.isDead) // Needs to check if isDead since this method is invoked in PlayerManager class after t seconds
        {
            restartGameButton.GetComponent<Image>().enabled = false;
            player.goldMultiplierInGame = 0;
            player.jetpackFuel = player.jetpackLevel * 75;

            cloudManager.destroyAllClouds();

            cloudManager.initialSpawn();
            cannonSmoke.Stop();
            player.addForce = true;
            player.impulseHappened = false;
            menuCanvas.gameObject.SetActive(true);
            deathMenu.SetActive(false);
            player.isDead = false;
            Time.timeScale = 1;
            terrainManager.resetTiles();
            gameStarted = false;
            player.hasCollided = false;
            spawnTree = true;
            goldAdded = false;
            Physics.gravity = new Vector3(0, -100f, 0);
            distanceText.gameObject.SetActive(false);
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationZ //Starting constraints
            | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            if (gold >= distanceIncreasePrice)
                distanceIncreaseButton.interactable = true;
            else
                distanceIncreaseButton.interactable = false;

            if (gold >= goldMultiplierPrice)
                goldMultiplierButton.interactable = true;
            else
                goldMultiplierButton.interactable = false;


            for (int i = 0; i < allObstacles.Count; i++) //Destroys all trees in allObstacles list
            {
                Destroy(allObstacles[i]);

            }
            gradientAnim.speed = 1; //Restarts gradient animation

            startGameButton.interactable = true;

            player.transform.position = new Vector3(-75f, 135f, 0f);        //Starting transform
            player.transform.eulerAngles = new Vector3(90, 90, 0);

            ringManager.destroyAllRings();

        }
    }

    //Increase Distance upgrade in the menu
    public void distanceIncreased() 
    {
        gold -= distanceIncreasePrice;
        goldText.text = gold.ToString();
        distanceIncreasePrice = (int)(distanceIncreasePrice * 1.5f);
        player.distanceLevel++;

        distanceIncreaseText.text = "Increase Distance\n" + "Cost: " + distanceIncreasePrice.ToString() + "\nLevel: " + player.distanceLevel;

        if (gold < distanceIncreasePrice)
            distanceIncreaseButton.interactable = false;

        if (gold >= goldMultiplierPrice)
            goldMultiplierButton.interactable = true;
        else
            goldMultiplierButton.interactable = false;

        if (gold >= jetpackPrice)
            jetpackUpgrade.interactable = true;
        else
            jetpackUpgrade.interactable = false;

        PlayerPrefs.SetInt("distancePrice", distanceIncreasePrice); //Makes distanceIncreasePrice stay if the game is reset
        PlayerPrefs.SetInt("gold", gold); //Makes gold stay if the game is reset
        PlayerPrefs.SetInt("distanceLevel", player.distanceLevel); //Makes distanceLevel stay if the game is reset

    }

    public void goldMultiplier()
    {
        gold -= goldMultiplierPrice;
        goldText.text = gold.ToString();
        goldMultiplierPrice = (int)(goldMultiplierPrice * 1.25f);
        player.goldMultiplier += .1f;

        goldMultiplierText.text = "Gold Multiplier\n" + "Cost: " + goldMultiplierPrice.ToString() + "\nMultiplier: " + (player.goldMultiplier) + "x";

        if (gold < goldMultiplierPrice)
            goldMultiplierButton.interactable = false;

        if (gold >= distanceIncreasePrice)
            distanceIncreaseButton.interactable = true;
        else
            distanceIncreaseButton.interactable = false;

        if (gold >= jetpackPrice)
            jetpackUpgrade.interactable = true;
        else
            jetpackUpgrade.interactable = false;

        PlayerPrefs.SetInt("goldMultiplierPrice", goldMultiplierPrice); //Makes goldMUltiplierPrice stay the same if the game is reset
        PlayerPrefs.SetInt("gold", gold); //Makes gold stay if the game is reset
        PlayerPrefs.SetFloat("goldMultiplier", player.goldMultiplier); //Makes goldMultiplier stay if the game is reset

    }

    public void startDeathWindow()
    {
        deathMenu.SetActive(true);
        restartGameButton.GetComponent<Image>().enabled = true;
        deathDistanceText.text = finalDistance.ToString("#");
        deathGoldMultiplierText.text = (player.goldMultiplier + player.goldMultiplierInGame).ToString("#.0") + "x";
        deathGoldText.text = (finalDistance * (player.goldMultiplier+player.goldMultiplierInGame)).ToString("#");

        if (finalDistance == 0)
        {
            deathDistanceText.text = "0";
            deathGoldText.text = "0";
        }

        startDeathWindowAnim = true;


    }


    public void upgradeJetpack()
    {
        if (gold >= jetpackPrice && jetpackLocked)
        {
            jetpackLock.SetActive(false);
            gold -= 1500;
            player.jetpackLevel++;


            jetpackLocked = false;
            jetpackUpgrade.interactable = true;

            jetpackPrice = (int)(jetpackPrice * 1.5f);

            if (gold < jetpackPrice)
                jetpackUpgrade.interactable = false;

            jetpackText.text = "Jetpack\n" + "Cost: " + jetpackPrice.ToString() + "\nFuel Level: " + player.jetpackLevel;

            jetpackText.gameObject.SetActive(true);

        }
        else
            if(gold >= jetpackPrice)
        {
            gold -= jetpackPrice;
            player.jetpackLevel++;

            jetpackPrice = (int)(jetpackPrice * 1.5f);

            if (gold < jetpackPrice)
                jetpackUpgrade.interactable = false;

            jetpackText.text = "Jetpack\n" + "Cost: " + jetpackPrice.ToString() + "\nFuel Level: " + player.jetpackLevel;
        }

        player.jetpackFuel = player.jetpackLevel * 100;

        if (gold < distanceIncreasePrice)
            distanceIncreaseButton.interactable = false;
        if (gold < goldMultiplierPrice)
            goldMultiplierButton.interactable = false;

        PlayerPrefs.SetInt("jetpackLevel", player.jetpackLevel); //Makes jetpackLevel stay if the game is reset
        PlayerPrefs.SetInt("jetpackPrice", jetpackPrice); //Makes jetpackPrice stay the same if the game is reset
        PlayerPrefs.SetInt("gold", gold); //Makes gold stay if the game is reset

    }

    void resetMenuTexts()
    {
        distanceIncreaseText.text = "Increase Distance\n" + "Cost: " + distanceIncreasePrice.ToString() + "\nLevel: " + player.distanceLevel;
        goldMultiplierText.text = "Gold Multiplier\n" + "Cost: " + goldMultiplierPrice.ToString() + "\nMultiplier: " + player.goldMultiplier + "x";
        jetpackText.text = "Jetpack\n" + "Cost: " + jetpackPrice.ToString() + "\nFuel Level: " + player.jetpackLevel;
    }

    public void cannonSmokeStart()
    {
        cannonSmoke.Play();
    }

}
