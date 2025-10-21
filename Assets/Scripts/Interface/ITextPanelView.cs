using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public interface ITextPanelView : IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform GetRectTransform();

    public TextMeshProUGUI GetTMP();

    public void SubscribeOnPanelEnter(UnityAction<ITextPanelView> onPanelEnter);

    public void SubscribeOnPanelExit(UnityAction<ITextPanelView> onPanelExit);

    public Image GetProgressImage();
}
