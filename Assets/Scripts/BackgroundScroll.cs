using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    public List<float> scrollSpeeds = new List<float>(new float[4]); // 배경 스크롤 속도
    private List<Material> backgroundMaterials = new List<Material>(new Material[4]);
    private List<GameObject> backgroundGameObjects = new List<GameObject>(new GameObject[4]);
    private List<Vector2> offsets = new List<Vector2>(new Vector2[4]);

    void Start()
    {
        // Sprite Renderer에서 Material 가져오기
        for (int i = 0; i < backgroundMaterials.Count; i+=1){
            backgroundMaterials[i]= gameObject.transform.GetChild(i).GetComponent<Renderer>().material;
            backgroundGameObjects[i]= gameObject.transform.GetChild(i).gameObject;
        }
    }

    void Update()
    {
        //카메라의 x만 따라다님님
        gameObject.transform.position = new Vector3(Camera.main.transform.position.x,gameObject.transform.position.y,gameObject.transform.position.z);
        // X 방향으로만 Offset 이동
        for (int i = 0;i<offsets.Count;i+=1){
            offsets[i]=Vector2.right*scrollSpeeds[i]*Camera.main.transform.position.x/640f;
            backgroundMaterials[i].mainTextureOffset = offsets[i];
        } 
    }
}
