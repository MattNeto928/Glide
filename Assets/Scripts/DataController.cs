using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerManager player;

    public void LoadPlayerProgress()
    {

        if (PlayerPrefs.HasKey("gold")) //If there is a "gold" PlayerPrefs
            gameManager.gold = PlayerPrefs.GetInt("gold"); //Sets gold to the last running of the program

        if(PlayerPrefs.HasKey("distancePrice")) //If there is a "distancePrice" PlayerPrefs
            gameManager.distanceIncreasePrice = PlayerPrefs.GetInt("distancePrice");

        if (PlayerPrefs.HasKey("distanceLevel")) //If there is a "distanceLevel PlayerPrefs
            player.distanceLevel = PlayerPrefs.GetInt("distanceLevel");

        if (PlayerPrefs.HasKey("goldMultiplierPrice"))
            gameManager.goldMultiplierPrice = PlayerPrefs.GetInt("goldMultiplierPrice");

        if (PlayerPrefs.HasKey("goldMultiplier"))
            player.goldMultiplier = PlayerPrefs.GetFloat("goldMultiplier");

        if (PlayerPrefs.HasKey("jetpackLevel"))
            player.jetpackLevel = PlayerPrefs.GetInt("jetpackLevel");

        if (PlayerPrefs.HasKey("jetpackPrice"))
            gameManager.jetpackPrice = PlayerPrefs.GetInt("jetpackPrice");
    }
}
