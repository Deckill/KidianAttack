using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

public class RotateKidian : MonoBehaviour
{
    public float maxTime;
    void Update()
    {
        //RotateGun();
    }
    public void RotateToMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector3 direction = mousePosition - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 부드러운 회전 적용
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, targetAngle), Time.deltaTime/maxTime);
        
        // Flip 적용
        if(mousePosition.x < transform.position.x){
            gameObject.transform.localScale=new Vector3(-1,-1,1)*0.5f;
        }else{
            gameObject.transform.localScale=new Vector3(-1,1,1)*0.5f;
        }
    }
    public void RotateToMouseInstant(){
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector3 direction = mousePosition - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 부드러운 회전 적용
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
        
        // Flip 적용
        if(mousePosition.x < transform.position.x){
            gameObject.transform.localScale=new Vector3(-1,-1,1)*0.5f;
        }else{
            gameObject.transform.localScale=new Vector3(-1,1,1)*0.5f;
        }
        
    }
    public void RotateAuto(float rotationSpeed){
        gameObject.transform.Rotate(0,0,-rotationSpeed*Time.deltaTime);
    }
}
