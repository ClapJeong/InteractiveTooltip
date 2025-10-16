using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;

public enum TextPanelDirection
{
    TopLeft = 0, TopCenter = 1, TopRight = 2,
    Left = 10, Right = 11,
    BottomLeft = 20, BottomCenter = 21, BottomRight = 22,
}

public class TooptipManager : MonoBehaviour
{
    public static TooptipManager instance;

    private InteractableTMPController tmpController;
    private TooltipPanelController tooltipPanelController;

    private readonly List<TextPanelSet> detectingTargets = new();
    private readonly Dictionary<WordData, TextPanelSet> createdPanels = new();

    private TextPanelSet pointerEnterPanel;
    private TextPanelSet pointerExitPanel;

    [SerializeField] private WordDataSO wordDataSO;
    [SerializeField] private TextPanelSet textPanelPrefab;

    [SerializeField] private Transform enableRoot;
    [SerializeField] private Transform disableRoot;

    private void Awake()
    {
        instance = this;

        tmpController = new InteractableTMPController(
            OnEnterWord,
            OnLeaveWord);

        tooltipPanelController = new TooltipPanelController(
            textPanelPrefab,
            enableRoot,
            disableRoot,
            OnPointerEnterPanel,
            OnPointerExitPanel);
    }

    private void Update()
    {
        tmpController.UpdateDetecting(detectingTargets);
    }

    private void LateUpdate()
    {
        if (pointerExitPanel != null &&
            pointerEnterPanel == null &&
            createdPanels.ContainsValue(pointerExitPanel))
        {
            DestroyAllCreatedPanels();
        }

        pointerEnterPanel = null;
        pointerExitPanel = null;
    }

    public void Register(TextPanelSet textPanelSet)
    {
        if (!detectingTargets.Contains(textPanelSet))
            detectingTargets.Add(textPanelSet);
    }

    public void Unregister(TextPanelSet textPanelSet)
    {
        if (detectingTargets.Contains(textPanelSet))
            detectingTargets.Remove(textPanelSet);
    }

    private void DestroyAllCreatedPanels()
    {
        tooltipPanelController.DestroyPanel(createdPanels.Values.ToList());
        createdPanels.Clear();
    }

    private async void OnEnterWord(InteractableTMP tmp, TMP_WordInfo wordInfo, int nextDepth)
    {
        if (TryGetWordData(wordInfo.GetWord(), out var wordData))
        {
            if (!createdPanels.ContainsKey(wordData))
            {
                DestroyEqualAndUpperDepthPanels(nextDepth);

                var newPanelSet = await tooltipPanelController.InstantiatePanelAsync(tmp, wordInfo, wordData, nextDepth);
                createdPanels.Add(wordData, newPanelSet);
            }
            else
                tooltipPanelController.StopDestroyingAnchoredPanel();
        }
    }

    private void DestroyEqualAndUpperDepthPanels(int depth)
    {
        var overDepth = createdPanels
                  .Where(pair => pair.Value.deapth >= depth)
                  .ToList();

        tooltipPanelController.DestroyPanel(overDepth.Select(p => p.Value).ToList());

        foreach (var pair in overDepth)
            createdPanels.Remove(pair.Key);
    }

    private void OnLeaveWord(string keyWord)
    {
        if (TryGetWordData(keyWord, out var wordData))
        {
            var targetPanel = createdPanels[wordData];
            switch (targetPanel.anchorState)
            {
                case TextPanelSet.AnchorState.None:
                    {
                        tooltipPanelController.DestroyPanel(targetPanel);
                        createdPanels.Remove(wordData);
                    }
                    break;

                case TextPanelSet.AnchorState.AnchoredWithWord:
                    {
                        tooltipPanelController.DelayDestroyingAnchoredPanel(targetPanel,
                            () => createdPanels.Remove(wordData));
                    }
                    break;

                case TextPanelSet.AnchorState.AnchoredWithPanel:
                    break;
            }
        }
    }

    private bool TryGetWordData(string key, out WordData data)
    {
        data = null;

        foreach (var wordData in wordDataSO.Datas)
            if (wordData.key == key)
            {
                data = wordData;
                return true;
            }

        return false;
    }

    private void OnPointerEnterPanel(TextPanelSet textPanelSet)
    {
        tooltipPanelController.StopDestroyingAnchoredPanel();
        textPanelSet.anchorState = TextPanelSet.AnchorState.AnchoredWithPanel;
        pointerEnterPanel = textPanelSet;
    }

    private void OnPointerExitPanel(TextPanelSet textPanelSet)
        => pointerExitPanel = textPanelSet;
}
