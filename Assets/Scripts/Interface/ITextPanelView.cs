using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ViewRectData
{
    public Vector2 pivot;
    public Vector2 position;

    public ViewRectData(Vector2 pivot, Vector2 position)
    {
        this.pivot = pivot;
        this.position = position;
    }

    public static ViewRectData Empty
        => new(Vector2.zero, Vector2.zero);
}

public interface ITextPanelView : ITextPanelLinkAcceser, ITextPanelDisplayController, IPointerEnterHandler, IPointerExitHandler
{
    public void SetRoot(Transform root);

    public void SetActive(bool active);

    public void SubscribeOnPanelEnter(UnityAction<ITextPanelView> onPanelEnter);

    public void SubscribeOnPanelExit(UnityAction<ITextPanelView> onPanelExit);
}
