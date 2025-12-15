using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO.Compression;


public class SearchPanelController : MonoBehaviour
{
    
    [Header("Inputs")]
    [SerializeField] private TMP_InputField searchInput;

    [Header("Buttons")]
    [SerializeField] private Button btnBack;
    [SerializeField] private Button btnSearch;
    [SerializeField] private Button btnClearInput;

    private string searchKeyword = "";

    // =========================
    // Refs
    // =========================
    // [Header("Refs")]
    // [SerializeField] private MenuBtnController Menu;

    private void Awake()
    {
        btnBack.onClick.AddListener(OnClickBack);
        btnSearch.onClick.AddListener(OnClickSearch);
        btnClearInput.onClick.AddListener(OnClickClear);
        searchInput.onValueChanged.AddListener(_ => ShowClearBtn());
        gameObject.SetActive(false);
        btnClearInput.gameObject.SetActive(false);

    } 

    public void OnClickBack()
    {
        // MarkerFilterController.Instance.SetSearchKeyword("");
        Hide();
    }

    public void OnClickSearch()
    {
        //버튼이 눌렸을 때 InputField의 문자열과 일치하는 마커슬롯만 인벤토리에 표시
        //마커 슬롯에 남아있는 마커만 회전
        //public void SetMultipleTargets(IEnumerable<MarkerInstance> markers)
        searchKeyword = searchInput.text;
        searchInput.text = string.Empty;
        if(string.IsNullOrEmpty(searchKeyword))
            return;
        MarkerFilterController.Instance.SetSearchKeyword(searchKeyword);
        Hide();
    }

    public void OnClickClear()
    {
        searchInput.text = string.Empty;
        btnClearInput.gameObject.SetActive(false);        
    }

    private void ShowClearBtn()
    {
        btnClearInput.gameObject.SetActive(!string.IsNullOrEmpty(searchInput.text));
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }

}
