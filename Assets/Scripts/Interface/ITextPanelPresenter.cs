using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum TextDirection
{
    Up,
    Down,
}

public interface ITextPanelLinkAcceser
{
    public bool TryGetPointingLink(Vector2 mousePosition, out TMP_LinkInfo linkInfo);
    public Vector2 GetLinkScreenPosition(TMP_LinkInfo linkInfo, TextDirection direction);
}

public interface ITextPanelDisplayController
{
    public void SetText(string text);

    public void SetPosition(Vector2 position);

    public void SetPivot(Vector2 anchor);

    public void RefreshRectTransform();

    public void OnAnchorProgress(float normalizedValue);
}

public interface ITextPanelPresenter : ITextPanelLinkAcceser, ITextPanelDisplayController
{
    public UniTask InitializeAsync(
        ITextPanelView iView,
        LinkData linkData,
        int depth,
        UnityAction<ITextPanelPresenter> onPanelEnter,
        UnityAction<ITextPanelPresenter> onPanelExit);

    public void Release();

    #region [ View ]
    public void SetRoot(Transform root);

    public void OnPanelEnter(ITextPanelView view);

    public void OnPanelExit(ITextPanelView view);
    #endregion

    #region [ Model ]

    public TextPanelAnchorState GetAnchorState();

    public void SetAnchorState(TextPanelAnchorState anchorState);

    public int GetDepth();
    #endregion

    public bool IsSameLink(string key);
}
