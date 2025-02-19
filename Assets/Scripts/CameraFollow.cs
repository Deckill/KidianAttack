using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public bool yMinBool=true;
    public bool yMaxBool=true;
    public bool xMinBool=true;
    public bool xMaxBool=false;
    public float yMin=0;
    public float yMax=16;
    public float xMin=0;
    public float xMax=16;
    // Start is called before the first frame update
    void Start()
    {
        if(!yMinBool){
            yMin = int.MinValue;
        }
        if(!yMaxBool){
            yMax = int.MaxValue;
        }
        if(!xMinBool){
            xMin = int.MinValue;
        }
        if(!xMaxBool){
            xMax = int.MaxValue;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.y>yMin&&gameObject.transform.position.y<yMax&&gameObject.transform.position.x>xMin&&gameObject.transform.position.x<xMax){
            Camera.main.transform.SetParent(gameObject.transform,true);
            //Camera.main.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y,-10);
        }else{
            Camera.main.transform.SetParent(gameObject.transform.parent,true);
            Vector3 pos = gameObject.transform.position;
            pos.x = Mathf.Clamp(pos.x,xMin,xMax);
            pos.y = Mathf.Clamp(pos.y,yMin,yMax);
            pos.z=-10f;
            Camera.main.transform.position = pos;
        }
        
    }
}
