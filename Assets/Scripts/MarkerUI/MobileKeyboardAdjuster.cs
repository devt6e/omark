using UnityEngine;
using System.Collections;
using TMPro; // TMP_InputField를 사용한다면 필요합니다.

public class MobileKeyboardAdjuster : MonoBehaviour
{
    // [Inspector 연결]
    [Header("조정 대상 UI 연결")]
    // 키보드 높이만큼 위로 올려야 할 UI 요소 (예: Footer 패널, 편집 패널 전체)
    public RectTransform adjustTarget;

    [Header("조정 설정")]
    // 키보드 높이를 감지할 때 여유 공간을 줄 값 (픽셀)
    public float verticalPadding = 20f;
    // UI 이동 속도 (부드럽게 이동하도록 Lerp 사용)
    public float moveSpeed = 10f;

    // 내부 상태 변수
    private Vector2 originalPosition;
    private RectTransform canvasRect;

    void Start()
    {
        // 조정 대상이 없으면 스크립트 중지
        if (adjustTarget == null)
        {
            Debug.LogError("Adjust Target RectTransform이 연결되지 않았습니다. 스크립트를 비활성화합니다.");
            enabled = false;
            return;
        }

        // Canvas RectTransform 가져오기
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        // 원래 위치 저장
        originalPosition = adjustTarget.anchoredPosition;

        // 코루틴 시작: 매 프레임 키보드 상태 감지
        StartCoroutine(CheckKeyboardVisibility());
    }

    private IEnumerator CheckKeyboardVisibility()
    {
        while (true)
        {
            yield return null; // 다음 프레임까지 대기

            // 현재 키보드 높이를 OS에서 가져옵니다.
            float keyboardHeightInPixels = GetKeyboardHeight();

            // 키보드가 띄워졌는지 확인
            if (keyboardHeightInPixels > 0)
            {
                // 캔버스 크기를 기준으로 키보드 높이를 조정
                float targetY = keyboardHeightInPixels + verticalPadding;

                // 새로운 위치로 이동
                Vector2 targetPosition = new Vector2(originalPosition.x, targetY);

                // 부드럽게 이동 (Lerp 사용)
                adjustTarget.anchoredPosition = Vector2.Lerp(
                    adjustTarget.anchoredPosition,
                    targetPosition,
                    Time.deltaTime * moveSpeed
                );
            }
            else
            {
                // 키보드가 숨겨진 상태: 원래 위치로 복귀
                adjustTarget.anchoredPosition = Vector2.Lerp(
                    adjustTarget.anchoredPosition,
                    originalPosition,
                    Time.deltaTime * moveSpeed * 2f // 복귀는 더 빠르게
                );
            }
        }
    }

    // 모바일 OS에서 키보드 높이를 가져오는 함수
    private float GetKeyboardHeight()
    {
        // 1. Android/iOS에서만 실행되도록 조건부 컴파일 사용
        #if UNITY_ANDROID || UNITY_IOS
            
            // iOS에서는 Screen.safeArea를 사용하여 키보드 높이를 추정합니다.
            // Android에서는 TouchScreenKeyboard.area를 사용합니다.

            // TouchScreenKeyboard.area는 Unity에서 키보드 영역을 추적하는 방법입니다.
            if (TouchScreenKeyboard.visible)
            {
                // 픽셀 단위의 키보드 높이를 반환
                return TouchScreenKeyboard.area.height; 
            }
            
        #endif

        // Editor 또는 키보드가 숨겨진 경우
        return 0f;
    }
}