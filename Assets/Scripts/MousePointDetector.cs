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
                textPanelService.InitializeViewAsync(defaultView, Vector2.zero, Vector2.zero, linkData, -1).Forget();
    }

    private void UpdatePointerDetecting()
    {
        var isAnyTextPanelDetected = false;
        var mousePoint = Input.mousePosition;
        foreach (var presenter in textPanelService.GetEnablePresenters())
        {
            if (presenter.TryGetPointingLink(mousePoint, out var linkInfo) && //포인트한 곳에 링크가 있음
               linkDataSO.TryGetData(linkInfo.GetLinkID(), out var linkData)) //해당 링크에 대한 데이터가 있음
            {
                isAnyTextPanelDetected = true;
                //새로 만들 뎁스에 이미 링크에 관한 패널이 있으면 아무것도 안함
                //새로 만들 뎁스에 링크에 관한 패널이 없으면 그거랑 그 이상 패널들 지우고 새로 만들기
                var newDepth = presenter.GetDepth() + 1;
                var hasPanel = textPanelService.HasTextPanel(newDepth, linkData);
                Debug.Log($"{Time.frameCount} hasPanel: {hasPanel}");
                if (!hasPanel)
                {
                    DeleteTextPanesOverDepth(newDepth);

                    var position = presenter.GetLinkScreenPosition(linkInfo, TextDirection.Up);
                    CreateNewTextPanelAsync(linkData, newDepth, new Vector2(0.5f, 0.0f), position).Forget();
                    break;
                }
            }
        }

        if (isAnyTextPanelDetected == false)
        {
            OnFailedDetectingWord();
        }
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
                            timer.PlayTimer(topPresenter,
                                                    exitingDuration,
                                                    null,
                                                    () => textPanelService.Release(topPresenter),
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
        var presenters = textPanelService.GetMoreDepthPresenters(inclusiveDepth).ToList();
        foreach (var presenter in presenters)
        {
            if (timer.HasTimer(presenter))
                timer.CancelTimer(presenter);
            textPanelService.Release(presenter);
        }
    }

    private async UniTask CreateNewTextPanelAsync(LinkData linkData, int depth, Vector2 pivot, Vector2 position)
    {
        var view = await textPanelService.GetViewAsync();
        view.SetRoot(deactiveRoot);

        var presenter = await textPanelService.InitializeViewAsync(view, pivot, position, linkData, depth);
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
        Debug.Log("Anchoring Canceled!");
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
            if (enteredPresenter.GetDepth() > -1 &&
                timer.HasTimer(enteredPresenter))
            {
                timer.CancelTimer(enteredPresenter);
                enteredPresenter.SetAnchorState(TextPanelAnchorState.PanelAnchored);
            }
        }
        else if (exitedPresenter != null &&
                 exitedPresenter.GetDepth() > -1 &&
                 enteredPresenter == null)
        {
            DeleteTextPanesOverDepth(exitedPresenter.GetDepth());
        }
        enteredPresenter = null;
        exitedPresenter = null;
    }
}
