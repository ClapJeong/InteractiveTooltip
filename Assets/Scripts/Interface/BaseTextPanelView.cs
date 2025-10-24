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

    public void SetRoot(Transform root)
        => rectTransform.SetParent(root);

    public void SetText(string text)
        => textMeshProUGUI.text = text;

    public void SetAnchoringProgress(float normalizedValue)
        => progressImage.fillAmount = normalizedValue;

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

    public void SetActive(bool active)
        => gameObject.SetActive(active);

    public bool TryGetPointingLink(Vector2 mousePosition, out TMP_LinkInfo linkInfo)
    {
        var linkIndex = TMP_TextUtilities.FindIntersectingLink(textMeshProUGUI, mousePosition, null);
        if (linkIndex != -1)
        {
            linkInfo = textMeshProUGUI.textInfo.linkInfo[linkIndex];
            return true;
        }
        else
        {
            linkInfo = new TMP_LinkInfo();
            return false;
        }
    }

    public void RefreshRectTransform()
        => LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

    public Vector2 GetLinkScreenPosition(TMP_LinkInfo linkInfo, TextDirection direction)
        => textMeshProUGUI.GetLinkScreenPosition(linkInfo, direction);

    public void SetPosition(Vector2 position)
        => rectTransform.position = position;

    public void SetPivot(Vector2 pivot)
        => rectTransform.pivot = pivot;
}
