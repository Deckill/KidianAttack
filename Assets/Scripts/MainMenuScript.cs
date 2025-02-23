using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public GameObject startGameScreen;
    public GameObject tutorialMainScreen;
    public List<GameObject> tutorialScreens;
    public TMP_Text gameMode0Text;
    public TMP_Text gameMode1Text;

    // Start is called before the first frame update
    void Start()
    {
        SwitchToScreen(null);
        Time.timeScale=1f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SwitchToScreen(GameObject screen){
        startGameScreen.SetActive(false);
        if(screen == startGameScreen){
            gameMode0Text.text=gameMode0Text.text.Replace("{highScore0}",ScoreManager.LoadScore(0).ToString());
            gameMode1Text.text=gameMode1Text.text.Replace("{highScore1}",ScoreManager.LoadScore(1).ToString());}
        tutorialMainScreen.SetActive(false);
        foreach (GameObject tutorialScreen in tutorialScreens){
            tutorialScreen.SetActive(false);
            if(screen==tutorialScreen){
                tutorialMainScreen.SetActive(true);
            }
        }
        
        if(screen!=null){
            screen.SetActive(true);
            if(screen ==tutorialMainScreen){
                tutorialScreens[0].SetActive(true);
            }
        }
    }
    public void StartGame(int gameMode){
        PlayerPrefs.SetInt("GameMode", gameMode);
        SceneManager.LoadScene("DefenseMode");
        PlayerPrefs.Save();
    }
}
