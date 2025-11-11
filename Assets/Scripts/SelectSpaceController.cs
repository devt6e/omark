using UnityEngine;
using UnityEngine.EventSystems;
public class SelectSpaceController : MonoBehaviour,  IPointerDownHandler, IPointerUpHandler
{
    private bool isPressed = false;
    private Camera uiCam;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        uiCam = GetComponentInParent<Canvas>().worldCamera; // Canvas의 UI 카메라 참조
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        Debug.Log($"{name} : Pointer Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isPressed) return;
        isPressed = false;

        // 터치 위치가 RectTransform 내부에 있는지 확인
        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, eventData.position, uiCam))
        {
            Debug.Log($"{name} : Pointer Up (내부 클릭 판정)");

            ActivatePanel();
        }
        else
        {
            Debug.Log($"{name} : Pointer Up (영역 밖)");
        }
    }
    private void ActivatePanel()
    {
        if (SelectSpaceManager.Instance != null && SelectSpaceManager.Instance.targetPanel != null)
            SelectSpaceManager.Instance.targetPanel.SetActive(true);
    }
}
/*
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // 이미 구현되어 있는 패널 활성화 함수 연결
    [SerializeField] private GameObject targetPanel;

    private Camera mainCam;
    private bool isPointerDown = false;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isPointerDown) return;
        isPointerDown = false;

        // 스크린 좌표를 월드 좌표로 Ray 변환
        Ray ray = mainCam.ScreenPointToRay(eventData.position);
        RaycastHit hit;

        // 3D 콜라이더가 붙은 경우
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                // 클릭 해제 지점이 자신 콜라이더 내부일 때
                ActivatePanel();
            }
        }

        // (2D 콜라이더 사용 시)
        
        RaycastHit2D hit2D = Physics2D.GetRayIntersection(ray);
        if (hit2D.collider != null && hit2D.collider.gameObject == this.gameObject)
        {
            ActivatePanel();
        }
        
    }

    private void ActivatePanel()
    {
        if (targetPanel != null)
        {
            targetPanel.SetActive(true);
            Debug.Log("패널 활성화됨");
        }
    }
}


*/