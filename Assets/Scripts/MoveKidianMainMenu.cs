using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveKidianMainMenu : MonoBehaviour
{
    public RectTransform imageTransform; // UI 이미지의 RectTransform
    public float speed = 300f; // 이동 속도

    private Vector2 direction; // 현재 이동 방향
    private RectTransform canvasRect; // UI 캔버스 크기 참조

    void Start()
    {
        if (imageTransform == null)
            imageTransform = GetComponent<RectTransform>();

        canvasRect = imageTransform.root.GetComponent<Canvas>().GetComponent<RectTransform>();

        // 초기 이동 방향을 랜덤하게 설정
        direction = Random.insideUnitCircle.normalized;
    }

    void Update()
    {
        MoveAndBounce();
        RotateTowardsDirection();
    }

    void MoveAndBounce()
    {
        // 현재 위치 업데이트
        Vector2 newPos = imageTransform.anchoredPosition + direction * speed * Time.deltaTime;
        Vector2 halfSize = imageTransform.sizeDelta * 0.5f;
        Vector2 canvasSize = canvasRect.sizeDelta * 0.5f;

        bool hitHorizontal = false;
        bool hitVertical = false;

        // X축 충돌 체크
        if (newPos.x - halfSize.x <= -canvasSize.x || newPos.x + halfSize.x >= canvasSize.x)
        {
            hitHorizontal = true;
        }

        // Y축 충돌 체크
        if (newPos.y - halfSize.y <= -canvasSize.y || newPos.y + halfSize.y >= canvasSize.y)
        {
            hitVertical = true;
        }

        // 만약 벽에 닿았다면 방향 변경
        if (hitHorizontal || hitVertical)
        {
            direction = GetRandomDirection(hitHorizontal, hitVertical);
        }

        // 최종 위치 적용
        imageTransform.anchoredPosition += direction * speed * Time.deltaTime;
    }

    void RotateTowardsDirection()
    {
        // 이동 방향을 바라보도록 회전 (Z축 회전)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        imageTransform.rotation = Quaternion.Euler(0, 0, angle+90);
    }

    Vector2 GetRandomDirection(bool flipX, bool flipY)
    {
        // 기존 방향을 유지하면서 새로운 랜덤 방향 생성
        Vector2 newDirection = Random.insideUnitCircle.normalized;

        // X 반사가 필요하면 기존 X 방향 반전
        if (!flipX) newDirection.x = Mathf.Sign(direction.x) * Mathf.Abs(newDirection.x);
        else newDirection.x = -direction.x;

        // Y 반사가 필요하면 기존 Y 방향 반전
        if (!flipY) newDirection.y = Mathf.Sign(direction.y) * Mathf.Abs(newDirection.y);
        else newDirection.y = -direction.y;

        return newDirection.normalized;
    }
}
