using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class MarkerDefinitionSlot : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("UI")]
    [SerializeField] private TMP_Text txt_name;
    [SerializeField] private GameObject[] colorImages;

    [Header("Definition")]
    [SerializeField] private string definitionId;

    [Header("Long Press")]
    [SerializeField] private float longPressTime = 0.4f;

    [SerializeField] private string defaultName;
    
    // 씬에서 주입될 참조 (프리팹 인스펙터로 연결 불가한 것들)
    private MarkerSlotSpawner spawner;
    private MarkerInfoPanel infoPanel;

    private Coroutine longPressCo;
    private bool longPressTriggered;
    private bool isPointerDown;

    /// <summary>
    /// (중요) 슬롯 생성 직후, 씬 쪽에서 반드시 호출해서 참조를 주입한다.
    /// </summary>
    public void Initialize(string definitionId,MarkerSlotSpawner spawner, MarkerInfoPanel infoPanel)
    {
        this.definitionId = definitionId;
        this.spawner = spawner;
        this.infoPanel = infoPanel;

        ApplyInfo();
    }

    private void ApplyInfo()
    {
        MarkerDefinition def = MarkerDefinitionRepository.Instance.GetById(definitionId);
        if (def == null)
        {
            Debug.LogError("[MarkerDefinitionSlot] Definition not found");
            return;
        }
        // MarkerDefinitionRepository.Instance.GetDefInfo(definitionId);
        this.txt_name.text = def.DisplayName;
        // this.colorImage.color = def.Color;
        int index = def.ColorIndex;

        for (int i = 0; i < colorImages.Length; i++)
        {
            colorImages[i].SetActive(i == index);
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (spawner == null)
        {
            Debug.LogError("[MarkerDefinitionSlot] spawner is null. Initialize() was not called.");
            return;
        }

        if (spawner.IsDefinitionLocked(definitionId))
            return;

        isPointerDown = true;
        longPressTriggered = false;

        longPressCo = StartCoroutine(LongPressRoutine());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;

        if (longPressCo != null)
        {
            StopCoroutine(longPressCo);
            longPressCo = null;
        }
        // ⭐ 롱프레스가 아니었다면 → 탭
        if (!longPressTriggered)
        {
            OpenInfoPanel();
        }
    }

    private IEnumerator LongPressRoutine()
    {
        float t = 0f;

        while (t < longPressTime)
        {
            if (!isPointerDown)
                yield break;

            t += Time.deltaTime;
            yield return null;
        }
        // ⭐ 롱프레스 성공
        longPressTriggered = true;
        // 배치 시작 요청 (실제 생성/배치는 spawner가 담당)
        spawner.BeginPlacement(definitionId);
    }

    /// <summary>
    /// 외부(UI 생성 버튼 등)에서 즉시 배치하고 싶을 때 호출하는 편의 함수
    /// </summary>
    public void BeginPlacementFromCode()
    {
        if (spawner == null)
        {
            Debug.LogError("[MarkerDefinitionSlot] spawner is null. Initialize() was not called.");
            return;
        }

        if (spawner.IsDefinitionLocked(definitionId))
            return;

        spawner.BeginPlacement(definitionId);
    }

    private void OpenInfoPanel()
    {
        if (infoPanel == null)
        {
            Debug.LogWarning("[MarkerDefinitionSlot] EditPanel is not assigned.");
            return;
        }
        // MarkerDefinitionRepository.Instance.GetDefInfo(definitionId);
        infoPanel.Open(definitionId);
    }

    public void Refresh() { ApplyInfo(); }
}
