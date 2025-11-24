using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro�� ����ϱ� ���� �ʿ��մϴ�.

public class UIPopupManager : MonoBehaviour
{
    // ======================================================================
    // 1. ���� �˾� �� �г� ���� (Unity Inspector�� ����)
    // ======================================================================
    [Header("1. ���� �˾� �� �г�")]
    public GameObject markerDetailPopup;    // ��Ŀ �� ���� �˾� (SCENE3-3 �ʱ� ȭ��)
    public GameObject deleteConfirmPopup;   // "���� �����Ͻðڽ��ϱ�?" �г� (���� Ȯ��)
    public GameObject detailEditPanel;      // ���� �� ���� �Է� �г�

    // ======================================================================
    // 2. ���� �˾� UI ��� ����
    // ======================================================================
    [Header("2. ���� �˾� ���� ���")]
    public TextMeshProUGUI nameText;        // ��Ŀ �̸� ǥ��
    public TextMeshProUGUI detailInfoText;  // �� ���� ǥ��

    // �ϸ�ũ (���ã��) ���� ǥ�� �� ����
    public Button bookmarkButton;           // ���ã��(Bookmark) ��ư
    public Image bookmarkImageComponent;     // ���ã�� ��ư�� Image ������Ʈ
    public Sprite filledStarSprite;           // ����� ä���� �� �̹��� (���ã�� O)
    public Sprite emptyStarSprite;            // �� �� �̹��� (���ã�� X)

    public Button deleteButton;             // '����' ��ư (-> ���� Ȯ�� �˾�)
    public Button editButton;               // '����' ��ư (-> ���� �� �г�)
    public Button closeButton;              // �ݱ�/��� ��ư

    // ======================================================================
    // 3. ���� �г� UI ��� ����
    // ======================================================================
    [Header("3. ���� �г� ���� ���")]
    public TMP_InputField nameInputField;   // **(�ʼ�) ��Ŀ �̸� �Է� �ʵ�**
    public TMP_InputField detailInputField; // **(�ʼ�) ���� �߰��� �� ���� �Է� �ʵ�**
    public GameObject colorButtonContainer; // ���� ��ư���� ��� �θ� ������Ʈ (������)
    public Button editConfirmButton;        // ���� �г��� 'Ȯ��' ��ư
    public Button editCancelButton;         // ���� �г��� '���' ��ư
    // ����: ���� ��ư���� ���������� �� ��ũ��Ʈ�� OnColorSelected �Լ��� �����ؾ� �մϴ�.

    // ======================================================================
    // 4. ���� Ȯ�� �˾� UI ��� ����
    // ======================================================================
    [Header("4. ���� Ȯ�� �˾� ���� ���")]
    public Button deleteYesButton;          // ���� Ȯ�� �˾��� '��' ��ư
    public Button deleteNoButton;           // ���� Ȯ�� �˾��� '�ƴϿ�' ��ư

    // ======================================================================
    // 5. �˻� Ȯ�� �˾� UI ��� ����
    // ======================================================================
    [Header("5. �˻� �г� ���")]
    public GameObject searchPanel;              // �˻� �г� ��ü (Ȱ��ȭ/��Ȱ��ȭ��)
    public TMP_InputField searchInputField;     // ��Ŀ �̸��� �Է��� �ʵ�
    public Button searchConfirmButton;          // �˻� 'Ȯ��' ��ư
    [Header("6. �˻� ���� �г� ���")]
    public GameObject failedPanel;              // ���� �޽��� �г�
    public Button failedConfirmButton;          // ���� �г��� 'Ȯ��' ��ư

    // ======================================================================
    // 6. ���� ���� ����
    // ======================================================================
    private MarkerData currentMarkerData;    // ���� �˾��� ǥ�� ���� ��Ŀ ������
    private string selectedColorCode;       // ���� �� ���õ� ����
    private bool tempIsFavorite;
    private ColorButtonTag[] colorButtons;

    private CameraFocusController focusController; // ī�޶� ��Ŀ�� ��ũ��Ʈ �ν��Ͻ�
    private MarkerListUIController uiController;   // ��Ŀ ����Ʈ ���� ��ũ��Ʈ �ν��Ͻ�

