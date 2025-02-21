using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ChargeManager : MonoBehaviour
{
    public LineRenderer straightLineRenderer;
    public LineRenderer gravityLineRenderer;
    public Button ultButton;
    public Transform moverObject;
    public GameObject greyScreen;
    public float straightSpeed = 50f;  // 직선 속도
    public float gravitySpeed = 15f;    // 반사 후 시작 속도
    public float gravity = -9.8f;      // 중력 가속도
    public float maxDistance = 15f;    // 최대 거리
    public LayerMask targetLayer;      // 타겟 레이어

    private Vector3 initialVelocity; //시작 속도도
    private Vector3 reflectionPoint; //
    private Vector3 reflectionDirection;
    private Vector3 oldReflectionDirection;
    private Vector3 endPos;
    private bool isDrawing = false;
    private bool isMoving = false;
    private bool isReflected = false;
    private bool isHit=false;
    private bool isClickable = true;
    private float reflectionTime = 0;
    GameObject activeTarget;

    void Start()
    {
        ultButton.onClick.AddListener(OnButtonClicked);
        greyScreen.SetActive(false);
        straightLineRenderer.positionCount = 0;
        gravityLineRenderer.positionCount = 0;
    }

    void Update()
    {
        // 1. 마우스를 누를 때 직선 궤적 그리기 시작
        if (Input.GetMouseButtonDown(0)&&isClickable)
        {
            isHit=false;
            isDrawing = true;
            isMoving = false;
            Time.timeScale = 0.025f;
            straightLineRenderer.positionCount = 0;
            gravityLineRenderer.positionCount = 0;
        }

        // 2. 마우스를 누른 상태에서 빨간색 직선 궤적 그리기
        if (isDrawing)
        {
            greyScreen.SetActive(true);
            Vector3 startPos = moverObject.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            Vector3 direction = (mousePos - startPos).normalized;
            endPos = startPos + direction * maxDistance;

            RaycastHit2D hit = Physics2D.Raycast(startPos, direction, maxDistance, targetLayer);
            if(activeTarget!=null){
                activeTarget.transform.position =new Vector3(activeTarget.transform.position.x, activeTarget.transform.position.y,0);
            }
            // 직선 궤적 그리기
            if (hit.collider != null)
            {
                activeTarget=hit.collider.gameObject;
                activeTarget.transform.position =new Vector3(activeTarget.transform.position.x, activeTarget.transform.position.y,-2);
                
                isHit=true;
                reflectionPoint = hit.point;
                reflectionDirection = Vector2.Reflect(direction, hit.normal);
                straightLineRenderer.positionCount = 2;
                straightLineRenderer.SetPosition(0, startPos);
                straightLineRenderer.SetPosition(1, reflectionPoint);

                DrawGravityTrajectory();
            }
            else
            {
                activeTarget=null;
                isHit=false;
                reflectionPoint=endPos;
                reflectionDirection=direction;
                straightLineRenderer.positionCount = 2;
                straightLineRenderer.SetPosition(0, startPos);
                straightLineRenderer.SetPosition(1, endPos);
                DrawGravityTrajectory();
            }

        }

        // 3. 마우스를 떼면 궤적을 따라 이동 시작
        if (Input.GetMouseButtonUp(0))
        {
            greyScreen.SetActive(false);
            oldReflectionDirection=reflectionDirection;
            isReflected=false;
            isDrawing = false;
            isMoving = true;
            Time.timeScale = 1f;
        }

        // 4. 직선 궤적 이동
        if (isMoving && !isReflected)
        {
            float deltaDistance = straightSpeed * Time.deltaTime;
            if(isHit){
                moverObject.position = Vector3.MoveTowards(moverObject.position,reflectionPoint,deltaDistance);

                // 반사 포인트에 도달 시 반사 운동 시작
                if (Vector3.Distance(moverObject.position, reflectionPoint) < deltaDistance*1.5f)
                {
                    isReflected = true;
                    reflectionTime = 0;
                    moverObject.position=reflectionPoint;
                }
            }else{
                moverObject.position = Vector3.MoveTowards(moverObject.position,endPos,deltaDistance);

                // 반사 포인트에 도달 시 반사 운동 시작
                if (Vector3.Distance(moverObject.position, endPos) < deltaDistance*1.5f)
                {
                    isReflected = true;
                    reflectionTime = 0;
                    moverObject.position=endPos;
                }
            }
        }

        // 5. 반사 궤적 이동 (중력 가속도 적용)
        if ( isReflected)
        {
            reflectionTime += Time.deltaTime;
            Vector3 gravityVelocity = oldReflectionDirection * gravitySpeed;
            gravityVelocity.y += gravity * reflectionTime;
            moverObject.position += gravityVelocity * Time.deltaTime;
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
    public void OnButtonClicked(){}
}
