using UnityEngine;
using TMPro;

public class T6SpaceHeaderUI : MonoBehaviour
{
    [Header("UI - Space Name")]
    public TMP_Text txtSpaceName;

    // SpaceDetail 수신 시 UI 업데이트
    public void SetSpaceDetail(T6SpaceDetail detail)
    {
        if (detail == null) return;

        txtSpaceName.text = detail.meta.name;
    }
}
