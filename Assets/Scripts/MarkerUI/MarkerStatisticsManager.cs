using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class MarkerStatisticsManager : MonoBehaviour
{
    // ======================================================================
    // 1. 통계 출력 필드 (Inspector 연결 필요)
    // ======================================================================
    [Header("통계 출력 필드")]
    public TextMeshProUGUI totalCountText;
    public TextMeshProUGUI favoriteCountText;

    // **색상별 개수 출력 필드** (사용자님의 UI 레이아웃에 맞춰 연결)
    public TextMeshProUGUI redCountText;
    public TextMeshProUGUI blueCountText;
    public TextMeshProUGUI orangeCountText;
    public TextMeshProUGUI greenCountText;
    public TextMeshProUGUI blackCountText;
    // 필요한 경우 다른 색상도 추가 (yellow, green, etc.)

    // 이 함수를 통계 버튼 OnClick()에 연결합니다.
    public void GenerateStatistics()
    {
        ARMarkerData[] placedMarkers = FindObjectsByType<ARMarkerData>(FindObjectsSortMode.None);

        if (placedMarkers.Length == 0)
        {
            // 마커가 없을 때 모든 필드를 초기화
            if (totalCountText != null) totalCountText.text = "전체 마커 : 0개";
            if (favoriteCountText != null) favoriteCountText.text = "즐겨찾기 : 0개";

            // 색상별 필드 초기화 (필요시)
            if (redCountText != null) redCountText.text = "빨강 마커 : 0개";
            if (blueCountText != null) blueCountText.text = "파랑 마커 : 0개";
            if (orangeCountText != null) orangeCountText.text = "노랑 마커 : 0개";
            if (greenCountText != null) greenCountText.text = "초록 마커 : 0개";
            if (blackCountText != null) blackCountText.text = "검정 마커 : 0개";
            // ...
            return;
        }

        // 2. 통계 계산을 위한 Dictionary와 변수 초기화
        Dictionary<string, int> colorCounts = new Dictionary<string, int>();
        int favoriteCount = 0;
        int totalMarkers = placedMarkers.Length;

        // 3. 데이터 순회 및 집계
        foreach (ARMarkerData marker in placedMarkers)
        {
            if (marker.fullMarkerData == null) continue;

            // 색상별 개수 집계
            string color = marker.fullMarkerData.ColorCode;
            if (colorCounts.ContainsKey(color))
            {
                colorCounts[color]++;
            }
            else
            {
                colorCounts.Add(color, 1);
            }

            // 즐겨찾기 개수
            if (marker.fullMarkerData.IsFavorite)
            {
                favoriteCount++;
            }
        }

        // 4. **[핵심]** 계산된 값을 전용 텍스트 필드에 출력

        // 총 개수 출력
        if (totalCountText != null)
            totalCountText.text = "전체 마커 : " + totalMarkers.ToString() + "개";

        // 즐겨찾기 개수 출력
        if (favoriteCountText != null)
            favoriteCountText.text = "즐겨찾기 : " + favoriteCount.ToString() + "개";

        // 5. **색상별 개수 출력** (Hex 코드를 기반으로 매핑)

        // 모든 색상 필드를 0으로 초기화 (새로운 계산 시작 전에 필요)
        if (redCountText != null) redCountText.text = "빨강 마커 : 0개";
        if (blueCountText != null) blueCountText.text = "파랑 마커 : 0개";
        if (orangeCountText != null) orangeCountText.text = "노랑 마커 : 0개";
        if (greenCountText != null) greenCountText.text = "초록 마커 : 0개";
        if (blackCountText != null) blackCountText.text = "검정 마커 : 0개";

        foreach (var kvp in colorCounts)
        {
            // Hex Code를 확인하고 해당 텍스트 필드에 값을 할당합니다.
            // (ColorCode는 UIPopupManager.cs와 MarkerColorImageManager.cs에서 정의된 Hex 값을 사용해야 합니다.)
            switch (kvp.Key.ToUpper())
            {
                case "#FF0000": // 빨간색
                    if (redCountText != null) redCountText.text = "빨강 마커 : " + kvp.Value.ToString() + "개";
                    break;
                case "#0000FF": // 파란색
                    if (blueCountText != null) blueCountText.text = "파랑 마커 : " + kvp.Value.ToString() + "개";
                    break;
                case "#FFC000": // 오렌지색
                    if (orangeCountText != null) orangeCountText.text = "노랑 마커 : " + kvp.Value.ToString() + "개";
                    break;
                case "#00FF00": // 초록색
                    if (greenCountText != null) greenCountText.text = "초록 마커 : " + kvp.Value.ToString() + "개";
                    break;
                case "#000000": // 검은색
                    if (blackCountText != null) blackCountText.text = "검정 마커 : " + kvp.Value.ToString() + "개";
                    break;
                    // TODO: 다른 색상이 있다면 여기에 case를 추가합니다.
            }
        }
    }

    // ... (GetColorName Helper 함수는 더 이상 UI 출력에 사용되지 않으므로 생략) ...
}