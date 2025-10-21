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

    private TextPanelFactory factory;
    private TextPanelTimer timer;

    private readonly float anchoringDuratioin = 1.5f;
    private readonly float exitingDuration = 0.3f;

    private ITextPanelPresenter enteredPresenter;
    private ITextPanelPresenter exitedPresenter;

    private void Awake()
    {
        factory = new TextPanelFactory(
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
                factory.InitializeDefaultTextPanle(defaultView, linkData.Key, linkData.Description).Forget();
    }

    private void UpdatePointerDetecting()
    {
        var isAnyTextPanelDetected = false;
        var mousePoint = Input.mousePosition;
        DetectPresenters(factory.DefaultPresenters);
        DetectPresenters(factory.GetEnablePresenters());

        if (isAnyTextPanelDetected == false)
        {
            OnFailedDetectingWord();
        }

        void DetectPresenters(IEnumerable<BaseTextPanelPresenter> presenters)
        {
            foreach (var presenter in presenters)
            {
                if (presenter.TryGetPointingLink(mousePoint, out var linkInfo) &&
                   linkDataSO.TryGetData(linkInfo.GetLinkID(), out var linkData))
                {
                    var newDepth = presenter.GetDepth() + 1;
                    if (factory.HasDepth(newDepth))
                    {
                        if (factory
                            .GetEnablePresenters()
                            .Where(presenter => presenter.GetDepth() == newDepth)
                            .First()
                            .IsSameLink(linkData.Key) == false)
                        {
                            DeleteTextPanesOverDepth(newDepth);
                            var position = presenter.GetTMP().GetWordPosition(
                            linkInfo.linkTextfirstCharacterIndex,
                            linkInfo.linkTextfirstCharacterIndex + linkInfo.linkIdLength,
                            TextDirection.Up);
                            CreateNewTextPanelAsync(newDepth, linkData, position).Forget();
                        }
                    }
                    else
                    {
                        var position = presenter.GetTMP().GetWordPosition(
                            linkInfo.linkTextfirstCharacterIndex,
                            linkInfo.linkTextfirstCharacterIndex + linkInfo.linkIdLength,
                            TextDirection.Up);
                        CreateNewTextPanelAsync(newDepth, linkData, position).Forget();
                    }
                    isAnyTextPanelDetected = true;
                    break;
                }
            }
        }
    }

    private void OnFailedDetectingWord()
    {
        if (factory.TryGetTopPresenter(out var topPresenter))
        {
            switch (topPresenter.GetAnchorState())
            {
                case TextPanelAnchorState.None:
                    factory.Release(topPresenter);
                    break;

                case TextPanelAnchorState.WordAnchored:
                    {
                        if (timer.HasTimer(topPresenter) == false)
                            timer.PlayTimer(topPresenter,
                                                    exitingDuration,
                                                    null,
                                                    () => factory.Release(topPresenter),
                                                    null);
                    }

                    break;

                case TextPanelAnchorState.PanelAnchored:
                    break;
            }
        }
    }

    private void DeleteTextPanesOverDepth(int inclusiveDepth)
    {
        var presenters = factory.GetEnablePresenters().Where(presenter => presenter.GetDepth() >= inclusiveDepth).ToList();
        foreach (var presenter in presenters)
        {
            if (timer.HasTimer(presenter))
                timer.CancelTimer(presenter);
            factory.Release(presenter);
        }
    }

    private async UniTask CreateNewTextPanelAsync(int depth, LinkData linkData, Vector2 position)
    {
        var presenter = await factory.Get(linkData.Key, linkData.Description, depth, position);
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
        timer.CancelTimer(presenter);
    }

    private void OnPointEnterTextPanel(ITextPanelPresenter presenter)
    {
        enteredPresenter = presenter;
    }

    private void OnPointExitTextPanel(ITextPanelPresenter presenter)
    {
        exitedPresenter = presenter;
    }

    private void UpdatePanelEnterExit()
    {
        if (enteredPresenter != null)
        {
            if (timer.HasTimer(enteredPresenter))
            {
                timer.CancelTimer(enteredPresenter);
                enteredPresenter.SetAnchorState(TextPanelAnchorState.PanelAnchored);
            }
        }
        else if (exitedPresenter != null && enteredPresenter == null)
        {
            DeleteTextPanesOverDepth(0);
        }
        enteredPresenter = null;
        exitedPresenter = null;
    }
}
