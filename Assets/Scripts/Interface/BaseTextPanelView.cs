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
    [SerializeField] private Outline outLine;
    private ITextPanelPresenter presenter;

    public UnityAction<ITextPanelPresenter> onPanelEnter;
    public UnityAction<ITextPanelPresenter> onPanelExit;

    public void SetRoot(Transform root)
        => rectTransform.SetParent(root);

    public void SetText(string text)
        => textMeshProUGUI.text = text;

    public void OnAnchorProgress(float normalizedValue)
    {
        progressImage.fillAmount = normalizedValue;
        outLine.enabled = normalizedValue >= 1.0f;
    }

    public void OnPointerEnter(PointerEventData eventData)
        => onPanelEnter?.Invoke(presenter);

    public void OnPointerExit(PointerEventData eventData)
        => onPanelExit?.Invoke(presenter);

    public void SetActive(bool active)
        => gameObject.SetActive(active);

    public bool TryGetPointingLink(Vector2 mousePosition, out TMP_LinkInfo linkInfo)
    {
        var linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshProUGUI, mousePosition, null);
        var isDetected = linkIndex > -1;
        linkInfo = isDetected ? textMeshProUGUI.textInfo.linkInfo[linkIndex]
                              : new TMP_LinkInfo();
        return isDetected;
    }

    public void RefreshRectTransform()
        => LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

    public Vector2 GetLinkScreenPosition(TMP_LinkInfo linkInfo, TextDirection direction)
        => textMeshProUGUI.GetLinkScreenPosition(linkInfo, direction);

    public void SetPosition(Vector2 position)
        => rectTransform.position = position;

    public void SetPivot(Vector2 pivot)
        => rectTransform.pivot = pivot;

    public void Initialize(ITextPanelPresenter presenter)
    {
        this.presenter = presenter;
        outLine.enabled = presenter.GetDepth() < 0;
    }

    public void SubscribeOnEnter(UnityAction<ITextPanelPresenter> onEnter)
    {
        this.onPanelEnter -= onEnter;
        this.onPanelEnter += onEnter;
    }

    public void SubscribeOnExit(UnityAction<ITextPanelPresenter> onExit)
    {
        this.onPanelExit -= onExit;
        this.onPanelExit += onExit;
    }
}
