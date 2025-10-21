using TMPro;
using UnityEngine;

public class BaseTextPanelView : MonoBehaviour, ITextPanelView
{
    public string defaultKey;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    public TextMeshProUGUI GetTMP() => textMeshProUGUI;

    public RectTransform GetRectTransform() => rectTransform;
}
