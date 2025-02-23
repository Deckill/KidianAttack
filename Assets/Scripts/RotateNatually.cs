using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateNatually : MonoBehaviour
{
    public float initialAngle = 0;
    public float speed = 1;
    public bool isPeriodic;
    public float period=5;
    private float currentTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.Rotate(0,0,initialAngle);
    }

    // Update is called once per frame
    void Update()
    {
        currentTime+=Time.deltaTime;
        if(isPeriodic){
            gameObject.transform.rotation = Quaternion.Euler(0,0,initialAngle+speed*Mathf.Sin(currentTime*2*Mathf.PI/period));
            if(currentTime>period){
                gameObject.transform.rotation = Quaternion.Euler(0,0,initialAngle);
                currentTime=0;
            }
        }else{
            gameObject.transform.Rotate(0,0,speed*Time.deltaTime);
        }
    }
}
