using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO.Compression;


public class MenuBtnController : MonoBehaviour
{
    
    [Header("Buttons")]
    [SerializeField] private Button btnMenu;
    [SerializeField] private Button btnSearch;
    [SerializeField] private Button btnViewAll;    
    [SerializeField] private Button btnViewFavorite;
    [SerializeField] private Button btnStatistic;

    [Header("Panel")]
    [SerializeField] private GameObject menuItem;

    [Header("Refs")]
    [SerializeField] private SearchPanelController SearchPanel;
    [SerializeField] private StatisticsManager StratisticsPanel;

    private bool isItemOpen = false;


    private void Awake()
    {
        btnMenu.onClick.AddListener(ToggleItmes);
        btnSearch.onClick.AddListener(OpenSerachPanel);
        btnViewAll.onClick.AddListener(ViewAll);
        btnViewFavorite.onClick.AddListener(ViewFavorite);
        btnStatistic.onClick.AddListener(OpenStatisticPanel);
        menuItem.SetActive(false);
    } 

    public void ToggleItmes()
    {
        isItemOpen = !isItemOpen;
        menuItem.SetActive(isItemOpen);
    }

    public void OpenSerachPanel()
    {
        SearchPanel.Show();
        ToggleItmes();
    }

    public void ViewFavorite()
    {
        if(MarkerFilterController.Instance.FavoriteOnly)
            return;
        MarkerFilterController.Instance.SetFavorite(true);
    }

    //검색, 즐겨찾기 상태 모두 해제
    public void ViewAll()
    {
        MarkerFilterController.Instance.SetFavorite(false);
        MarkerFilterController.Instance.ClearSearch();
    }
    public void OpenStatisticPanel()
    {
        StratisticsPanel.Show();
        ToggleItmes();
    }
}
