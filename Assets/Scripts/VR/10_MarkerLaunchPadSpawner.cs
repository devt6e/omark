using UnityEngine;

public class MarkerLaunchPadSpawner : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform content;              // ScrollView/Content
    [SerializeField] private GameObject launchPadItemPrefab; // MarkerLaunchPadItem 프리팹

    /// <summary>
    /// 생성 버튼 OnClick에 연결
    /// </summary>
    public void CreateLaunchPad()
    {
        if (content == null || launchPadItemPrefab == null)
        {
            Debug.LogError("[MarkerLaunchPadSpawner] Reference missing");
            return;
        }

        GameObject item = Instantiate(launchPadItemPrefab, content);

        // (선택) 생성 직후 초기화
        // item.GetComponent<UIMarkerLaunchPad>()?.Init(...);
        // item.GetComponent<MarkerLaunchPadView>()?.SetText("새 마커");

        item.transform.SetAsLastSibling(); // 맨 아래 추가 (가독성)
    }
}
