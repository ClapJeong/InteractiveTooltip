using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DefaultTextPanelViewSet
{
    public BaseTextPanelView view;
    public string key;
}

public class MousePointDetector : MonoBehaviour
{
    [SerializeField] private List<BaseTextPanelView> defaultTextPanelViews = new();

    [Space(5)]
    [SerializeField] private LinkDataSO linkDataSO;
    [SerializeField] private BaseTextPanelView prefab;
    [SerializeField] private Transform activeRoot;
    [SerializeField] private Transform deactiveRoot;

    private TextPanelService textPanelService;
    private TextPanelTimer timer;

    private readonly float anchoringDuratioin = 1.5f;
    private readonly float exitingDuration = 0.3f;

    private ITextPanelPresenter enteredPresenter;
    private ITextPanelPresenter exitedPresenter;

    private void Awake()
    {
        textPanelService = new TextPanelService(
            prefab,
            activeRoot,
            deactiveRoot,
            OnPointEnterTextPanel,
            OnPointExitTextPanel);
        timer = new TextPanelTimer();

        InitializeDefaultTextPanels();
    }

    private void Update()
    {
        UpdatePointerDetecting();
    }

    private void LateUpdate()
    {
        UpdatePanelEnterExit();
    }

    private void InitializeDefaultTextPanels()
    {
        foreach (var defaultView in defaultTextPanelViews)
            if (linkDataSO.TryGetData(defaultView.defaultKey, out var linkData))
                textPanelService.InitializeHierarchyViewAsync(defaultView, linkData).Forget();
    }

    private void UpdatePointerDetecting()
    {
        var isAnyTextPanelDetected = false;
        var mousePoint = Input.mousePosition;
        foreach (var presenter in textPanelService.GetEnablePresenters())
        {
            if (presenter.TryGetPointingLink(mousePoint, out var linkInfo) &&
               linkDataSO.TryGetData(linkInfo.GetLinkID(), out var linkData))
            {
                isAnyTextPanelDetected = true;

                var newDepth = presenter.GetDepth() + 1;
                var hasPanel = textPanelService.HasTextPanel(newDepth, linkData);

                if (!hasPanel)
                {
                    DeleteTextPanesOverDepth(newDepth);

                    var pivot = new Vector2(0.5f, 0.0f);
                    var offset = new Vector2(0.0f, 10.0f);
                    var position = presenter.GetLinkScreenPosition(linkInfo, TextDirection.Up) + offset;
                    var viewRectData = new ViewRectData(pivot, position);
                    CreateNewTextPanelAsync(linkData, newDepth, viewRectData).Forget();
                    break;
                }
            }
        }

        if (isAnyTextPanelDetected == false)
            OnFailedDetectingWord();
    }

    private void OnFailedDetectingWord()
    {
        if (textPanelService.TryGetTopDepthPresenter(out var topPresenter))
        {
            switch (topPresenter.GetAnchorState())
            {
                case TextPanelAnchorState.None:
                    textPanelService.Release(topPresenter);
                    break;

                case TextPanelAnchorState.WordAnchored:
                    {
                        if (timer.HasTimer(topPresenter) == false)
                        {
                            timer.PlayTimer(topPresenter,
                                                    exitingDuration,
                                                    onProgress: null,
                                                    onComplete: () => textPanelService.Release(topPresenter),
                                                    onCanceled: null);
                        }
                    }

                    break;

                case TextPanelAnchorState.PanelAnchored: return;

                default: throw new System.NotImplementedException();
            }
        }
    }

    private void DeleteTextPanesOverDepth(int inclusiveDepth)
    {
        var presenters = textPanelService.GetMoreDepthPresenters(inclusiveDepth).ToList();
        foreach (var presenter in presenters)
        {
            if (timer.HasTimer(presenter))
                timer.CancelTimer(presenter);
            textPanelService.Release(presenter);
        }
    }

    private async UniTask CreateNewTextPanelAsync(LinkData linkData, int depth, ViewRectData rectData)
    {
        var view = await textPanelService.GetViewAsync();
        view.SetRoot(deactiveRoot);

        var presenter = await textPanelService.InitializeViewAsync(view, linkData, depth, rectData);
        presenter.RefreshRectTransform();

        timer.PlayTimer(presenter,
            anchoringDuratioin,
            presenter.OnAnchorProgress,
            () => OnWordAnchorComplete(presenter),
            () => OnWordAnchorCancled(presenter));
    }

    private void OnWordAnchorComplete(ITextPanelPresenter presenter)
    {
        presenter.SetAnchorState(TextPanelAnchorState.WordAnchored);
    }

    private void OnWordAnchorCancled(ITextPanelPresenter presenter)
    {

    }

    private void OnPointEnterTextPanel(ITextPanelPresenter presenter)
    {
        if (presenter.GetDepth() > -1)
            enteredPresenter = presenter;
    }

    private void OnPointExitTextPanel(ITextPanelPresenter presenter)
    {
        if (presenter.GetDepth() > -1)
            exitedPresenter = presenter;
    }

    private void UpdatePanelEnterExit()
    {
        if (enteredPresenter != null &&
            timer.HasTimer(enteredPresenter))
        {
            timer.CancelTimer(enteredPresenter);
            enteredPresenter.SetAnchorState(TextPanelAnchorState.PanelAnchored);
        }
        else if (exitedPresenter != null &&
                 enteredPresenter == null)
        {
            DeleteTextPanesOverDepth(exitedPresenter.GetDepth());
        }
        enteredPresenter = null;
        exitedPresenter = null;
    }
}
