using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;


public class BaseTextPanelPresenter : ITextPanelPresenter
{
    private ITextPanelView view;
    private TextPanelModel model;

    public async UniTask InitializeAsync(ITextPanelView iView, Transform root, string text, int depth)
    {
        this.view = iView;
        model = new TextPanelModel(text, depth);
        view.GetTMP().text = text;
        view.GetRectTransform().SetParent(root);
        view.GetRectTransform().gameObject.SetActive(true);
        await UniTask.CompletedTask;
    }

    public int GetDepth()
        => model.depth;

    public TextMeshProUGUI GetTMP()
        => view.GetTMP();

    public RectTransform GetRectTransform()
        => view.GetRectTransform();

    public void Release(Transform deactiveRoot)
    {
        view.GetRectTransform().SetParent(deactiveRoot);
        view.GetRectTransform().gameObject.SetActive(false);
    }

    public bool TryGetPointingLink(Vector2 mousePosition, out TMP_LinkInfo linkInfo)
    {
        var tmp = view.GetTMP();
        var linkIndex = TMP_TextUtilities.FindIntersectingLink(tmp, mousePosition, null);
        if (linkIndex != -1)
        {
            linkInfo = tmp.textInfo.linkInfo[linkIndex];
            return true;
        }
        else
        {
            linkInfo = new TMP_LinkInfo();
            return false;
        }
    }

    public TextPanelAnchorState GetAnchorState()
        => model.AnchorState;

    public void SetAnchorState(TextPanelAnchorState anchorState)
        => model.SetAnchorState(anchorState);
}