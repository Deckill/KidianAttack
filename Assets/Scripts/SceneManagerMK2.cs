using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System.Resources;

public class SceneManagerMK2 : MonoBehaviour
{
    public LineRenderer straightLineRenderer;
    public LineRenderer gravityLineRenderer;
    public GameObject debugObj1;
    public GameObject debugObj2;
    public GameObject kidian;
    public GameObject kidianObj;
    public GameObject greyScreen;
    public GameObject ultFillButton;
    public GameObject monsterController;
    public LayerMask targetLayer;      // 타겟 레이어
    public GameObject activeTarget;
    public InGameMenuScript mainMenuScript;
    public GraphicRaycaster uiRaycaster; // UI의 GraphicRaycaster 연결 필요
    private PointerEventData pointerEventData;
    public float currentTime;
    public float hitTime;
    public string currentState;

    public bool isInvulnerability=false;

    public bool isHardMode=false;
    public bool canUlt=false;
    private bool usedUltPrevious=false;
    private float rotationSpeedMultiplier;
    private float straightSkillSpeed = 50f;  // 직선 속도
    private float straightUltSpeed = 150f;  // 직선 속도
    private float gravitySpeed = 15f;    // 반사 후 시작 속도
    private float gravity = -9.8f;      // 중력 가속도
    public float maxSkillDistance = 15f;  
    // public float maxMaxSkillDistance = 15f;  // 최대 거리
    private float maxUltDistance = 10000f;

    private Vector3 initialVelocity; //시작 속도도
    private Vector3 reflectionPoint; //
    private Vector3 direction;
    private Vector3 reflectionDirection;
    private Vector3 oldReflectionDirection;
    private Vector3 reflectRotationSpeedVector;
    private Vector3 startPos;
    private Vector3 mousePos;
    private Vector3 endPos;
    private bool isHit;
    private int damage=0;
    int invulnerabilityFrame=0;
    RaycastHit2D hit;
    RotateKidian rotateKidian;
    KidianController kidianController;

    public int ultScore=10;
    
