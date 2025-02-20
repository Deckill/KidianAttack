using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterManager : MonoBehaviour
{
    public List<GameObject> monsters;
    public List<GameObject> bosses;
    public float a=5;
    public float b=0.02f;
    public float c=1;
    public float bossTime=60f;
    float currentTime;
    public GameObject dmgEffectPrefab;
    public GameObject monsterPosArrowPrefab;

    // Start is called before the first frame update
    void Start()
    {
        currentTime=100f;
    }

    // Update is called once per frame
    void Update()
    {
        currentTime+=Time.deltaTime;
        if(Time.time<bossTime){
            if(currentTime>a*Mathf.Exp(-1*b*Time.time)+c){
                float randY = Random.Range(-1f, 1f)*gameObject.transform.position.y*2/3;
                Vector3 extraVector = new Vector3(0, randY,2);
                Instantiate(monsters[Random.Range(0,monsters.Count-1)],gameObject.transform.position+extraVector,quaternion.identity,gameObject.transform);
                currentTime=0;
            }
        }else{

        }
    }
}