    void Start()
    {
        // 1. ��� �г� ���� �� �����
        if (markerDetailPopup != null) markerDetailPopup.SetActive(false);
        if (deleteConfirmPopup != null) deleteConfirmPopup.SetActive(false);
        if (detailEditPanel != null) detailEditPanel.SetActive(false);

        // 2. ���� �˾� ��ư ����
        if (bookmarkButton != null) bookmarkButton.onClick.AddListener(OnBookmarkToggled);
        if (deleteButton != null) deleteButton.onClick.AddListener(OnDeleteClicked);
        if (editButton != null) editButton.onClick.AddListener(OnEditClicked);
        if (closeButton != null) closeButton.onClick.AddListener(HideMarkerDetailPopup);

        // 3. ���� �г� ��ư ����
        if (editConfirmButton != null) editConfirmButton.onClick.AddListener(OnEditConfirmed);
        if (editCancelButton != null) editCancelButton.onClick.AddListener(OnEditCancelled);

        // 4. ���� Ȯ�� �˾� ��ư ����
        if (deleteYesButton != null) deleteYesButton.onClick.AddListener(OnDeleteFinalConfirmed);
        if (deleteNoButton != null) deleteNoButton.onClick.AddListener(HideDeleteConfirmPopup);

        // �ʱ� ���� ���� (����)
        selectedColorCode = "#FFFFFF";

        // [ī�޶� ��Ŀ�� ��ũ��Ʈ �� UI ��Ʈ�ѷ� ����]
        focusController = FindAnyObjectByType<CameraFocusController>();
        uiController = FindAnyObjectByType<MarkerListUIController>();

        // [�˻� ��ư ����]
        if (searchConfirmButton != null)
        {
            searchConfirmButton.onClick.AddListener(OnSearchConfirmClicked);
        }

        // ���� �г��� 'Ȯ��' ��ư ����
        if (failedConfirmButton != null)
        {
            failedConfirmButton.onClick.AddListener(OnFailedConfirmClicked);
        }
    }

    // ======================================================================
    // ��Ŀ �� �˾� ǥ��/����� (Marker Icon Clicked)
    // ======================================================================

    // UIMarkerItemData.cs���� ȣ���ϴ� �Լ�
    public void ShowMarkerDetailPopup(MarkerData data)
    {
        currentMarkerData = data;
        tempIsFavorite = data.IsFavorite;

        // �˾��� ������ �ݿ�
        if (nameText != null)
        {
            nameText.text = data.Name;
        }

        if (detailInfoText != null)
        {
            // DetailInformation�� null�̰ų� ��������� �⺻��(��: "���� ����")�� ǥ���� �� �ֽ��ϴ�.
            detailInfoText.text = string.IsNullOrEmpty(data.DetailInformation)
                                  ? "��Ŀ �� ������ �����ϴ�."
                                  : data.DetailInformation;
        }

        UpdateBookmarkVisual(tempIsFavorite);

        // �˾� ǥ��
        if (markerDetailPopup != null)
        {
            markerDetailPopup.SetActive(true);
        }
        Debug.Log($"[Popup] �� �˾� ǥ��. ��Ŀ ID: {data.Id}");
    }

    private void UpdateBookmarkVisual(bool isFavorite)
    {
        if (bookmarkImageComponent != null && filledStarSprite != null && emptyStarSprite != null)
        {
            if (isFavorite)
            {
                bookmarkImageComponent.sprite = filledStarSprite;
            }
            else
            {
                bookmarkImageComponent.sprite = emptyStarSprite;
            }
        }
        else
        {
            Debug.LogWarning("���ã�� �̹��� ������Ʈ �Ǵ� ��������Ʈ ������ �����Ǿ����ϴ�.");
        }
    }

    public void HideMarkerDetailPopup()
    {
        if (markerDetailPopup != null)
        {
            markerDetailPopup.SetActive(false);
        }
        currentMarkerData = null;
    }

    // ======================================================================
    // 1. ���ã�� (Bookmark) ��� ����
    // ======================================================================
    private void OnBookmarkToggled()
    {
        if (currentMarkerData != null)
        {
            // 1. ������ ������Ʈ
            tempIsFavorite = !tempIsFavorite;

            // 2. �˾� UI �ݿ� (�� �̹��� ��� ����)
            UpdateBookmarkVisual(tempIsFavorite);

            Debug.Log($"[Action] ���ã�� ���� ����: ID {currentMarkerData.Id} -> {currentMarkerData.IsFavorite}");
        }
    }

    // ======================================================================
    // 2. ���� ���� (Delete Button)
    // ======================================================================

    // ���� �˾����� '����' ��ư Ŭ�� �� (���� Ȯ�� �г� ��û)
    private void OnDeleteClicked()
    {
        if (currentMarkerData != null && deleteConfirmPopup != null)
        {
            // "���� �����Ͻðڽ��ϱ�?" �г� ����
            deleteConfirmPopup.SetActive(true);
            //HideMarkerDetailPopup(); // �� ���� �˾�
        }
    }

