using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterScript : MonoBehaviour
{
    public float size = 0.5f;
    public int maxHealth;
    public int currentHealth;
    public float gravity=-29.4f;
    public float baseSpeed = 3f; // 기본 왼쪽 이동 속도
    public float hitSpeed=3f;
    public float hitDeccelerate=1f;
    public int jibMunSeoDmg=1;
    public GameObject target;
    public Vector3 baseVelocity;
    public AudioScript audioScript;
    private Vector3 currentVelocity;
    private Vector3 extraVelocity;
    private bool isAlive=true;
    private GameObject monsterPosArrow;
    private GameObject jibMunSeo;
    private GameObject heartPrefab;
    private List<GameObject> hearts=new List<GameObject>();
    public float jubMunSeoFollowRadius=20f;
    private float rotationDeathSpeed=0;
    
    // Start is called before the first frame update
    void Start()
    {
        target = gameObject.transform.parent.gameObject;
        currentHealth=maxHealth;
        // 기본 왼쪽 이동 속도 설정
        baseVelocity = (target.transform.position-gameObject.transform.position).normalized * baseSpeed;
        currentVelocity = baseVelocity;
        monsterPosArrow= Instantiate(gameObject.transform.parent.GetComponent<MonsterManager>().monsterPosArrowPrefab,gameObject.transform);
        jibMunSeo=gameObject.transform.parent.GetComponent<MonsterManager>().jibMunSeo;
        heartPrefab = gameObject.transform.parent.GetComponent<MonsterManager>().heartPrefab;
        for(int i=0;i<maxHealth;i+=1){
            
            hearts.Add(Instantiate(heartPrefab,gameObject.transform));
        }
        audioScript = gameObject.transform.parent.GetComponent<MonsterManager>().sceneManagerMK2.GetComponent<AudioScript>();
        UpdateHeartPos(Random.insideUnitCircle);
    }

    // Update is called once per frame
    void Update()
    {
        if(isAlive){
            if(baseVelocity.x<0){
                gameObject.transform.localScale=new Vector3(1f,1f,1f)*size;
            }else{
                gameObject.transform.localScale=new Vector3(-1f,1f,1f)*size;
            }
            // 추가 속도 적용
            currentVelocity = baseVelocity + extraVelocity;
            //Debug.Log("current Vel: "+currentVelocity);
            extraVelocity =Vector3.MoveTowards(extraVelocity,Vector3.zero,hitDeccelerate* Time.deltaTime);
            
            if(gameObject.transform.position.y>30&&extraVelocity.y >0){
                extraVelocity*=-1;
            }
            if(Vector2.Distance(gameObject.transform.position,jibMunSeo.transform.position)<3f){
                jibMunSeo.GetComponent<JibMunSeoScript>().TakeDamage(jibMunSeoDmg);
                Death();
            }
        }else{
            currentVelocity += gravity*Vector3.up*Time.deltaTime;
            gameObject.transform.Rotate(0,0,-rotationDeathSpeed);
        }
        
        // 이동 적용
        transform.position += currentVelocity * Time.deltaTime;
        if(gameObject.transform.position.y<0){
            Death();
        }
        if(Vector2.Distance(gameObject.transform.position,jibMunSeo.transform.position)<jubMunSeoFollowRadius){
            baseVelocity = (jibMunSeo.transform.position-gameObject.transform.position).normalized*baseSpeed/3f;
        }
        PositionArrow();
    }
    /// <summary>
    /// 공격을 받았을때 부르는 함수
    /// hitVelocity 만큼 밀려난다
    /// </summary>
    /// <param name="damage"></param> 몹이 받는 피해량
    /// <param name="normalDirection"></param> 몹이 밀려나는 방향
    public void TakeDamage(int damage,Vector3 normalDirection,Vector3 hitPoint){
        audioScript.PlayDamageSound();
        extraVelocity += normalDirection.normalized * hitSpeed;
        currentHealth-=damage;
        currentHealth = Mathf.Clamp(currentHealth, 0,maxHealth);
        UpdateHeartPos(normalDirection);
        Destroy(Instantiate(gameObject.transform.parent.GetComponent<MonsterManager>().dmgEffectPrefab,gameObject.transform.position,quaternion.identity),1f);
        if(currentHealth<=0){
            rotationDeathSpeed=Vector3.Cross(normalDirection,(hitPoint-gameObject.transform.position)).z/gameObject.transform.localScale.x;
            rotationDeathSpeed=Mathf.Clamp(rotationDeathSpeed, 0,1f);
            //Debug.Log("rotatie"+(hitPoint-gameObject.transform.position)+", speed"+rotationDeathSpeed);
            
            currentVelocity = extraVelocity.normalized*40f;
            isAlive=false;
        }

    }
    public void UpdateHeartPos(Vector3 direction){
        for(int i=0;i<currentHealth;i+=1){
            hearts[i].transform.localPosition = new Vector3(-3f+(i+1f)/(currentHealth+1f)*6f,3f,0.01f*i);
        }
        for(int i=hearts.Count-1;i>=currentHealth;i-=1){
            hearts[i].GetComponent<MonsterHeartScript>().FlyAway(direction*2f);
            hearts.RemoveAt(i);
            // hearts[i].SetActive(false);
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
        // if(gameObject==gameObject.transform.parent.GetComponent<MonsterManager>().sceneManagerMK2.activeTarget){
        //     gameObject.transform.parent.GetComponent<MonsterManager>().sceneManagerMK2.activeTarget=null;
        // }
        gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        jibMunSeoDmg=0;
        //enabled=false;
        //gameObject.transform.parent.GetComponent<MonsterManager>().sceneManagerMK2.activeTarget=null;
        Destroy(gameObject,0.1f);
        
    }
}
