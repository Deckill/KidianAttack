using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float yMin=0;
    public float yMax=16;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.y>yMin&&gameObject.transform.position.y<yMax){
            Camera.main.transform.SetParent(gameObject.transform,true);
            //Camera.main.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y,-10);
        }else{
            Camera.main.transform.SetParent(gameObject.transform.parent,true);
            if(gameObject.transform.position.y<=yMin){
                Camera.main.transform.position = new Vector3(gameObject.transform.position.x, yMin,-10);
            }else{
                Camera.main.transform.position = new Vector3(gameObject.transform.position.x, yMax,-10);
            }
        }
        
    }
}
