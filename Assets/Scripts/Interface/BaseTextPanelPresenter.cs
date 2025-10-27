using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class BaseTextPanelPresenter : ITextPanelPresenter
{
    private ITextPanelView view;
    private TextPanelModel model;

    public async UniTask InitializeAsync(ITextPanelView iView, TextPanelModel model)
    {
        this.model = model;
        this.view = iView;
        view.Initialize(this);
        view.SetText(model.text);
        view.SetActive(true);
        await UniTask.CompletedTask;
    }

    public int GetDepth()
        => model.depth;

    public void Release()
        => view.SetActive(false);

    public bool TryGetPointingLink(Vector2 mousePosition, out TMP_LinkInfo linkInfo)
        => view.TryGetPointingLink(mousePosition, out linkInfo);

    public TextPanelAnchorState GetAnchorState()
        => model.AnchorState;

    public void SetAnchorState(TextPanelAnchorState anchorState)
        => model.SetAnchorState(anchorState);

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

    public void SubscribeOnEnter(UnityAction<ITextPanelPresenter> onEnter)
        => view.SubscribeOnEnter(onEnter);

    public void SubscribeOnExit(UnityAction<ITextPanelPresenter> onExit)
        => view.SubscribeOnExit(onExit);
}