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

    }
}
