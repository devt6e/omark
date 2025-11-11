using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpaceCreator : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField inputField;     // 입력 필드
    public GameObject uiPrefab;           // 생성할 프리팹
    public Transform contentParent;       // ScrollView의 Content

    public void OnCreateButtonClicked()
    {
        // 1. 입력 텍스트 확인
        string nameText = inputField.text.Trim();
        if (string.IsNullOrEmpty(nameText))
        {
            Debug.LogWarning("UIItemCreator: 이름이 비어 있습니다.");
            return;
        }

        // 2. 프리팹 인스턴스 생성
        GameObject newItem = Instantiate(uiPrefab, contentParent);

        // 3. 하위 텍스트 컴포넌트 찾기
        TextMeshProUGUI[] texts = newItem.GetComponentsInChildren<TextMeshProUGUI>();

        // 4. "이름"과 "최종 수정" 필드 업데이트
        foreach (var t in texts)
        {
            if (t.name == "name")
                t.text = "공간 이름 : " + nameText;
            else if (t.name == "last_modified")
                t.text = "최종 수정 : " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // 5. 입력 필드 초기화
        inputField.text = "";


    }


}

/*
using UnityEngine;
using UnityEngine.UI;
using TMPro;   // InputField가 TMP_InputField라면 필요

public class UIItemCreator : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField inputField;     // 입력 필드
    public GameObject uiPrefab;           // 생성할 프리팹
    public Transform contentParent;       // ScrollView의 Content

    public void OnCreateButtonClicked()
    {
        // 1. 입력 텍스트 확인
        string nameText = inputField.text.Trim();
        if (string.IsNullOrEmpty(nameText))
        {
            Debug.LogWarning("UIItemCreator: 이름이 비어 있습니다.");
            return;
        }

        // 2. 프리팹 인스턴스 생성
        GameObject newItem = Instantiate(uiPrefab, contentParent);

        // 3. 하위 텍스트 컴포넌트 찾기
        TextMeshProUGUI[] texts = newItem.GetComponentsInChildren<TextMeshProUGUI>();

        // 4. "이름"과 "최종 수정" 필드 업데이트
        foreach (var t in texts)
        {
            if (t.name == "NameText")
                t.text = nameText;
            else if (t.name == "LastModifiedText")
                t.text = "최종 수정: " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        // 5. 입력 필드 초기화
        inputField.text = "";
    }
}

*/