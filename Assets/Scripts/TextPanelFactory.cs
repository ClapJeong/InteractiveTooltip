using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextPanelFactory
{
    private readonly List<BaseTextPanelPresenter> defaultPresenters = new();
    public List<BaseTextPanelPresenter> DefaultPresenters => defaultPresenters;

    private readonly Dictionary<BaseTextPanelPresenter, BaseTextPanelView> activePairs = new();
    private readonly Queue<BaseTextPanelView> deactiveViews = new();

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
        this.onTextPanelExit = onTextPanelExit;
        this.onTextPanelEnter = onTextPanelEnter;
        this.onTextPanelExit = onTextPanelExit;
    }

    public IEnumerable<BaseTextPanelPresenter> GetEnablePresenters()
        => activePairs.Keys;

    public async UniTask<BaseTextPanelPresenter> Get(string key, string text, int depth, Vector2 position)
    {
        var createdView = deactiveViews.Count == 0 ? MonoBehaviour.Instantiate(prefab, deactiveRoot)
                                                   : deactiveViews.Dequeue();
        return await Get(createdView, key, text, depth, position);
    }

    public async UniTask<BaseTextPanelPresenter> Get(BaseTextPanelView view, string key, string text, int depth, Vector2 position)
    {
        var presenter = new BaseTextPanelPresenter();
        await presenter.InitializeAsync(
            view,
            deactiveRoot,
            key,
            text,
            depth,
            onTextPanelEnter,
            onTextPanelExit);

        await UniTask.NextFrame();
        var rectTransform = presenter.GetRectTransform();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        var offset = rectTransform.GetAnchoredOffset(new Vector2(0.5f, 1.0f), 10.0f);
        rectTransform.position = position + offset;

        rectTransform.SetParent(activeRoot);
        activePairs[presenter] = view;
        return presenter;
    }

    public async UniTask<BaseTextPanelPresenter> InitializeDefaultTextPanle(BaseTextPanelView view, string key, string text)
    {
        var presenter = new BaseTextPanelPresenter();
        await presenter.InitializeAsync(view,
            activeRoot,
            key,
            text,
            -1,
            onTextPanelEnter,
            onTextPanelExit);
        defaultPresenters.Add(presenter);

        return presenter;
    }

    public void Release(BaseTextPanelPresenter presenter)
    {
        presenter.Release(deactiveRoot);
        deactiveViews.Enqueue(activePairs[presenter]);
        activePairs.Remove(presenter);
    }

    public bool TryGetTopPresenter(out BaseTextPanelPresenter topPresenter)
    {
        if (activePairs.Count > 0)
        {
            topPresenter = activePairs.OrderByDescending(pair => pair.Key.GetDepth()).FirstOrDefault().Key;
            return true;
        }
        else
        {
            topPresenter = null;
            return false;
        }
    }

    public bool HasDepth(int depth)
        => activePairs.Any(pair => pair.Key.GetDepth() == depth);
}