    // ���� Ȯ�� �гο��� '��' ��ư Ŭ�� �� (���� ���� ����)
    private void OnDeleteFinalConfirmed()
    {
        if (currentMarkerData != null)
        {
            string deletedId = currentMarkerData.Id;

            Debug.Log($"[Action] ���� ��Ŀ ���� ����: ID {deletedId}");

            // TODO: 1. ������ ���� ��û
            // TODO: 2. AR �������� 3D ��Ŀ ������Ʈ �ı�

            // 3. UI ����Ʈ���� ��Ŀ ������ ������Ʈ ���� (�߰��� �ٽ� ����)
            MarkerListUIController uiController = FindFirstObjectByType<MarkerListUIController>();
            if (uiController != null)
            {
                uiController.RemoveMarkerIcon(deletedId);
            }

            currentMarkerData = null;
            HideDeleteConfirmPopup();
            HideMarkerDetailPopup();
        }
    }

    // ���� Ȯ�� �г� �ݱ� (���/�ƴϿ� ��ư Ŭ�� ��)
    public void HideDeleteConfirmPopup()
    {
        if (deleteConfirmPopup != null)
        {
            deleteConfirmPopup.SetActive(false);
        }
    }

    // ======================================================================
    // 3. ���� ���� (Edit Button)
    // ======================================================================

    private void UpdateColorSelectionVisual()
    {
        if (colorButtons == null) return;

        foreach (var tag in colorButtons)
        {
            if (tag.selectionHighlight != null)
            {
                // ���� ���õ� ���� �ڵ�� ��ư�� ���� �ڵ尡 ��ġ�ϴ��� Ȯ��
                bool isSelected = tag.buttonColorCode == selectedColorCode;

                // ��ġ�ϸ� ���� ������Ʈ�� �Ѱ�, �ƴϸ� ���ϴ�.
                tag.selectionHighlight.SetActive(isSelected);

                Debug.Log($"Checking {tag.buttonColorCode}. Selected: {isSelected}");
            }
        }
    }

    // ���� �˾����� '����' ��ư Ŭ�� ��
    private void OnEditClicked()
    {
        if (currentMarkerData != null && detailEditPanel != null)
        {
            // 1. ���� �гο� ���� �̸� ������ �̸� ä���
            if (nameInputField != null)
            {
                nameInputField.text = currentMarkerData.Name;
            }

            // 2. **�� ���� ������ �̸� ä��� (�߰�)**
            if (detailInputField != null)
            {
                detailInputField.text = currentMarkerData.DetailInformation;
            }

            // 3. ���� �ʱ�ȭ
            selectedColorCode = currentMarkerData.ColorCode;
            // colorButtonContainer ������ ��� ColorButtonTag ������Ʈ�� �����ɴϴ�.
            if (colorButtonContainer != null)
            {
                colorButtons = colorButtonContainer.GetComponentsInChildren<ColorButtonTag>();
            }
            UpdateColorSelectionVisual();
            // TODO: ���� ���� ��ư�� ���õ� ���·� �ð������� ǥ�õǵ��� �ϴ� ���� �߰�

            // 4. �г� ǥ��
            detailEditPanel.SetActive(true);
            //HideMarkerDetailPopup(); // �� ���� �˾��� ����
        }
    }

    // ���� �г� �� ���� ��ư Ŭ�� �� ȣ�� (�� ���� ��ư�� OnClick�� ����)
    public void OnColorSelected(string colorCode)
    {
        selectedColorCode = colorCode;

        UpdateColorSelectionVisual();

        Debug.Log($"[Edit] �� ���� ����: {colorCode}");
    }

    // ���� �г��� 'Ȯ��' ��ư Ŭ�� �� (���� ����)
    private void OnEditConfirmed()
    {
        if (currentMarkerData != null)
        {
            currentMarkerData.IsFavorite = tempIsFavorite;

            MarkerListUIController uiController = FindFirstObjectByType<MarkerListUIController>();
            if (uiController != null)
            {
                // UpdateMarkerIconStatus ���ο��� ���ŵ� currentMarkerData�� ����մϴ�.
                uiController.UpdateMarkerIconStatus(currentMarkerData);
            }

            // 1. �̸� ���� (Detail information �Է�)
            string newName = nameInputField.text;
            if (nameInputField != null && !string.IsNullOrEmpty(newName))
            {
                currentMarkerData.Name = newName;
            }

            // 2. **�� ���� ���� (�߰�)**
            string newDetail = detailInputField.text;
            if (detailInputField != null) // ��� �־ ���� (����ڰ� ���� �� ����)
            {
                currentMarkerData.DetailInformation = newDetail;
            }

            // 3. ���� ����
            if (!string.IsNullOrEmpty(selectedColorCode))
            {
                currentMarkerData.ColorCode = selectedColorCode;
            }

            // TODO: ������ ������Ʈ ��û �� UI ����Ʈ ���� ��û

            // 4. **UI ����Ʈ ���� ��û (�ٽ� ����/���� ����)**
            if (uiController != null)
            {
                uiController.UpdateMarkerIconStatus(currentMarkerData); // currentMarkerData�� �����Ͽ� ����Ʈ UI�� �̸�, ���� ���� �����մϴ�.
            }

            Debug.Log($"[Action] ���� ���� �Ϸ�. Name: {currentMarkerData.Name}, Detail: {currentMarkerData.DetailInformation}");
            HideMarkerDetailPopup();
            HideEditPanel(); // ���� �г� �ݱ�
        }
    }

