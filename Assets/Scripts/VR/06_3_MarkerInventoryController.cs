using UnityEngine;

public class MarkerInventoryController : MonoBehaviour
{
    [Header("ScrollView")]
    [SerializeField] private Transform contentRoot;

    [Header("Prefabs")]
    [SerializeField] private MarkerLaunchPadItem launchPadPrefab;

    public MarkerLaunchPadItem AddMarkerLaunchPad(T6MarkerItemData data)
    {
        MarkerLaunchPadItem item =
            Instantiate(launchPadPrefab, contentRoot);

        item.Bind(data);
        return item;
    }
}
