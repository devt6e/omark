using UnityEngine;
using UnityEngine.UI; // UI 관련 클래스를 사용하기 위해 필요
using System; // Func 타입을 사용하기 위해 필요 (선택 사항)

public class FavoriteToggle : MonoBehaviour
{
    // [SerializeField]를 사용하여 Inspector에서 요소를 연결합니다.

    // 1. 상태를 변경할 대상 Image 컴포넌트
    [SerializeField] private Image starImage;

    // 2. 상태에 따른 Sprite (방법 A를 사용할 경우)
    [SerializeField] private Sprite emptyStarSprite;
    [SerializeField] private Sprite filledStarSprite;

    // 3. 현재 즐겨찾기 상태를 저장할 변수
    private bool isFavorite = false;

    // 4. (선택 사항) 초기 상태 설정
    void Start()
    {
        // 초기 상태에 따라 이미지를 설정합니다. (데이터 로드 후 적용)
        UpdateStarAppearance();
    }

    // 버튼 클릭 시 호출될 공용 함수
    public void ToggleFavorite()
    {
        isFavorite = !isFavorite;

        UpdateStarAppearance();

        Debug.Log("즐겨찾기 상태 변경됨: " + isFavorite);
    }

    // 상태에 따라 Image 컴포넌트의 외형을 변경하는 함수
    private void UpdateStarAppearance()
    {
        if (starImage == null)
        {
            Debug.LogError("Star Image 컴포넌트가 연결되지 않았습니다!");
            return;
        }

        // 방법 A: Sprite 변경
        if (isFavorite)
        {
            starImage.sprite = filledStarSprite; // 노란색 채워진 별로 변경
        }
        else
        {
            starImage.sprite = emptyStarSprite; // 비어 있는 별로 변경
        }
    }
}