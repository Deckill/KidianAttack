using UnityEngine;

public class CenterCollisionCheck : MonoBehaviour
{
    public LayerMask targetLayer; // 타겟 레이어 설정

    void Update()
    {
        // 캐릭터의 중심점 좌표
        Vector2 centerPoint = transform.position;

        // 중심점에 콜라이더가 있는지 확인
        Collider2D hit = Physics2D.OverlapPoint(centerPoint, targetLayer);

        if (hit != null)
        {
            Debug.Log("타겟과 충돌!"+hit);
        }
    }
}
