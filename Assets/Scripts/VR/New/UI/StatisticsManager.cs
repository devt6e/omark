using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO.Compression;

public class StatisticsManager : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text txtFavorite;
    [SerializeField] private TMP_Text txtRed;
    [SerializeField] private TMP_Text txtBlue;
    [SerializeField] private TMP_Text txtYellow;
    [SerializeField] private TMP_Text txtGreen;
    [SerializeField] private TMP_Text txtBlack;
    [SerializeField] private TMP_Text txtAI;

    [Header("Button")]
    [SerializeField] private Button btnOkay;

    private void Awake()
    {
        btnOkay.onClick.AddListener(Hide);
        gameObject.SetActive(false);
    }
    
    private void UpdateInfo()
    {

    }

    public void Show()
    {   
        UpdateInfo();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
