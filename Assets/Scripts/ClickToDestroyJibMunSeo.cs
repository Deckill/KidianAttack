using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClickToDestroyJibMunSeo : MonoBehaviour
{
    public Image targetImage; // 변경할 UI Image
    public Sprite[] sprites;  // 변경할 스프라이트 배열
    private int currentIndex = 0;

    public void ChangeSprite()
    {
        if (sprites.Length == 0) return;

        currentIndex = (currentIndex + 1) % sprites.Length; // 순환 인덱스
        targetImage.sprite = sprites[currentIndex];
    }
}
