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
        Transform root,
        string key,
        string text,
        int depth,
        UnityAction<ITextPanelPresenter> onPanelEnter,
        UnityAction<ITextPanelPresenter> onPanelExit)
    {
        this.onPanelEnter = onPanelEnter;
        this.onPanelExit = onPanelExit;
        this.view = iView;
        model = new TextPanelModel(key, text, depth);
        view.GetTMP().text = text;
        view.GetRectTransform().SetParent(root);
        view.GetRectTransform().gameObject.SetActive(true);
        view.SubscribeOnPanelEnter(OnPanelEnter);
        view.SubscribeOnPanelExit(OnPanelExit);
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

    public void OnPanelEnter(ITextPanelView view)
        => onPanelEnter?.Invoke(this);

    public void OnPanelExit(ITextPanelView view)
        => onPanelExit?.Invoke(this);

    public void OnAnchorProgress(float normalizedValue)
    {
        view.GetProgressImage().fillAmount = normalizedValue;
    }

    public bool IsSameLink(string key)
        => model.key == key;
}