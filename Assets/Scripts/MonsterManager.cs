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
    public float bossTime=600000f;
    float currentTime;
    public GameObject dmgEffectPrefab;
    public List<Vector3> spawnAnchor;
    public GameObject monsterPosArrowPrefab;
    public GameObject jibMunSeo;
    public GameObject heartPrefab;
    public SceneManagerMK2 sceneManagerMK2;
    public float monsterAdjustment=1f;

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
            if(PlayerPrefs.GetInt("GameMode")==0){
                monsterAdjustment=1.8f*Mathf.Exp(0.007f*Time.time)-1.8f;
            }else if(PlayerPrefs.GetInt("GameMode")==1){
                monsterAdjustment=1.6f*Mathf.Exp(0.012f*Time.time)-1.6f;
            }
            if(currentTime>a*Mathf.Exp(-1*b*Time.time)+c){
                float randY = Random.Range(-1f, 1f)*gameObject.transform.position.y*2/3;
                Vector3 extraVector = new Vector3(0, randY,2);
                Instantiate(monsters[Random.Range(0,monsters.Count-1)],spawnAnchor[Random.Range(0,spawnAnchor.Count)] +extraVector,quaternion.identity,gameObject.transform);
                currentTime=0;
            }
        }else{

        }
    }
}
