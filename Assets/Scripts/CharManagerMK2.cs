using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChargeManagerMK2 : MonoBehaviour
{
    public LineRenderer straightLineRenderer;
    public LineRenderer gravityLineRenderer;
    public GameObject debugObj1;
    public GameObject debugObj2;
    [SerializeField] string currentState;
    public GameObject kidian;
    public GameObject kidianObj;
    public GameObject greyScreen;
    public float rotationSpeedMultiplier;
    public float straightSpeed = 50f;  // 직선 속도
    public float gravitySpeed = 15f;    // 반사 후 시작 속도
    public float gravity = -9.8f;      // 중력 가속도
    public float maxDistance = 15f;    // 최대 거리
    public LayerMask targetLayer;      // 타겟 레이어

    private Vector3 initialVelocity; //시작 속도도
    private Vector3 reflectionPoint; //
    private Vector3 direction;
    private Vector3 reflectionDirection;
    private Vector3 oldReflectionDirection;
    private Vector3 reflectRotationSpeedVector;
    private Vector3 startPos;
    private Vector3 mousePos;
    private Vector3 endPos;
    private float currentTime;
    private GameObject activeTarget;
    private bool isHit;
    RotateKidian rotateKidian;
    void Start(){
        currentTime=0;
        currentState="Idle";
        greyScreen.SetActive(false);
        straightLineRenderer.positionCount=2;
        gravityLineRenderer.positionCount = 0;
        rotateKidian = kidianObj.GetComponent<RotateKidian>();
    }
    void Update(){
        currentTime+=Time.deltaTime;
        switch (currentState)
        {
            case "Idle":{
                if(Input.GetMouseButtonDown(0)){
                    currentState="SkillReady";
                }else if(Input.GetMouseButtonDown(1)){
                    currentState="UltReady";
                }
            }break;

            case "SkillReady":{//스킬 차징중중
                Time.timeScale = 0.025f;
                reflectRotationSpeedVector=Vector3.zero;
                currentState="SkillReadyLoop";
                SetAnimation("charge");
            }break;

            case "SkillReadyLoop":{//스킬 차징중중
                MoveKidian();
                if(Input.GetMouseButtonUp(0)){
                    currentState="Skill";
                }else if(Input.GetMouseButtonDown(1)){
                    currentState="UltReady";
                }
                gravityLineRenderer.positionCount = 0;
                DrawStraightTrajectory();
                DrawGravityTrajectory();
                rotateKidian.RotateToMouse();
            }break;

            case"Skill":{//스킬 발사! 직선선
                Time.timeScale = 1f;
                greyScreen.SetActive(false);
                oldReflectionDirection=reflectionDirection;
                currentState="SkillLoop";
                SetAnimation("fly");
                rotateKidian.RotateToMouseInstant();
            }break;

            case"SkillLoop":{//스킬 발사! 직선선
                float deltaDistance = straightSpeed * Time.deltaTime;
                Vector3 hitPoint;
                
                if(isHit){
                    hitPoint=reflectionPoint;
                    
                }else{
                    hitPoint=endPos;
                }
                kidian.transform.position = Vector3.MoveTowards(kidian.transform.position,hitPoint,deltaDistance);
                if(Vector3.Distance(kidian.transform.position,hitPoint)<deltaDistance*1.5f){
                    currentState="Reflected";
                    kidian.transform.position=hitPoint;
                    currentTime=0;
                }
            }break;

            case"UltReady":{//궁 시전 준비비
                Time.timeScale = 0.025f;
                if(Input.GetMouseButtonUp(1)){
                    currentState="Ult";
                }
            }break;

            case"Ult":{//궁 발사!
                Time.timeScale = 1f;

            }break;
            case"Reflected":{//반사 또는 스킬궁 끝난 후에 일어나는 일
                SetAnimation("spin");
                if(direction.x>0){
                    kidianObj.transform.Rotate(0,0,-90);
                }else{
                    kidianObj.transform.Rotate(0,0,90);
                }
                currentState="ReflectedLoop";
            }break;

            case"ReflectedLoop":{//반사 또는 스킬궁 끝난 후에 일어나는 일 루프프
                MoveKidian();
                if(Input.GetMouseButtonDown(0)){
                    currentState="SkillReady";
                }else if(Input.GetMouseButtonDown(1)){
                    currentState="UltReady";
                }
                rotateKidian.RotateAuto(reflectRotationSpeedVector.z*rotationSpeedMultiplier);
            }break;

            default:{

            }break;
        }
    }
    void MoveKidian(){
        Vector3 gravityVelocity = oldReflectionDirection*gravitySpeed;
        gravityVelocity.y+=gravity*currentTime;
        kidian.transform.position+=gravityVelocity*Time.deltaTime;
    }
    //직선 궤적 그리기
    void DrawStraightTrajectory(){
        greyScreen.SetActive(true);
        startPos=kidian.transform.position;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z=0;
        direction = (mousePos-startPos).normalized;
        endPos = startPos + direction*maxDistance;
        //debugObj1.transform.position=endPos;
        RaycastHit2D hit = Physics2D.Raycast(startPos,direction,maxDistance,targetLayer);
        if(activeTarget!=null){//타겟을 회색 스크린 뒤로 뺌뺌
            activeTarget.transform.position =new Vector3(activeTarget.transform.position.x, activeTarget.transform.position.y,2);
        }
        if(hit.collider !=null){//타겟이 직선상에 있으면면
            activeTarget=hit.collider.gameObject;
            isHit=true;
            activeTarget.transform.position=new Vector3(activeTarget.transform.position.x, activeTarget.transform.position.y,0);
            reflectionPoint = hit.point;
            reflectionDirection = Vector2.Reflect(direction,hit.normal);
            reflectRotationSpeedVector = Vector3.Cross(direction,hit.normal);
            straightLineRenderer.SetPosition(0, startPos);
            straightLineRenderer.SetPosition(1, reflectionPoint);
        }else{//없으면면
            activeTarget=null;
            isHit=false;
            reflectionPoint=endPos;
            reflectionDirection=direction;
            straightLineRenderer.SetPosition(0, startPos);
            straightLineRenderer.SetPosition(1, endPos);
        }
    }
        // 반사 궤적 그리기 (중력 가속도 적용)
    void DrawGravityTrajectory()
    {
        List<Vector3> gravityPoints = new List<Vector3>();
        Vector3 startPosition = reflectionPoint;
        Vector3 velocity = reflectionDirection * gravitySpeed;
        float timeStep = 0.1f;

        for (float t = 0; t < 2f; t += timeStep)
        {
            Vector3 point = startPosition + (velocity * t);
            point.y += 0.5f * gravity * t * t;
            gravityPoints.Add(point);
        }

        gravityLineRenderer.positionCount = gravityPoints.Count;
        gravityLineRenderer.SetPositions(gravityPoints.ToArray());
    }
    void SetAnimation(string state){
        kidianObj.GetComponent<Animator>().Play(state);
        // switch(state){
        //     case "spin":{
        //         kidianObj.GetComponent<Animator>().SetInteger("state",0);
        //     }break;
        //     case "charge":{
        //         kidianObj.GetComponent<Animator>().SetInteger("state",1);
        //     }break;
        //     case "fly":{
        //         kidianObj.GetComponent<Animator>().SetInteger("state",2);
        //     }break;
        //     default:{

        //     }break;
        // }
    }
}
