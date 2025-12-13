using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MarkerEntity : MonoBehaviour
{
    [Header("Data")]
    public T6MarkerData data;

    [Header("Visual")]
    [SerializeField] private MarkerVisualController visual;

    public bool IsSelected { get; private set; }

    private Vector3 baseScale;
    private Vector3 basePosition;

    private void Awake()
    {
        baseScale = transform.localScale;
        basePosition = transform.position;
    }

    public void Initialize(T6MarkerData markerData)
    {
        data = markerData;
        transform.SetPositionAndRotation(data.position, data.rotation);
        visual.SetBaseColor(markerData.color);
    }

    public void Select()
    {
        if (IsSelected) return;

        IsSelected = true;
        visual.SetSelected();
    }

    public void Deselect()
    {
        if (!IsSelected) return;

        IsSelected = false;
        visual.SetNormal();
    }

    public void SyncData()
    {
        if (data != null)
            data.UpdateTransform(transform);
    }
}
