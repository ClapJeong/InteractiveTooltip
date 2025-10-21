using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    private void Awake()
    {
        factory = new TextPanelFactory(prefab, activeRoot, deactiveRoot);
        InitializeDefaultTextPanels();
    }

    private void Update()
    {
        UpdatePointerDetecting();
    }

    private void InitializeDefaultTextPanels()
    {
        foreach (var defaultView in defaultTextPanelViews)
            if (linkDataSO.TryGetData(defaultView.defaultKey, out var linkData))
                factory.InitializeDefaultTextPanle(defaultView, linkData.Description).Forget();
    }

    private void UpdatePointerDetecting()
    {
        var isAnyTextPanelDetected = false;
        var detectedLinkInfo = new TMP_LinkInfo();
        var mousePoint = Input.mousePosition;
        DetectPresenters(factory.DefaultPresenters);
        DetectPresenters(factory.GetEnablePresenters());

        if (isAnyTextPanelDetected == false &&
            factory.TryGetTopPresenter(out var topPresenter))
        {
            if (topPresenter.GetAnchorState() != TextPanelAnchorState.Anchored)
                factory.Release(topPresenter);
        }

        void DetectPresenters(IEnumerable<BaseTextPanelPresenter> presenters)
        {
            foreach (var presenter in presenters)
            {
                if (presenter.TryGetPointingLink(mousePoint, out var linkInfo) &&
                   linkDataSO.TryGetData(linkInfo.GetLinkID(), out var linkData))
                {
                    var newDepth = presenter.GetDepth() + 1;
                    if (factory.HasDepth(newDepth) == false)
                    {
                        DeleteTextPanesOverDepth(newDepth);
                        CreateNewTextPanel(newDepth, linkData, presenter.GetTMP().GetWordPosition(
                            linkInfo.linkTextfirstCharacterIndex,
                            linkInfo.linkTextfirstCharacterIndex + linkInfo.linkIdLength,
                            TextDirection.Up));
                    }

                    isAnyTextPanelDetected = true;
                }
            }
        }
    }

    private void DeleteTextPanesOverDepth(int inclusiveDepth)
    {
        var presenters = factory.GetEnablePresenters().Where(presenter => presenter.GetDepth() >= inclusiveDepth).ToList();
        foreach (var presenter in presenters)
            factory.Release(presenter);
    }

    private void CreateNewTextPanel(int depth, LinkData linkData, Vector2 position)
    {
        factory.Get(linkData.Description, depth, position).Forget();
    }
}
