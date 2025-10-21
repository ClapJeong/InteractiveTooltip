using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public enum TextDirection
{
    Up,
    Down,
}

public interface ITextPanelPresenter
{
    public UniTask InitializeAsync(ITextPanelView iView, Transform root, string text, int depth);

    public void Release(Transform deactiveRoot);

    public RectTransform GetRectTransform();

    public TextMeshProUGUI GetTMP();

    public int GetDepth();

    public TextPanelAnchorState GetAnchorState();

    public void SetAnchorState(TextPanelAnchorState anchorState);

    public bool TryGetPointingLink(Vector2 mousePosition, out TMP_LinkInfo linkInfo);
}
