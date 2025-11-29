using UnityEngine;

public class UIManager : MonoBehaviour
{
    // 팝업 그룹 전체 (모달 배경 포함)를 연결할 변수
    [SerializeField]
    private GameObject deleteConfirmationPanel;

    // 1. '삭제' 버튼 클릭 시 호출
    public void OpenDeleteConfirmation()
    {
        if (deleteConfirmationPanel != null)
        {
            // 패널 활성화 (모달 배경이 켜져 클릭 차단 시작)
            deleteConfirmationPanel.SetActive(true);
        }
    }

    // 2. '예' 버튼 클릭 시 호출
    public void ConfirmDelete()
    {
        // **[여기에 마커 삭제 로직 추가]**
        Debug.Log("마커를 실제로 삭제합니다.");

        // 팝업 패널 비활성화 (클릭 차단 해제)
        if (deleteConfirmationPanel != null)
        {
            deleteConfirmationPanel.SetActive(false);
        }
    }

    // 3. '아니오' 버튼 클릭 시 호출
    public void CancelDelete()
    {
        // 팝업 패널 비활성화 (클릭 차단 해제)
        if (deleteConfirmationPanel != null)
        {
            deleteConfirmationPanel.SetActive(false);
        }
    }
}