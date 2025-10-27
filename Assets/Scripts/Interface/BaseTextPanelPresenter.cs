using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class BaseTextPanelPresenter : ITextPanelPresenter
{
    private ITextPanelView view;
    private TextPanelModel model;

    private UnityAction<ITextPanelPresenter> onPanelEnter;
    private UnityAction<ITextPanelPresenter> onPanelExit;

    public async UniTask InitializeAsync(
        ITextPanelView iView,
        TextPanelModel model,
        UnityAction<ITextPanelPresenter> onPanelEnter,
        UnityAction<ITextPanelPresenter> onPanelExit)
    {
        this.onPanelEnter = onPanelEnter;
        this.onPanelExit = onPanelExit;

        this.model = model;
        this.view = iView;
        view.SetText(model.text);
        view.SetActive(true);
        view.SubscribeOnPanelEnter(OnPanelEnter);
        view.SubscribeOnPanelExit(OnPanelExit);
        await UniTask.CompletedTask;
    }

    public int GetDepth()
        => model.depth;

    public void Release()
    {
        view.SetActive(false);
    }

    public bool TryGetPointingLink(Vector2 mousePosition, out TMP_LinkInfo linkInfo)
        => view.TryGetPointingLink(mousePosition, out linkInfo);

    public TextPanelAnchorState GetAnchorState()
        => model.AnchorState;

    public void SetAnchorState(TextPanelAnchorState anchorState)
        => model.SetAnchorState(anchorState);

    public void OnPanelEnter(ITextPanelView view)
        => onPanelEnter?.Invoke(this);

    public void OnPanelExit(ITextPanelView view)
        => onPanelExit?.Invoke(this);

    public bool IsSameLink(LinkData linkData)
        => model.key == linkData.Key;

    public void SetPosition(Vector2 position)
        => view.SetPosition(position);

    public void SetPivot(Vector2 pivot)
        => view.SetPivot(pivot);

    public void SetRoot(Transform root)
        => view.SetRoot(root);

    public void SetText(string text)
        => view.SetText(text);

    public void OnAnchorProgress(float normalizedValue)
        => view.OnAnchorProgress(normalizedValue);

    public void RefreshRectTransform()
        => view.RefreshRectTransform();

    public Vector2 GetLinkScreenPosition(TMP_LinkInfo linkInfo, TextDirection direction)
        => view.GetLinkScreenPosition(linkInfo, direction);
}