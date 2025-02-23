using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private static List<string> SCORE_KEYS = new List<string>{"highScore0","highScore1","highScore2","highScore3","highScore4","highScore5"};
    private const int ENCRYPTION_KEY = 20211003; // XOR 키 (랜덤한 정수)

    public static void SaveScore(int gameModeIndex,int score)
    {
        int encryptedScore = score ^ ENCRYPTION_KEY; // XOR 암호화
        PlayerPrefs.SetInt(SCORE_KEYS[gameModeIndex], encryptedScore);
        PlayerPrefs.Save();
    }

    public static int LoadScore(int gameModeIndex)
    {
        if (PlayerPrefs.HasKey(SCORE_KEYS[gameModeIndex]))
        {
            int encryptedScore = PlayerPrefs.GetInt(SCORE_KEYS[gameModeIndex]);
            return encryptedScore ^ ENCRYPTION_KEY; // XOR 복호화
        }else{
            SaveScore(gameModeIndex,0);
        }
        return 0; // 기본값
    }
}
