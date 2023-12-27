using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public TextMeshProUGUI totalCoinTxt,startTimerTxt;
    int timerCount = 3;
    public bool GameStarted = false;
    public int yourPosition = 0;

    private void Awake()
    {
        if (instance == null) instance = this;
    }
    private void Start()
    {
        totalCoinTxt.text = PlayerPrefs.GetInt("totalCoinsPref").ToString();
        StartCoroutine(startTimerEnum());
    }

    IEnumerator startTimerEnum() {//3,2,1 text animation
        while (timerCount>0)
        {
            startTimerTxt.text = timerCount.ToString();
            startTimerTxt.gameObject.GetComponent<Animation>().Play("counter");
            timerCount -= 1;
            yield return new WaitForSeconds(1f);

        }
        startTimerTxt.gameObject.GetComponent<Animation>().Play("counter");
        startTimerTxt.text = "GO";
        GameStarted = true;

    }

    public void AddCoins(int amount) {//when ever player collect coin
        int crrCoins = PlayerPrefs.GetInt("totalCoinsPref");
        crrCoins += amount;
        PlayerPrefs.SetInt("totalCoinsPref",crrCoins);

        totalCoinTxt.text = PlayerPrefs.GetInt("totalCoinsPref").ToString();

    }

    public void RestartScene() =>SceneManager.LoadScene(1);
    public void LoadHomeScreen() => SceneManager.LoadScene(0); 
}
