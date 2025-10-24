using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public interface ITextPanelView : IPointerEnterHandler, IPointerExitHandler
{
    public void SetRoot(Transform root);

    public void SetPosition(Vector2 position);

    public void SetPivot(Vector2 anchor);

    public void SetText(string text);

    public void SetActive(bool active);

    public void SubscribeOnPanelEnter(UnityAction<ITextPanelView> onPanelEnter);

    public void SubscribeOnPanelExit(UnityAction<ITextPanelView> onPanelExit);

    public void SetAnchoringProgress(float normalizedValue);

    public bool TryGetPointingLink(Vector2 mousePosition, out TMP_LinkInfo linkInfo);

    public void RefreshRectTransform();

    public Vector2 GetLinkScreenPosition(TMP_LinkInfo linkInfo, TextDirection direction);
}
