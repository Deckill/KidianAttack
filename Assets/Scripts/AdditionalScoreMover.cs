using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AdditionalScoreMover : MonoBehaviour
{
    public float floatSpeed = 50f; // 위로 올라가는 속도
    public float duration = 1.5f;  // 사라지는 시간

    private TMP_Text textMesh;
    private Color startColor;
    private float currentTime=0;
    // Start is called before the first frame update
    void Start()
    {
        textMesh = GetComponent<TMP_Text>();
        startColor = textMesh.color;
        Destroy(gameObject, duration); // 일정 시간이 지나면 삭제
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime+=Time.deltaTime;
        transform.position += Vector3.up * floatSpeed * Time.deltaTime; // 위로 이동
        float alpha = Mathf.Lerp(1f, 0f, currentTime / duration); // 서서히 투명해짐
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
        
    }
}
