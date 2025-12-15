using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject hiddenPanel;

    [Header("Move and Marker Panel Settings")]
    [SerializeField]
    private GameObject movePanel; // Hierarchy�� move_panel�� ������ �ʵ�

    [SerializeField]
    private GameObject researchPanel; // �˻��� �Է� �г��� ������ �ʵ�

    [SerializeField]
    private TMP_InputField researchInputField; // �˻��� �Է�â (TMP_InputField)

    [SerializeField]
    private GameObject makrerPanel; // Hierarchy�� Makrer �г��� ������ �ʵ�

    [SerializeField]
    private GameObject ImagePanel;

    [SerializeField]
    private GameObject StatisticsPanel;

    // Menu ��ư�� ������ �Լ�: �г��� Ȱ��ȭ(���̰�) �մϴ�.
    public void OpenPanel()
    {
        if (hiddenPanel != null)
        {
            hiddenPanel.SetActive(true);
            ImagePanel.SetActive(false);
        }
    }

    // move_back ��ư�� ������ �Լ�: �г��� ��Ȱ��ȭ(�����) �մϴ�.
    public void ClosePanel()
    {
        if (hiddenPanel != null)
        {
            hiddenPanel.SetActive(false);
            ImagePanel.SetActive(true);
        }
    }

    public void ToggleMovePanel()
    {
        // ����� ������Ʈ�� �ִ��� �ٽ� �ѹ� Ȯ��
        if (movePanel == null || makrerPanel == null)
        {
            Debug.LogError("MovePanel �Ǵ� MakrerPanel�� Inspector�� ������� �ʾҽ��ϴ�.");
            return;
        }

        // 1. movePanel�� ���� Ȱ��ȭ ���¸� �������� ���ο� ���¸� �����մϴ�.
        //    (��: ���� ������ true(����), ���� ������ false(�ݱ�))
        bool isMovePanelOpening = !movePanel.activeSelf;

        // 2. movePanel�� ���¸� ����մϴ�.
        movePanel.SetActive(isMovePanelOpening);

        // 3. makrerPanel�� ���¸� movePanel�� ���ο� ���¿� ���ݴ�� �����մϴ�.
        //    (��, movePanel�� ������ MakrerPanel�� ������, movePanel�� ������ MakrerPanel�� �����ϴ�.)
        makrerPanel.SetActive(!isMovePanelOpening);
    }

    // ������ ��ư�� ������ �Լ�
    public void OpenResearchPanel()
    {
        if (researchPanel != null)
        {
            researchPanel.SetActive(true);
        }
    }

    // ��� ��ư�� ������ �Լ� (�г� ��Ȱ��ȭ)
    public void CloseResearchPanel()
    {
        if (researchPanel != null)
        {
            researchPanel.SetActive(false);
        }

        if (researchInputField != null)
        {
            // Input Field�� �ؽ�Ʈ�� �� ���ڿ��� �����մϴ�.
            researchInputField.text = string.Empty;
        }
    }

    public void OpenStatisticsPanel()
    {
        if (StatisticsPanel != null)
        {
            StatisticsPanel.SetActive(true);
        }
    }

    public void CloseStatisticsPanel()
    {
        if (StatisticsPanel != null)
        {
            StatisticsPanel.SetActive(false);
        }
    }
}
