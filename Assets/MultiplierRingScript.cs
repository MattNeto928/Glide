using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MultiplierRingScript : MonoBehaviour
{
    public TextMeshProUGUI inGameMultiplierText;
    private float multiplierIncrement;

    void Start()
    {
        multiplierIncrement = Random.Range(1, 3) / 10.0f;
        Debug.Log(multiplierIncrement);
        
        inGameMultiplierText.text = "+" + multiplierIncrement + "X";
    }

    public float inGameMultiplier()
    {
        return multiplierIncrement;
    }
}