    AudioScript audioScript;
    int ultCounter=0;
    public InGameMenuScript inGameMenuScript;
    public GameObject jibMunSeo;
    void Start(){
        if(PlayerPrefs.GetInt("GameMode",0)==1){
            isHardMode = true;
        }else{
            isHardMode=false;
        }
        currentTime=0;
        hitTime=0;
        currentState="Idle";
        greyScreen.SetActive(false);
        straightLineRenderer.positionCount=2;
        gravityLineRenderer.positionCount = 0;
        rotateKidian = kidianObj.GetComponent<RotateKidian>();
        kidianController = kidianObj.GetComponent<KidianController>();
        audioScript = gameObject.GetComponent<AudioScript>();
        pointerEventData = new PointerEventData(EventSystem.current);
    }
    void Update(){
        currentTime+=Time.deltaTime;
        switch (currentState)
        {
            case "Idle":{
                if(Input.GetMouseButtonDown(0)&&!IsPointerOverUI()){
                    currentState="SkillReady";
                    currentTime=0;
                }else if(Input.GetMouseButtonDown(1)&&canUlt&&!IsPointerOverUI()){
                    currentState="UltReady";
                    currentTime=0;
                }
            }break;

            case "SkillReady":{//스킬 차징 시전전
                kidianController.chargeEffectPre.SetActive(true);
                Time.timeScale = 0.025f;
                isInvulnerability=true;
                //audioScript.ChangeBGMPitch(0.5f);
                //maxSkillDistance=1f;
                reflectRotationSpeedVector=Vector3.zero;
                currentState="SkillReadyLoop";
                currentTime=0;
                ultScore=10;
                if(usedUltPrevious&&isHardMode){
                    usedUltPrevious=false;
                    ultCounter=0;
                    UpdateUlt();
                }
                if(activeTarget!=null&&activeTarget.GetComponent<MonsterScript>().isAlive){
                    activeTarget.GetComponent<CapsuleCollider2D>().enabled = true;
                }
                SetAnimation("Charge");
            }break;

            case "SkillReadyLoop":{//스킬 차징 중
                MoveKidian();
                if(Input.GetMouseButtonUp(0)){
                    currentState="Skill";
                    currentTime=0;
                }
                //maxSkillDistance = Mathf.Clamp(maxSkillDistance+0.1f,0,maxMaxSkillDistance);
                gravityLineRenderer.positionCount = 0;
                
                startPos=kidian.transform.position;
                mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                DrawStraightTrajectory(startPos,mousePos,maxSkillDistance);
                
                DrawGravityTrajectory();
                rotateKidian.RotateToMouse();
            }break;

            case"Skill":{//스킬 발사! 직선선
                kidianController.chargeEffectPre.SetActive(false);
                kidianController.chargeEffect.SetActive(true);
                isInvulnerability=true;
                invulnerabilityFrame=0;
                if(activeTarget!=null){
                    activeTarget.GetComponent<CapsuleCollider2D>().enabled = false;
                }
                Time.timeScale = 1f;
                //audioScript.ChangeBGMPitch(1f);
                greyScreen.SetActive(false);
                oldReflectionDirection=reflectionDirection;
                currentState="SkillLoop";
                currentTime=0;
                damage=1;
                audioScript.PlayEffectSound();
                SetAnimation("Fly");
                rotateKidian.RotateToMouseInstant();
            }break;

            case"SkillLoop":{//스킬 발사! 직선선
                float deltaDistance = straightSkillSpeed * Time.deltaTime;
                Vector3 hitPoint;
                
                if(isHit){
                    hitPoint=reflectionPoint;
                    
                }else{
                    hitPoint=endPos;
                }
                kidian.transform.position = Vector3.MoveTowards(kidian.transform.position,hitPoint,deltaDistance);
                if(Vector3.Distance(kidian.transform.position,hitPoint)<deltaDistance*1.5f){
                    currentState="Reflected";
                    currentTime=0;
                    kidian.transform.position=hitPoint;
                    currentTime=0;
                }
            }break;

            case"UltReady":{//궁 시전 준비비
                SetKidianTransparency(0f);
                ultCounter=0;
                canUlt=false;
                usedUltPrevious=true;
                ultFillButton.GetComponent<Image>().fillAmount=ultCounter/8f;
                if(activeTarget!=null){
                    activeTarget.GetComponent<CapsuleCollider2D>().enabled = true;
                }
                isInvulnerability=true;
                Time.timeScale = 0.025f;
                //audioScript.ChangeBGMPitch(0.5f);
                currentState="UltReadyLoop";
            }break;
            case"UltReadyLoop":{//궁 시전 준비비
                if(Input.GetMouseButtonUp(1)||Input.GetMouseButtonUp(0)){
                    currentState="Ult";
                    currentTime=0;
                }
                gravityLineRenderer.positionCount = 0;
                
                startPos=Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Screen.height,0));
                endPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,0,0));
                DrawStraightTrajectory(startPos,endPos,maxUltDistance);
                DrawGravityTrajectory();
                rotateKidian.RotateToMouse();
            }break;

            case"Ult":{//궁 발사!
                Time.timeScale = 1f;
                //audioScript.ChangeBGMPitch(1f);
                SetKidianTransparency(1f);
                kidianController.chargeEffectPre.SetActive(false);
                if(activeTarget!=null&&activeTarget.GetComponent<MonsterScript>().isAlive){
                    activeTarget.GetComponent<CapsuleCollider2D>().enabled = false;
                }
                GameObject ultWindEffect = Instantiate(kidianController.ultWindEffect);
                ultWindEffect.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,25,1.5f);
                ultWindEffect.transform.localScale = new Vector3(3, 3,1);
                Destroy(ultWindEffect,1f);
                GameObject ultEffect = Instantiate(kidianController.ultEffect);
                ultEffect.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,30,1.5f);
                ultEffect.transform.localScale = new Vector3(3, 15,1);
                Destroy(ultEffect,1f);

                kidian.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Screen.height,0));
                if(activeTarget!=null){
                    damage=2;
                    ultEffect.transform.localScale 
                    = new Vector3(3,(Screen.height-Camera.main.WorldToScreenPoint(activeTarget.transform.position).y)/Screen.height*15f,1);

                }else{
                    ultEffect.transform.localScale = new Vector3(3,15f,1);
                }
                isInvulnerability=true;
                invulnerabilityFrame=0;
                audioScript.PlayEffectSound();
                greyScreen.SetActive(false);
                oldReflectionDirection=reflectionDirection;
                currentState="UltLoop";
                currentTime=0;
                SetAnimation("Ult");
                
                rotateKidian.RotateToMouseInstant();

                currentState="UltLoop";
            }break;

            case"UltLoop":{//궁 발사!
                // Time.timeScale = 1f;
                // //audioScript.ChangeBGMPitch(1f);

                float deltaDistance = straightUltSpeed * Time.deltaTime;
                Vector3 hitPoint;
                
                if(isHit){
                    hitPoint=reflectionPoint;
                    
                }else{
                    hitPoint=endPos;
                }
                kidian.transform.position = Vector3.MoveTowards(kidian.transform.position,hitPoint,deltaDistance);
                if(Vector3.Distance(kidian.transform.position,hitPoint)<deltaDistance*1.5f){
                    currentState="Reflected";
                    currentTime=0;
                    kidian.transform.position=hitPoint;
                    ultCounter=0;
                    currentTime=0;
                }
            }break;

            case"Reflected":{//반사 또는 스킬궁 끝난 후에 일어나는 일
                SetAnimation("Spin");
                if(direction.x>0){
                    kidianObj.transform.Rotate(0,0,-90);
                }else{
                    kidianObj.transform.Rotate(0,0,90);
                }
                MoveKidian();
                //Debug.Log(activeTarget);
                if(!ReferenceEquals(activeTarget,null)){
                    try{
                        activeTarget.GetComponent<MonsterScript>().TakeDamage(damage,-hit.normal,hit.point);
                    }catch{
                        activeTarget=null;
                    }
                    if(damage==2){//궁극기 사용용
                        inGameMenuScript.AddScore(ultScore);
                        ultScore+=10;
                        UltControll(true);
                    }else{//일반 스킬 사용용
                        inGameMenuScript.AddScore(10);
                        UltControll(false);
                    }
                }
                currentState="ReflectedLoop";
                currentTime=0;
            }break;

            case"ReflectedLoop":{//반사 또는 스킬궁 끝난 후에 일어나는 일 루프프
                // if(currentTime>invulnerabilityTime){
                //     isInvulnerability=false;
                // }
                invulnerabilityFrame+=1;
                if(invulnerabilityFrame==8){
                    isInvulnerability=false;
                }
                kidianController.chargeEffect.SetActive(false);

                MoveKidian();
                if(Input.GetMouseButtonDown(0)&&!IsPointerOverUI()){
                    if(!(isHardMode&&!isHit)){
                        currentState="SkillReady";
                        currentTime=0;
                    }else{

                    }
                }else if(Input.GetMouseButtonDown(1)&&canUlt&&!IsPointerOverUI()){
                    currentState="UltReady";
                    currentTime=0;
                }
                rotateKidian.RotateAuto(reflectRotationSpeedVector.z*rotationSpeedMultiplier);
            }break;
            case "Dead":{
                GameOver();


                currentState="DeadLoop";
            }break;
            case "DeadLoop":{

            }break;
            default:{

            }break;
        }
    }

    bool IsPointerOverUI()
    {
        // 마우스 위치 설정
        pointerEventData.position = Input.mousePosition;
        
        // UI 그래픽 요소 검사
        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(pointerEventData, results);
        
        return results.Count > 0; // UI 요소가 감지되면 true 반환
    }
    private void UltControll(bool ultUsed)
    {
        //궁으로 죽이면 한번 더더
        if(activeTarget!=null&&activeTarget.GetComponent<MonsterScript>().currentHealth<=0&&ultUsed){
            ultCounter=8;
        }
        ultCounter+=damage;
        UpdateUlt();
    }
    void UpdateUlt(){
        if(ultCounter>8){
            ultCounter=8;
        }
        if(ultCounter>=8){
            canUlt=true;
        }else{
            canUlt=false;
        }
        ultFillButton.GetComponent<Image>().fillAmount=ultCounter/8f;
    }

    void MoveKidian(){
        Vector3 gravityVelocity = oldReflectionDirection*gravitySpeed;
        gravityVelocity.y+=gravity*currentTime;
        kidian.transform.position+=gravityVelocity*Time.deltaTime;
    }
    //직선 궤적 그리기
    void DrawStraightTrajectory(Vector3 startPos, Vector3 targetPos, float maxDistance){
        greyScreen.SetActive(true);
        //startPos=kidian.transform.position;
        //mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        startPos.z=0;
        targetPos.z=0;
        direction = (targetPos-startPos).normalized;
        endPos = startPos + direction*maxDistance;
        //debugObj1.transform.position=endPos;
        hit = Physics2D.Raycast(startPos,direction,maxDistance,targetLayer);
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
    }
    void SetKidianTransparency(float alpha){
        Color newColor = kidianObj.GetComponent<SpriteRenderer>().color;
        newColor.a = alpha;
        kidianObj.GetComponent<SpriteRenderer>().color = newColor;
    }
    public void GameOver(){
        Destroy(jibMunSeo);
        kidianObj.SetActive(false);
        monsterController.SetActive(false);
        inGameMenuScript.ShowFailScreen();
    }
    public void UltButton(){
        if(canUlt&&currentState=="ReflectedLoop"){
            currentState="UltReady";
            currentTime=0;
        }
    }
}
