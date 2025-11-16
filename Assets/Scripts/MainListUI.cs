using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainListUI : MonoBehaviour
{
    [Header("Create Popup")]
    public GameObject popupCreateRoom;        // Popup_CreateRoom
    public TMP_InputField inputRoomName;      // Popup/Box/Row_Input/Input
    public Button btnCancel;                  // Popup/Box/Row_Buttons/Btn_Cancel
    public Button btnOK;                      // Popup/Box/Row_Buttons/Btn_OK

    [Header("List Targets")]
    public Transform listContent;             // Panel_ListNormal/Scroll_List/Viewport/Content
    public GameObject roomItemPrefab;         // Prefabs/RoomItem

    [Header("Open Button")]
    public Button btnOpenCreatePopup;         // Panel_ListNormal/Row_Actions/Btn_Create

    void Awake()
    {
        if (popupCreateRoom) popupCreateRoom.SetActive(false);

        if (btnOpenCreatePopup) btnOpenCreatePopup.onClick.AddListener(OpenCreatePopup);
        if (btnCancel) btnCancel.onClick.AddListener(CloseCreatePopup);
        if (btnOK) btnOK.onClick.AddListener(CreateRoom);
    }

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

        // 1) 이름 결정
        string nameToUse = "새 공간";
        if (inputRoomName && !string.IsNullOrWhiteSpace(inputRoomName.text))
            nameToUse = inputRoomName.text.Trim();

        // 2) 프리팹 인스턴스 생성 (리스트 Content 하위)
        GameObject go = Instantiate(roomItemPrefab, listContent);
        go.name = $"RoomItem_{nameToUse}";

        // 3) 생성 직후 상태 : Normal ON / Edit OFF
        SetRoomItemMode(go, isEdit: false);

        // 4) 텍스트 바인딩 (양쪽 그룹의 이름/날짜 동기화)
        ApplyRoomItemTexts(go, nameToUse, GetNowDateString());

        // 5) 팝업 닫기
        CloseCreatePopup();

        // (선택) 생성 후 마지막에 포커스 이동/스크롤 조정 등을 하고 싶으면 여기에 추가
    }

    // --------- 헬퍼들 ---------

    static string GetNowDateString()
    {
        // 원하는 형식으로 포맷 (와이어프레임 예시: YYYY-XX-XX 00:00)
        return System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
    }

    // RoomItem 안의 그룹 활성/비활성
    public static void SetRoomItemMode(GameObject roomItem, bool isEdit)
    {
        if (!roomItem) return;

        var t = roomItem.transform;
        var groupNormal = t.Find("Group_Normal");
        var groupEdit = t.Find("Group_Edit");

        if (groupNormal) groupNormal.gameObject.SetActive(!isEdit);
        if (groupEdit) groupEdit.gameObject.SetActive(isEdit);
    }

    // 이름/날짜 텍스트 적용 (양 그룹 동시 반영)
    public static void ApplyRoomItemTexts(GameObject roomItem, string name, string date)
    {
        if (!roomItem) return;

        // Group_Normal
        SetIfExist(roomItem.transform, "Group_Normal/Col_Texts/Txt_Name", name);
        SetIfExist(roomItem.transform, "Group_Normal/Col_Texts/Txt_Date", date);

        // Group_Edit
        SetIfExist(roomItem.transform, "Group_Edit/Col_Texts/Txt_Name", name);
        SetIfExist(roomItem.transform, "Group_Edit/Col_Texts/Txt_Date", date);
    }

    static void SetIfExist(Transform root, string path, string text)
    {
        var tr = root.Find(path);
        if (!tr) return;

        // 우선 TMP_Text 시도
        var tmp = tr.GetComponent<TMP_Text>();
        if (tmp) { tmp.text = text; return; }

        // Label이 TMP가 아니라면(일반 Text 등)도 커버
        var uText = tr.GetComponent<UnityEngine.UI.Text>();
        if (uText) { uText.text = text; return; }
    }
}
