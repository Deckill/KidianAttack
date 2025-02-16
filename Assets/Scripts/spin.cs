using UnityEngine;

public class spin : MonoBehaviour
{
    public float rotationDuration = 0.2f; // n초 동안 회전
    private float rotationSpeed = 0f;

    private void Start()
    {
        // 초당 회전 속도 설정 (360도를 rotationDuration 동안 회전)
        rotationSpeed = 360f / rotationDuration;
    }
    private void Update()
    {
        // 초당 rotationSpeed만큼 Z축으로 회전
        transform.Rotate(0, 0,- rotationSpeed * Time.deltaTime);
    }
}
