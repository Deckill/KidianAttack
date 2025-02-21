using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class JibMunSeoScript : MonoBehaviour
{
    public List<Sprite> jibMunSeoSprites ;
    int maxHealth;
    public int currentHealth;
    public SceneManagerMK2 sceneManagerMK2;
    // Start is called before the first frame update
    void Start()
    {
        maxHealth = jibMunSeoSprites.Count-1;
        currentHealth=maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPos = gameObject.transform.position;
        currentPos.y = Mathf.Sin(Time.time)*0.5f+5f;
        gameObject.transform.position = currentPos;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateSprite();
        if(currentHealth<=0){
            sceneManagerMK2.GameOver();
        }
    }

    void UpdateSprite()
    {
        int index = Mathf.Clamp(maxHealth-currentHealth, 0, maxHealth);
        gameObject.GetComponent<SpriteRenderer>().sprite = jibMunSeoSprites[index];
    }
}
