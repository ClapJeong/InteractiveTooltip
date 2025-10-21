using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseTextPanelView : MonoBehaviour, ITextPanelView
{
    public string defaultKey;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private Image progressImage;

    public UnityAction<ITextPanelView> onPanelEnter;
    public UnityAction<ITextPanelView> onPanelExit;

    public TextMeshProUGUI GetTMP() => textMeshProUGUI;

    public RectTransform GetRectTransform() => rectTransform;

    public void OnPointerEnter(PointerEventData eventData)
        => onPanelEnter?.Invoke(this);

    public void OnPointerExit(PointerEventData eventData)
        => onPanelExit?.Invoke(this);

    public void SubscribeOnPanelEnter(UnityAction<ITextPanelView> onPanelEnter)
    {
        this.onPanelEnter -= onPanelEnter;
        this.onPanelEnter += onPanelEnter;
    }

    public void SubscribeOnPanelExit(UnityAction<ITextPanelView> onPanelExit)
    {
        this.onPanelExit -= onPanelExit;
        this.onPanelExit += onPanelExit;
    }

    public Image GetProgressImage()
        => progressImage;
}
