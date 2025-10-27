using UnityEngine;
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

public interface ITextPanelView : ITextPanelLinkAcceser, ITextPanelDisplayController, IPointerEnterHandler, IPointerExitHandler, IPointerSubscriber<ITextPanelPresenter>
{
    public void Initialize(ITextPanelPresenter presenter);

    public void SetRoot(Transform root);

    public void SetActive(bool active);
}
