using UnityEngine;

public class MarkerColorSelector : MonoBehaviour
{
    public Color CurrentColor { get; private set; } = Color.red;

    public void SelectColor(Color color)
    {
        CurrentColor = color;
    }
}
