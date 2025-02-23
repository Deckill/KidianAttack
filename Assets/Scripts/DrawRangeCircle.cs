using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRangeCircle : MonoBehaviour
{
    public int segments = 100;  // 원의 부드러움 정도 (점 개수)
    public SceneManagerMK2 sceneManagerMK2;
    private float radius; // 원의 반지름
    private LineRenderer lineRenderer;

    void Start()
    {
        radius = sceneManagerMK2.maxSkillDistance;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;  // 점 개수 (마지막 점은 시작점과 동일)
        lineRenderer.loop = true;  // 원을 닫기 위해 루프 설정

        Draw();
    }

    void Draw()
    {
        float angle = 0f;
        for (int i = 0; i < segments + 1; i++)
        {
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            angle += (2f * Mathf.PI) / segments;
        }
    }
}
