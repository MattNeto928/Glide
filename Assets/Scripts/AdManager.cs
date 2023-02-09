using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using TMPro;

public class AdManager : MonoBehaviour, IUnityAdsListener
{
    private string playStoreID = "4143541";
    private string appStoreID = "4143540";

    private string interstitialAd= "Interstitial_iOS";
    private string rewardedVideoAd= "Rewarded_iOS";

    public bool isTargetPlayStore;
    public bool isTestAd;

    public string adPlaying;
    public GameManager gameManager;
    public PlayerManager player;

    public TextMeshProUGUI goldEarnedText;
    public GameObject goldEarned;
    public GameObject tapToContinue;
    public Canvas menuCanvas;
    public GameObject speedDial;
    public GameObject goldIcon;
    public Canvas topCanvas;
    public GameObject bar;

    private void Start()
    {
        Advertisement.AddListener(this);
        InitializeAdvertisement();
    }

    private void InitializeAdvertisement()
    {
        if (isTargetPlayStore)
        {
            Advertisement.Initialize(playStoreID, isTestAd);
            return;
        }
        Debug.Log("Using iOS");
        Advertisement.Initialize(appStoreID, isTestAd);
    }

    public void PlayerInterstitialAd()
    {
        if (!Advertisement.IsReady(interstitialAd))
            return;

        Advertisement.Show(interstitialAd);
    }

    public void DoubleGoldRewardedVideoAd()
    {
        if (!Advertisement.IsReady(rewardedVideoAd))
            return;

        Advertisement.Show(rewardedVideoAd);
        adPlaying = "2x Gold";
        gameManager.restartLevel();
    }

    public void SecondChanceRewardedVideoAd()
    {
        if (!Advertisement.IsReady(rewardedVideoAd))
            return;

        Advertisement.Show(rewardedVideoAd);
        
        adPlaying = "2nd Chance";
    }

    public void OnUnityAdsReady(string placementId)
    {

    }

    public void OnUnityAdsDidError(string message)
    {

    }

    public void OnUnityAdsDidStart(string placementId)
    {

    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        switch(showResult)
        {
            case ShowResult.Failed: 
                break;
            case ShowResult.Skipped:
                break;
            case ShowResult.Finished:
                if (placementId == interstitialAd)
                    Debug.Log("Finished the ad");
                if (placementId == rewardedVideoAd)
                {
                    if (adPlaying.Equals("2x Gold"))
                    {
                        gameManager.gold += (int)(player.goldMultiplier * gameManager.finalDistance);
                        gameManager.goldText.text = gameManager.gold.ToString();
                        goldEarnedText.text = "+" + ((int)(player.goldMultiplier * gameManager.finalDistance)).ToString() + " Coins!";
                        goldEarned.SetActive(true);
                        gameManager.restartLevel();
                        tapToContinue.SetActive(true);
                        menuCanvas.gameObject.SetActive(false);
                        speedDial.SetActive(false);
                        goldIcon.SetActive(false);
                    }
                    else
                        if (adPlaying.Equals("2nd Chance"))
                    {
                       
                    }
                }
                break;
        }
    }

    public void exitCoinsEarned()
    {
        topCanvas.GetComponent<Animator>().Play("Black Transition"); // Plays transition back to start
        StartCoroutine(resetMenu(.55f));
    }

    IEnumerator resetMenu(float time)
    {
        yield return new WaitForSeconds(time);

        menuCanvas.gameObject.SetActive(true);
        speedDial.SetActive(true);
        goldIcon.SetActive(true);
        tapToContinue.SetActive(false);
        goldEarned.SetActive(false);
        bar.GetComponent<Animator>().Play("Gradient Bar"); // Restarts the gradient animation
    }
}
