using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    public GameObject gameScreen;
    public GameObject pauseScreen;
    public GameObject WinScreen;
    public GameObject LoseScreen;
    float preTimeScale=1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PauseGame(){
        if(Time.timeScale!=0f){
            preTimeScale = Time.timeScale;
        }
        Time.timeScale = 0;
        SwitchToScreen(pauseScreen);
    }
    public void ResumeGame(){
        Time.timeScale = preTimeScale;
        SwitchToScreen(gameScreen);
    }
    public void SwitchToScreen(GameObject screen){
        if(pauseScreen != screen){
            gameScreen.SetActive(false);
        }
        pauseScreen.SetActive(false);
        WinScreen.SetActive(false);
        LoseScreen.SetActive(false);

        screen.SetActive(true);
    }
}
