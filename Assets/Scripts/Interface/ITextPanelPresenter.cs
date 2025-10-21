using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public enum TextDirection
{
    Up,
    Down,
}

public interface ITextPanelPresenter
{
    public UniTask InitializeAsync(
        ITextPanelView iView,
        Transform root,
        string key,
        string text,
        int depth,
        UnityAction<ITextPanelPresenter> onPanelEnter,
        UnityAction<ITextPanelPresenter> onPanelExit);

    public void Release(Transform deactiveRoot);

    #region [ View ]
    public RectTransform GetRectTransform();

    public TextMeshProUGUI GetTMP();

    public void OnPanelEnter(ITextPanelView view);

    public void OnPanelExit(ITextPanelView view);
    #endregion

    #region [ Model ]
    public int GetDepth();

    public TextPanelAnchorState GetAnchorState();

    public void SetAnchorState(TextPanelAnchorState anchorState);

    public void OnAnchorProgress(float normalizedValue);
    #endregion

    public bool IsSameLink(string key);

    public bool TryGetPointingLink(Vector2 mousePosition, out TMP_LinkInfo linkInfo);
}
