using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MonsterScript : MonoBehaviour
{
    public GameObject dmgEffectPrefab;
    public int maxHealth;
    public int currentHealth;
    public float gravity=-9.8f;
    public float baseSpeed = 3f; // 기본 왼쪽 이동 속도
    public float hitSpeed=3f;
    public float hitDeccelerate=1f;
    private Vector3 currentVelocity;
    private Vector3 extraVelocity;
    private float extraVelocityDuration;
    private float extraVelocityTimer;
    private bool isAlive=true;
    private GameObject monsterPosArrow;
    
    // Start is called before the first frame update
    void Start()
    {
        currentHealth=maxHealth;
        // 기본 왼쪽 이동 속도 설정
        currentVelocity = Vector3.left * baseSpeed;
        monsterPosArrow= Instantiate(gameObject.transform.parent.GetComponent<MonsterManager>().monsterPosArrowPrefab,gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if(isAlive){
            // 추가 속도 적용
            currentVelocity = Vector3.left * baseSpeed + extraVelocity;
            //Debug.Log("current Vel: "+currentVelocity);
            extraVelocity =Vector3.MoveTowards(extraVelocity,Vector3.zero,hitDeccelerate* Time.deltaTime);
            
            if(gameObject.transform.position.y>30&&extraVelocity.y >0){
                extraVelocity*=-1;
            }
        }else{
            currentVelocity += gravity*Vector3.up*Time.deltaTime;
        }
        
        // 이동 적용
        transform.position += currentVelocity * Time.deltaTime;
        if(gameObject.transform.position.y<0){
            Destroy(gameObject);
        }

        PositionArrow();
    }
    /// <summary>
    /// 공격을 받았을때 부르는 함수
    /// hitVelocity 만큼 밀려난다
    /// </summary>
    /// <param name="damage"></param> 몹이 받는 피해량
    /// <param name="normalDirection"></param> 몹이 밀려나는 방향
    public void TakeDamage(int damage,Vector3 normalDirection){
        
        extraVelocity += normalDirection.normalized * hitSpeed;
        extraVelocityDuration = hitSpeed/hitDeccelerate;
        currentHealth-=damage;
        Destroy(Instantiate(gameObject.transform.parent.GetComponent<MonsterManager>().dmgEffectPrefab,gameObject.transform.position,quaternion.identity),1f);
        if(currentHealth<=0){
            Death();
        }
    }

    public void PositionArrow(){
        
        Vector3 screenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        bool isOffScreen = screenPos.x < 0 || screenPos.x > Screen.width || screenPos.y < 0 || screenPos.y > Screen.height;

        if (isOffScreen)
        {
            monsterPosArrow.SetActive(true);

            // 화면 중심 기준 적 방향 벡터
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 direction = new Vector2(screenPos.x, screenPos.y) - screenCenter;
            direction = new Vector2(Mathf.Clamp(direction.x, -Screen.width/2f, Screen.width/2f),
                                         Mathf.Clamp(direction.y, -Screen.height/2f,Screen.height/2f));
            // 화면 가장자리 좌표 설정
            Vector2 arrowScreenPos = screenCenter + direction *0.9f;

            // 스크린 좌표 → 월드 좌표 변환
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(arrowScreenPos.x, arrowScreenPos.y, Camera.main.nearClipPlane));
            monsterPosArrow.transform.position = new Vector3(worldPos.x, worldPos.y, 0);

            // 화살표가 적을 가리키도록 회전
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            monsterPosArrow.transform.rotation = Quaternion.Euler(0, 0, angle-90);
        }
        else
        {
            monsterPosArrow.SetActive(false);
        }
    }
    public void Death(){
        //gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        currentVelocity = extraVelocity.normalized*40f;
        isAlive=false;
    }
}
