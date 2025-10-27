using Cysharp.Threading.Tasks;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class TextPanelFactory
{
    private readonly BaseTextPanelView prefab;

    private readonly Transform activeRoot;
    private readonly Transform deactiveRoot;

    private readonly UnityAction<ITextPanelPresenter> onTextPanelEnter;
    private readonly UnityAction<ITextPanelPresenter> onTextPanelExit;

    public TextPanelFactory(BaseTextPanelView prefab,
        Transform activeRoot,
        Transform deactiveRoot,
        UnityAction<ITextPanelPresenter> onTextPanelEnter,
        UnityAction<ITextPanelPresenter> onTextPanelExit)
    {
        this.prefab = prefab;
        this.activeRoot = activeRoot;
        this.deactiveRoot = deactiveRoot;
        this.onTextPanelEnter = onTextPanelEnter;
        this.onTextPanelExit = onTextPanelExit;
    }


    public async UniTask<BaseTextPanelView> CreateAsync(Transform root = null)
    {
        var operater = MonoBehaviour.InstantiateAsync<BaseTextPanelView>(prefab, root ?? deactiveRoot);
        await operater;
        return operater.Result.First();
    }

    public async UniTask<ITextPanelPresenter> InitializeViewAsync(
        ITextPanelView view,
        LinkData linkData,
        int depth,
        ViewRectData rectData)
    {
        var model = new TextPanelModel(linkData, depth);
        var presenter = new BaseTextPanelPresenter();
        await presenter.InitializeAsync(view, model);
        presenter.SubscribeOnEnter(onTextPanelEnter);
        presenter.SubscribeOnExit(onTextPanelExit);

        if (depth > -1)
        {
            presenter.SetPivot(rectData.pivot);
            presenter.SetPosition(rectData.position);
            presenter.SetRoot(activeRoot);
        }
        return presenter;
    }

    public void Release(ITextPanelPresenter presenter, Transform root = null)
    {
        presenter.Release();
        presenter.SetRoot(root ?? deactiveRoot);
    }
}
