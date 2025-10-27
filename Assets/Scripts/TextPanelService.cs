using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TextPanelService
{
    private readonly Dictionary<ITextPanelPresenter, ITextPanelView> activePairs = new();
    private readonly Queue<ITextPanelView> deactiveViews = new();

    private readonly TextPanelFactory factory;

    public TextPanelService(
        BaseTextPanelView prefab,
        Transform activeRoot,
        Transform deactiveRoot,
        UnityAction<ITextPanelPresenter> onPointEnter,
        UnityAction<ITextPanelPresenter> onPointExit)
    {
        factory = new TextPanelFactory(
    prefab,
    activeRoot,
    deactiveRoot,
    onPointEnter,
    onPointExit);
    }

    public async UniTask<ITextPanelView> GetViewAsync()
    {
        var isUsableView = deactiveViews.Count == 0;
        var createdView = isUsableView ? await factory.CreateAsync()
                                       : deactiveViews.Dequeue();
        return createdView;
    }

    public void Release(ITextPanelPresenter presenter, Transform root = null)
    {
        factory.Release(presenter, root);
        var view = activePairs[presenter];
        activePairs.Remove(presenter);
        deactiveViews.Enqueue(view);
    }

    public async UniTask<ITextPanelPresenter> InitializeViewAsync(ITextPanelView view, Vector2 pivot, Vector2 position, LinkData linkData, int depth)
    {
        var presenter = await factory.InitializeViewAsync(view, pivot, position, linkData, depth);
        activePairs.Add(presenter, view);
        return presenter;
    }

    public IEnumerable<ITextPanelPresenter> GetEnablePresenters()
        => activePairs.Keys;

    public bool TryGetTopDepthPresenter(out ITextPanelPresenter topPresenter)
    {
        topPresenter = activePairs.Count > 0 ? activePairs.Keys.OrderByDescending(presenter => presenter.GetDepth()).First()
                                             : null;
        if (topPresenter.GetDepth() == -1)
            topPresenter = null;

        return topPresenter != null;
    }

    public IEnumerable<ITextPanelPresenter> GetMoreDepthPresenters(int inclusiveDepth)
        => activePairs.Keys.Where(presenter => presenter.GetDepth() >= inclusiveDepth);

    public bool HasTextPanel(int depth, LinkData linkData)
    {
        var lehu = activePairs.Keys.Any(existPresenter =>
        {
            var sameDepth = existPresenter.GetDepth() == depth;
            var sameLink = existPresenter.IsSameLink(linkData.Key);
            return sameDepth && sameLink;
        });

        return lehu;
    }
}
