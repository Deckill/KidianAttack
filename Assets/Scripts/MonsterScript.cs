using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterScript : MonoBehaviour
{
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
    // Start is called before the first frame update
    void Start()
    {
        currentHealth=maxHealth;
        // 기본 왼쪽 이동 속도 설정
        currentVelocity = Vector3.left * baseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if(isAlive){
            // 추가 속도 적용
            currentVelocity = Vector3.left * baseSpeed + extraVelocity;
            //Debug.Log("current Vel: "+currentVelocity);
            extraVelocity =Vector3.MoveTowards(extraVelocity,Vector3.zero,hitDeccelerate* Time.deltaTime);
        }else{
            currentVelocity += gravity*Vector3.up*Time.deltaTime;
        }
        
        // 이동 적용
        transform.position += currentVelocity * Time.deltaTime;
        if(gameObject.transform.position.y<0){
            Destroy(gameObject);
        }
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
        if(currentHealth<=0){
            Death();
        }
    }
    public void Death(){
        //gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        currentVelocity = extraVelocity.normalized*40f;
        isAlive=false;
    }
}
