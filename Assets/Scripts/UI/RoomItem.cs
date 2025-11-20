using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RoomItem : MonoBehaviour
{
    // ============================================================
    //  Backend 정보 (필수)
    // ============================================================
    public long environmentId;     // 서버에서 받은 공간 ID
    public string s3FileUrl;       // 서버에서 받은 S3 파일 URL (null 가능)

    // ============================================================
    //  Normal / Edit UI 그룹
    // ============================================================
    [Header("Groups")]
    public GameObject groupNormal;
    public GameObject groupEdit;

    // ============================================================
    //  Normal Mode UI
    // ============================================================
    [Header("Normal Mode")]
    public TMP_Text txtNameNormal;
    public TMP_Text txtDateNormal;
    public Button btnPlay;         // 클릭 시 sample 씬 이동

    // ============================================================
    //  Edit Mode UI
    // ============================================================
    [Header("Edit Mode")]
    public TMP_InputField inputNameEdit;
    public TMP_Text txtDateEdit;
    public Button btnDeleteEdit;   // 단일 삭제 요청 버튼
    public Toggle toggleSelect;    // 일괄 삭제용 체크박스

    // 삭제 요청 콜백
    private Action<RoomItem> onDeleteRequest;

    // ============================================================
    //  초기 텍스트 설정
    // ============================================================
    public void SetTexts(string name, string date)
    {
        if (txtNameNormal) txtNameNormal.text = name;
        if (txtDateNormal) txtDateNormal.text = date;

        if (inputNameEdit) inputNameEdit.text = name;
        if (txtDateEdit) txtDateEdit.text = date;
    }

    // ============================================================
    //  편집모드에서 입력한 이름 반환
    // ============================================================
    public string GetEditedName()
    {
        return inputNameEdit != null ? inputNameEdit.text : "";
    }

    // 편집모드 종료 시 Normal 표시에도 반영
    public void ApplyEditedNameToNormal()
    {
        if (txtNameNormal && inputNameEdit)
            txtNameNormal.text = inputNameEdit.text;
    }

    // ============================================================
    //  Normal/Edit 전환
    // ============================================================
    public void SetEditMode(bool isEdit)
    {
        if (groupNormal) groupNormal.SetActive(!isEdit);
        if (groupEdit) groupEdit.SetActive(isEdit);
    }

    // ============================================================
    //  토글 기능
    // ============================================================
    public void SetToggle(bool on)
    {
        if (toggleSelect != null)
            toggleSelect.isOn = on;
    }

    public bool IsSelected()
    {
        return toggleSelect != null && toggleSelect.isOn;
    }

    // ============================================================
    //  삭제 요청 콜백 연결
    // ============================================================
    public void SetDeleteAction(Action<RoomItem> callback)
    {
        onDeleteRequest = callback;

        if (btnDeleteEdit != null)
        {
            btnDeleteEdit.onClick.RemoveAllListeners();
            btnDeleteEdit.onClick.AddListener(() =>
            {
                onDeleteRequest?.Invoke(this);
            });
        }
    }

    // ============================================================
    //  Btn_Play 클릭 시 sample 씬 이동
    // ============================================================
    private void Start()
    {
        if (btnPlay != null)
        {
            btnPlay.onClick.RemoveAllListeners();
            btnPlay.onClick.AddListener(() =>
            {
                // SampleSceneLoader(다음 단계에서 제작)에 전달
                SampleSceneLoader.Load(environmentId, s3FileUrl);

                // 씬 이동
                UnityEngine.SceneManagement.SceneManager.LoadScene("sample");
            });
        }
    }
}
