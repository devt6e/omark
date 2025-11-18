using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainListUI : MonoBehaviour
{
    [Header("Create Popup")]
    public GameObject popupCreateRoom;     // Popup_CreateRoom
    public TMP_InputField inputRoomName;   // Popup_CreateRoom/Box/Row_Input/Input
    public Button btnCancel;               // Popup_CreateRoom/Box/Row_Buttons/Btn_Cancel
    public Button btnOK;                   // Popup_CreateRoom/Box/Row_Buttons/Btn_OK

    [Header("List Targets (Normal)")]
    public Transform listContent;          // Panel_ListNormal/Scroll_List/Viewport/Content
    public GameObject roomItemPrefab;      // Prefabs/RoomItem

    [Header("Open Button")]
    public Button btnOpenCreatePopup;      // Panel_ListNormal/Row_Actions/Btn_Create

    [Header("Edit Mode UI")]
    public GameObject panelListNormal;     // Panel_ListNormal
    public GameObject panelListEdit;       // Panel_ListEdit
    public Transform listContentEdit;      // Panel_ListEdit/Scroll_List/Viewport/Content
    public Button btnOpenEdit;             // Panel_ListNormal/Row_Actions/Btn_EditList
    public Button btnCloseEdit;            // Panel_ListEdit/Row_Actions/Btn_Back
    public Button btnDeleteChecked;        // Panel_ListEdit/Row_Actions/Btn_Delete

    [Header("Empty Text")]
    public GameObject txtEmpty;            // "리스트가 비어있습니다" 안내 텍스트

    [Header("Delete Confirm Popup")]
    public GameObject popupConfirm;        // Popup_Confirm
    public Button popupConfirmOk;          // Popup_Confirm/Box/Btn_OK
    public Button popupConfirmCancel;      // Popup_Confirm/Box/Btn_Cancel
    public TMP_Text popupConfirmTitle;     // Popup_Confirm/Box/Txt_Title

    enum DeleteMode
    {
        None,
        Single,
        Multiple
    }

    DeleteMode pendingDeleteMode = DeleteMode.None;
    readonly List<GameObject> pendingDeleteEditItems = new List<GameObject>();
    readonly Dictionary<GameObject, GameObject> editToNormal = new Dictionary<GameObject, GameObject>();

    void Awake()
    {
        if (popupCreateRoom) popupCreateRoom.SetActive(false);
        if (popupConfirm) popupConfirm.SetActive(false);

        if (btnOpenCreatePopup) btnOpenCreatePopup.onClick.AddListener(OpenCreatePopup);
        if (btnCancel) btnCancel.onClick.AddListener(CloseCreatePopup);
        if (btnOK) btnOK.onClick.AddListener(CreateRoom);

        if (btnOpenEdit) btnOpenEdit.onClick.AddListener(OpenEditMode);
        if (btnCloseEdit) btnCloseEdit.onClick.AddListener(CloseEditMode);
        if (btnDeleteChecked) btnDeleteChecked.onClick.AddListener(OnClickDeleteChecked);

        if (popupConfirmOk) popupConfirmOk.onClick.AddListener(OnClickConfirmDelete);
        if (popupConfirmCancel) popupConfirmCancel.onClick.AddListener(CloseConfirmPopup);
    }

    void Start()
    {
        RefreshEmptyText();
    }

    // ------------------- 새 공간 생성 -------------------

    void OpenCreatePopup()
    {
        if (!popupCreateRoom) return;
        popupCreateRoom.SetActive(true);

        if (inputRoomName)
        {
            inputRoomName.text = "";
            inputRoomName.ActivateInputField();
        }
    }

    void CloseCreatePopup()
    {
        if (!popupCreateRoom) return;
        popupCreateRoom.SetActive(false);
    }

    void CreateRoom()
    {
        if (!roomItemPrefab || !listContent) return;

        string nameToUse = "새 공간";
        if (inputRoomName && !string.IsNullOrWhiteSpace(inputRoomName.text))
            nameToUse = inputRoomName.text.Trim();

        GameObject go = Instantiate(roomItemPrefab, listContent);
        go.name = $"RoomItem_{nameToUse}";

        SetRoomItemMode(go, false);
        ApplyRoomItemTexts(go, nameToUse, GetNowDateString());

        CloseCreatePopup();
        RefreshEmptyText();

        // ★ 추가/생성 후 레이아웃 강제 재계산
        RebuildListsNow();
    }

    // ------------------- 편집 모드 전환 -------------------

    void OpenEditMode()
    {
        SyncEditList();

        if (panelListNormal) panelListNormal.SetActive(false);
        if (panelListEdit) panelListEdit.SetActive(true);

        // ★ 표시 전환 뒤에도 재빌드
        RebuildListsNow();
    }

    void CloseEditMode()
    {
        if (panelListEdit) panelListEdit.SetActive(false);
        if (panelListNormal) panelListNormal.SetActive(true);

        FixAllNormalRoomItemMode();   // ★ Normal 화면에서는 항상 Group_Normal만 보이도록
        RefreshEmptyText();

        // ★ 전환 후 재빌드
        RebuildListsNow();
    }

    // Normal 리스트 → Edit 리스트 복제
    void SyncEditList()
    {
        if (!listContent || !listContentEdit) return;

        editToNormal.Clear();

        for (int i = listContentEdit.childCount - 1; i >= 0; i--)
        {
            Destroy(listContentEdit.GetChild(i).gameObject);
        }

        foreach (Transform normalItem in listContent)
        {
            GameObject clone = Instantiate(normalItem.gameObject, listContentEdit);
            SetRoomItemMode(clone, true);

            var toggle = clone.transform.Find("Group_Edit/Toggle_Select")?.GetComponent<Toggle>();
            if (toggle != null) toggle.isOn = false;

            editToNormal[clone] = normalItem.gameObject;

            var btnSingleDelete = clone.transform.Find("Group_Edit/Btn_Delete")?.GetComponent<Button>();
            if (btnSingleDelete != null)
            {
                btnSingleDelete.onClick.RemoveAllListeners();
                GameObject captured = clone;
                btnSingleDelete.onClick.AddListener(() => OnClickDeleteSingle(captured));
            }
        }

        // ★ 복제 작업 후 레이아웃 재빌드
        RebuildListsNow();
    }

    // ------------------- 삭제 요청 -------------------

    void OnClickDeleteChecked()
    {
        if (!listContentEdit) return;

        pendingDeleteEditItems.Clear();

        foreach (Transform editItem in listContentEdit)
        {
            var toggle = editItem.transform.Find("Group_Edit/Toggle_Select")?.GetComponent<Toggle>();
            if (toggle != null && toggle.isOn)
            {
                pendingDeleteEditItems.Add(editItem.gameObject);
            }
        }

        if (pendingDeleteEditItems.Count == 0) return;

        OpenConfirmPopup(DeleteMode.Multiple);
    }

    void OnClickDeleteSingle(GameObject editItem)
    {
        if (!editItem) return;

        pendingDeleteEditItems.Clear();
        pendingDeleteEditItems.Add(editItem);

        OpenConfirmPopup(DeleteMode.Single);
    }

    void OpenConfirmPopup(DeleteMode mode)
    {
        pendingDeleteMode = mode;

        if (popupConfirmTitle)
        {
            popupConfirmTitle.text =
                (mode == DeleteMode.Single)
                    ? "이 공간을 삭제하시겠습니까?"
                    : "선택한 공간들을 삭제하시겠습니까?";
        }

        if (popupConfirm) popupConfirm.SetActive(true);
    }

    // ------------------- 삭제 확정 / 취소 -------------------

    void OnClickConfirmDelete()
    {
        if (pendingDeleteMode == DeleteMode.None)
        {
            CloseConfirmPopup();
            return;
        }

        foreach (GameObject editItem in pendingDeleteEditItems)
        {
            if (!editItem) continue;

            if (editToNormal.TryGetValue(editItem, out GameObject normalItem))
            {
                if (normalItem) Destroy(normalItem);     // 원본 RoomItem 전체 제거
                editToNormal.Remove(editItem);
            }

            Destroy(editItem);                            // 편집용 복제본 RoomItem 전체 제거
        }

        pendingDeleteEditItems.Clear();
        pendingDeleteMode = DeleteMode.None;

        CloseConfirmPopup();

        if (HasAnyRoom())
        {
            SyncEditList();      // 남은 것들 기준으로 편집 화면 다시 그림
        }
        else
        {
            CloseEditMode();     // 아무 것도 없으면 편집 화면 종료 + Txt_Empty 표시
        }

        RefreshEmptyText();

        // ★ 삭제 후 레이아웃 재빌드
        RebuildListsNow();
    }

    void CloseConfirmPopup()
    {
        if (popupConfirm) popupConfirm.SetActive(false);
        pendingDeleteMode = DeleteMode.None;
        pendingDeleteEditItems.Clear();
    }

    // ------------------- 빈 상태 안내 텍스트 -------------------

    bool HasAnyRoom()
    {
        return listContent && listContent.childCount > 0;
    }

    void RefreshEmptyText()
    {
        if (!txtEmpty) return;

        bool isEmpty = !listContent || listContent.childCount == 0;
        txtEmpty.SetActive(isEmpty);
    }

    // Normal 리스트의 모든 RoomItem 을 "보기 모드"로 맞춤
    void FixAllNormalRoomItemMode()
    {
        if (!listContent) return;

        foreach (Transform tr in listContent)
        {
            SetRoomItemMode(tr.gameObject, false);
        }
    }

    // ------------------- RoomItem 헬퍼 -------------------

    static string GetNowDateString()
    {
        return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }

    public static void SetRoomItemMode(GameObject roomItem, bool isEdit)
    {
        if (!roomItem) return;

        var t = roomItem.transform;
        var groupNormal = t.Find("Group_Normal");
        var groupEdit = t.Find("Group_Edit");

        if (groupNormal) groupNormal.gameObject.SetActive(!isEdit);
        if (groupEdit) groupEdit.gameObject.SetActive(isEdit);
    }

    public static void ApplyRoomItemTexts(GameObject roomItem, string name, string date)
    {
        if (!roomItem) return;

        SetIfExist(roomItem.transform, "Group_Normal/Col_Texts/Txt_Name", name);
        SetIfExist(roomItem.transform, "Group_Normal/Col_Texts/Txt_Date", date);

        SetIfExist(roomItem.transform, "Group_Edit/Col_Texts/Txt_Name", name);
        SetIfExist(roomItem.transform, "Group_Edit/Col_Texts/Txt_Date", date);
    }

    static void SetIfExist(Transform root, string path, string text)
    {
        var tr = root.Find(path);
        if (!tr) return;

        var tmp = tr.GetComponent<TMP_Text>();
        if (tmp)
        {
            tmp.text = text;
            return;
        }

        var uText = tr.GetComponent<Text>();
        if (uText)
        {
            uText.text = text;
            return;
        }
    }

    // ------------------- ★ 레이아웃 강제 재빌드 -------------------
    void RebuildListsNow()
    {
        // 한 번 강제 계산
        if (listContent) LayoutRebuilder.ForceRebuildLayoutImmediate(listContent as RectTransform);
        if (listContentEdit) LayoutRebuilder.ForceRebuildLayoutImmediate(listContentEdit as RectTransform);

        // Canvas 갱신 후 한 번 더 (UI가 즉시 반영되도록)
        Canvas.ForceUpdateCanvases();
        if (listContent) LayoutRebuilder.ForceRebuildLayoutImmediate(listContent as RectTransform);
        if (listContentEdit) LayoutRebuilder.ForceRebuildLayoutImmediate(listContentEdit as RectTransform);
    }
}