    // ���� �г��� '���' ��ư Ŭ�� �� (��Ŀ ���� ����Ʈ�� ���ƿ�)
    private void OnEditCancelled()
    {
        // ������ ����ϰ� ���� �г��� �ݽ��ϴ�.
        HideEditPanel();
    }

    // ======================================================================
    // **6. �˻� ��� ����**
    // ======================================================================

    private void OnSearchConfirmClicked()
    {
        if (searchInputField == null || string.IsNullOrEmpty(searchInputField.text))
        {
            Debug.LogWarning("�˻�� �Է����ּ���.");
            return;
        }

        string searchName = searchInputField.text.Trim();

        if (uiController == null)
        {
            Debug.LogError("MarkerListUIController�� ã�� �� �����ϴ�.");
            return;
        }

        // 1. **�˻� �̸��� ��ġ�ϴ� 3D ��Ŀ�� ã���ϴ�.**
        GameObject targetMarker = Find3DMarkerByName(searchName);

        if (targetMarker != null)
        {
            Debug.Log($"[Search Success] ��Ŀ '{searchName}'�� ã�ҽ��ϴ�. ī�޶� �̵� ��û.");

            // 2. **[�ٽ�]** ī�޶� ��Ŀ�� ��û
            if (focusController != null)
            {
                focusController.FocusOnMarker(targetMarker.transform);
                // �˻� �� �г� �ݱ� (���� ����)
                if (searchPanel != null) searchPanel.SetActive(false);
                if (searchInputField != null) searchInputField.text = "";
            }
        }
        else
        {
            Debug.LogWarning($"[Search Fail] '{searchName}' �̸��� ���� ��Ŀ�� ã�� �� �����ϴ�.");
            ShowFailedPanel();
            // TODO: ����ڿ��� ��Ŀ�� ã�� �� ���ٴ� �޽��� ��� UI ���� �߰�
        }
    }

    // ======================================================================
    // 7. �˻� ���� �帧 ���� ���� (�߰�)
    // ======================================================================

    // �˻� ���� �� ȣ�� (���� �г� ����)
    private void ShowFailedPanel()
    {
        if (failedPanel != null)
        {
            // �˻� �г��� ����� ���� �г��� ���ϴ�.
            if (searchPanel != null) searchPanel.SetActive(false);
            failedPanel.SetActive(true);
        }
    }

    // ���� �гο��� 'Ȯ��' ��ư Ŭ�� �� ȣ��
    private void OnFailedConfirmClicked()
    {
        // ���� �г��� �ݰ�
        if (failedPanel != null) failedPanel.SetActive(false);
        // �˻� �г��� �ٽ� ���ϴ�.
        if (searchPanel != null) searchPanel.SetActive(true);
        // �Է� �ʵ� �ʱ�ȭ
        if (searchInputField != null) searchInputField.text = "";
    }

    // Helper �Լ�: �̸����� 3D ��Ŀ�� ã�� �Լ�
    private GameObject Find3DMarkerByName(string name)
    {
        // ARMarkerData�� �پ��ִ� ��� 3D ��Ŀ�� ��ȸ�մϴ�.
        ARMarkerData[] allMarkers = FindObjectsByType<ARMarkerData>(FindObjectsSortMode.None);

        foreach (var arData in allMarkers)
        {
            // ARMarkerData�� ����� �̸�(fullMarkerData.Name)�� ����Ͽ� �˻��մϴ�.
            // ��ҹ��� ���� ���� �˻� (OrdinalIgnoreCase)
            if (arData.fullMarkerData != null && arData.fullMarkerData.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
            {
                // **3D ������Ʈ�� ��ġ�Ǿ����� Ȯ�� (linked3DMarker�� ������� �ʴ� ���)**
                // arData�� �پ��ִ� GameObject�� 3D ��Ŀ�Դϴ�.
                if (arData.gameObject.activeInHierarchy)
                {
                    return arData.gameObject;
                }
            }
        }

        return null;
    }

    public void HideEditPanel()
    {
        if (detailEditPanel != null)
        {
            detailEditPanel.SetActive(false);
        }
    }
}