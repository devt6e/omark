using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class MarkerListSlider : MonoBehaviour, IDragHandler, IEndDragHandler
{
    // [Inspector 연결]
    [Header("슬라이딩 패널 설정")]
    public RectTransform panelRect; // 슬라이딩할 마커 리스트 패널의 RectTransform
    public float transitionSpeed = 15f; // 이동 속도
    public float dragThreshold = 0.05f; // 드래그를 인정할 최소 비율 (화면 높이의 5%)

    // 내부 상태 변수
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private float hiddenY;     // 숨겨진 상태의 Y 앵커 위치
    private float visibleY;    // 보이는 상태의 Y 앵커 위치
    private bool isVisible = false; // 현재 패널 상태

    void Start()
    {
        if (panelRect == null)
        {
            panelRect = GetComponent<RectTransform>();
            if (panelRect == null)
            {
                Debug.LogError("패널 RectTransform 연결 필수!");
                enabled = false;
                return;
            }
        }

        // 1. 상태 Y 위치 계산
        // VisibleY: 화면 하단에 딱 붙어있을 때 (Y 앵커 = 0, 보통 0 또는 약간 위)
        visibleY = 0f;

        // HiddenY: 패널 높이만큼 아래로 내려가 숨겨졌을 때 (패널 높이의 마이너스 값)
        hiddenY = -panelRect.rect.height;

        // 2. 초기 위치 설정 (숨겨진 상태)
        targetPosition = new Vector2(panelRect.anchoredPosition.x, hiddenY);
        panelRect.anchoredPosition = targetPosition;
    }

    void Update()
    {
        // 3. 목표 위치로 부드럽게 이동
        panelRect.anchoredPosition = Vector2.Lerp(
            panelRect.anchoredPosition,
            targetPosition,
            Time.deltaTime * transitionSpeed
        );
    }

    // 드래그 중 호출 (IDragHandler)
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Drag Input Received!");
        // Y축 드래그만 허용하며, 터치 위치를 따라 이동
        float newY = panelRect.anchoredPosition.y + eventData.delta.y;

        // 이동 범위 제한: 완전히 숨겨진 상태(hiddenY)와 완전히 보이는 상태(visibleY) 사이
        newY = Mathf.Clamp(newY, hiddenY, visibleY);

        panelRect.anchoredPosition = new Vector2(panelRect.anchoredPosition.x, newY);

        // 드래그 중에는 Lerp 이동을 잠시 멈춥니다.
        targetPosition = panelRect.anchoredPosition;
    }

    // 드래그 종료 시 호출 (IEndDragHandler)
    public void OnEndDrag(PointerEventData eventData)
    {
        // 화면 높이를 기준으로 드래그 된 거리를 계산
        float screenHeight = Screen.height;
        float dragDistance = eventData.position.y - eventData.pressPosition.y; // 총 드래그된 픽셀 거리
        float dragRatio = Mathf.Abs(dragDistance) / screenHeight;

        // 1. 임계값(Threshold)을 넘었는지 확인 (의도된 드래그인지)
        if (dragRatio >= dragThreshold)
        {
            // 2. 최종 목표 상태 결정
            if (dragDistance > 0) // 위로 드래그 (보이게)
            {
                targetPosition = new Vector2(panelRect.anchoredPosition.x, visibleY);
                isVisible = true;
            }
            else // 아래로 드래그 (숨기게)
            {
                targetPosition = new Vector2(panelRect.anchoredPosition.x, hiddenY);
                isVisible = false;
            }
        }
        else // 임계값을 넘지 못했다면 (약한 드래그)
        {
            // 가장 가까운 상태로 복귀
            if (panelRect.anchoredPosition.y > hiddenY + (visibleY - hiddenY) / 2f)
            {
                targetPosition = new Vector2(panelRect.anchoredPosition.x, visibleY);
                isVisible = true;
            }
            else
            {
                targetPosition = new Vector2(panelRect.anchoredPosition.x, hiddenY);
                isVisible = false;
            }
        }
    }
}