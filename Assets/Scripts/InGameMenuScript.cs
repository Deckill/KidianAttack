using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InGameMenuScript : MonoBehaviour
{
    public GameObject gameScreen;
    public GameObject pauseScreen;
    public GameObject winScreen;
    public GameObject loseScreen;
    public GameObject rangeCircle;
    public Toggle drawCircleToggle;
    public GameObject currentScoreTMPObj;
    public GameObject failScoreTMPObj;
    public GameObject successScoreTMPObj;
    public GameObject additionalScorePrefab;
    public int currentScore=0;
    string initialScoreText ;
    float preTimeScale=1f;
    // Start is called before the first frame update
    void Start()
    {
        initialScoreText = currentScoreTMPObj.GetComponent<TextMeshProUGUI>().text;
        drawCircleToggle.isOn=(PlayerPrefs.GetInt("DrawCircleBool",1)==1);
        rangeCircle.SetActive((PlayerPrefs.GetInt("DrawCircleBool",1)==1));
        Time.timeScale=1f;
        UpdateCurrentScore();
    }
    public void PauseGame(){
        if(Time.timeScale!=0f){
            preTimeScale = Time.timeScale;
        }
        Time.timeScale = 0;
        SwitchToScreen(pauseScreen);
    }
    public void RestartGame(){
        SceneManager.LoadScene("DefenseMode");
        ResumeGame();
    }
    public void ExitGame(){
        SceneManager.LoadScene("MainMenu");
    }
    public void ResumeGame(){
        Time.timeScale = preTimeScale;
        SwitchToScreen(gameScreen);
    }
    public void ShowFailScreen(){
        SwitchToScreen(loseScreen);
    }
    public void ShowWinScreen(){
        SwitchToScreen(winScreen);
    }
    public void SwitchToScreen(GameObject screen){
        if(pauseScreen != screen){
            gameScreen.SetActive(false);
        }
        pauseScreen.SetActive(false);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        screen.SetActive(true);
    }
    public void ToggleRangeCircle2(){
        if (drawCircleToggle.IsActive()){
            rangeCircle.SetActive(!rangeCircle.activeSelf);
            if(rangeCircle.activeSelf){
                PlayerPrefs.SetInt("DrawCircleBool",1);            
            }else{
                PlayerPrefs.SetInt("DrawCircleBool",0);
            }
            PlayerPrefs.Save();
        }
    }
    public void UpdateCurrentScore(){
        currentScoreTMPObj.GetComponent<TextMeshProUGUI>().text=initialScoreText;
        currentScoreTMPObj.GetComponent<TextMeshProUGUI>().text =
            currentScoreTMPObj.GetComponent<TextMeshProUGUI>().text.Replace("{currentScore}",currentScore.ToString());
        currentScoreTMPObj.GetComponent<TextMeshProUGUI>().text =
            currentScoreTMPObj.GetComponent<TextMeshProUGUI>().text.Replace("{highScore}",ScoreManager.LoadScore(PlayerPrefs.GetInt("GameMode")).ToString());
        failScoreTMPObj.GetComponent <TextMeshProUGUI>().SetText(currentScoreTMPObj.GetComponent<TextMeshProUGUI>().text);
        successScoreTMPObj.GetComponent <TextMeshProUGUI>().SetText(currentScoreTMPObj.GetComponent<TextMeshProUGUI>().text);
    }
    public void AddScore(int additionalScore){
        currentScore +=additionalScore;
        GameObject additionalScoreObj= Instantiate(additionalScorePrefab,currentScoreTMPObj.transform);
        additionalScoreObj.transform.position += Vector3.left*5+Vector3.up*5;
        additionalScoreObj.GetComponent<TextMeshProUGUI>().text = "+"+additionalScore.ToString();
        UpdateHighScore(PlayerPrefs.GetInt("GameMode"));
        UpdateCurrentScore();
    }
    public void UpdateHighScore(int GameMode){
        int highScore =ScoreManager.LoadScore(GameMode);
        //Debug.Log(""+highScore);
        if(currentScore>highScore){
            ScoreManager.SaveScore(GameMode,currentScore);
        }
    }
}
