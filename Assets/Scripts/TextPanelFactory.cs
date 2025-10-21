using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
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

    public TextPanelFactory(BaseTextPanelView prefab, Transform activeRoot, Transform deactiveRoot)
    {
        this.prefab = prefab;
        this.activeRoot = activeRoot;
        this.deactiveRoot = deactiveRoot;
    }

    public IEnumerable<BaseTextPanelPresenter> GetEnablePresenters()
        => activePairs.Keys;

    public async UniTask<BaseTextPanelPresenter> Get(string text, int depth, Vector2 position)
    {
        var createdView = deactiveViews.Count == 0 ? MonoBehaviour.Instantiate(prefab, deactiveRoot)
                                                   : deactiveViews.Dequeue();
        return await Get(createdView, text, depth, position);
    }

    public async UniTask<BaseTextPanelPresenter> Get(BaseTextPanelView view, string text, int depth, Vector2 position)
    {
        var presenter = new BaseTextPanelPresenter();
        await presenter.InitializeAsync(view, deactiveRoot, text, depth);

        await UniTask.NextFrame();
        var rectTransform = presenter.GetRectTransform();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        var offset = rectTransform.GetAnchoredOffset(new Vector2(0.5f, 1.0f), 10.0f);
        rectTransform.position = position + offset;

        rectTransform.SetParent(activeRoot);
        activePairs[presenter] = view;
        return presenter;
    }

    public async UniTask<BaseTextPanelPresenter> InitializeDefaultTextPanle(BaseTextPanelView view, string text)
    {
        var presenter = new BaseTextPanelPresenter();
        await presenter.InitializeAsync(view, activeRoot, text, -1);
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
