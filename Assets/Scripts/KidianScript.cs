using UnityEngine;

public class KidianController : MonoBehaviour
{
    public LayerMask targetLayer; // 타겟 레이어 설정
    public SceneManagerMK2 sceneManagerMK2;
    public GameObject chargeEffectPre;
    public GameObject chargeEffect;
    public GameObject ultWindEffect;
    public GameObject ultEffect;
    public GameObject hitEffect;
    public GameObject kidianPosArrow;

    public int kidianHealth=3;
    void Start(){
        chargeEffect.SetActive(false);
        chargeEffectPre.SetActive(false);
    }
    void Update()
    {
        // 캐릭터의 중심점 좌표
        Vector2 centerPoint = transform.position;

        // 중심점에 콜라이더가 있는지 확인
        Collider2D hit = Physics2D.OverlapPoint(centerPoint, targetLayer);

        if (hit != null&&!sceneManagerMK2.isInvulnerability)
        {
            Debug.Log("타겟과 충돌!"+hit);
            sceneManagerMK2.isInvulnerability=true;
            kidianHealth-=1;
        }
        if(gameObject.transform.position.y<0){
            kidianHealth=0;
        }
        if(kidianHealth<=0){
            sceneManagerMK2.currentState="Dead";
            Debug.Log("Game Over");
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        bool isOffScreen = screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height;

        if (isOffScreen)
        {
            kidianPosArrow.SetActive(true);

            // 화면 중심 기준 적 방향 벡터
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 direction = new Vector2(screenPos.x, screenPos.y) - screenCenter;
            direction = new Vector2(Mathf.Clamp(direction.x, -Screen.width/2f, Screen.width/2f),
                                         Mathf.Clamp(direction.y, -Screen.height/2f,Screen.height/2f));
            // 화면 가장자리 좌표 설정
            Vector2 arrowScreenPos = screenCenter + direction *0.9f;

            // 스크린 좌표 → 월드 좌표 변환
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(arrowScreenPos.x, arrowScreenPos.y, Camera.main.nearClipPlane));
            kidianPosArrow.transform.position = new Vector3(worldPos.x, worldPos.y, 0);

            // 화살표가 적을 가리키도록 회전
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            kidianPosArrow.transform.rotation = Quaternion.Euler(0, 0, angle-90);
            kidianPosArrow.transform.localScale =Vector3.one*gameObject.transform.localScale.y*5f;
        }
        else
        {
            kidianPosArrow.SetActive(false);
        }
    }
}
