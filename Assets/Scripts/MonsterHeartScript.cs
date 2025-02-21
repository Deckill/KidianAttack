using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterHeartScript : MonoBehaviour
{
    float gravity = -9.8f*3f;
    Vector3 currentVelocity = Vector3.zero;
    bool isFlying=false;
    public float forcePower = 5f; // 초기 힘 크기
    public float torquePower = 2f; // 회전력 크기

    //private Rigidbody2D rb;

    void Start()
    {
    }
    void Update(){
        if(isFlying){
            gameObject.transform.position +=currentVelocity*Time.deltaTime;
            currentVelocity+=gravity*Vector3.up*Time.deltaTime;
        }
        if(gameObject.transform.position.y<0f){
            Destroy(gameObject);
        }
    }

    public void FlyAway(Vector3 direction)
    {
        gameObject.transform.SetParent(null);
        isFlying=true;
        currentVelocity = (Random.insideUnitCircle+(Vector2)direction)*10;
    }
}
