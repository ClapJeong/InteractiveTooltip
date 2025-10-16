using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class TooltipPanelController
{
    private readonly float defaultSpace = 15.0f;
    private readonly ObjectPool<TextPanelSet> textPanelPool;
    private readonly TextPanelSet TextPanelPrefab;
    private readonly Transform enableRoot;
    private readonly Transform disableRoot;
    private readonly UnityAction<TextPanelSet> onPointerEnterPanel;
    private readonly UnityAction<TextPanelSet> onPointerExitPanel;

    private CancellationTokenSource anchoringCTS;
    private CancellationTokenSource destroyingDelayCTS;

    public TooltipPanelController(TextPanelSet textPanelPrefab, Transform enableRoot, Transform disableRoot, UnityAction<TextPanelSet> onPointerEnterPanel, UnityAction<TextPanelSet> onPointerExitPanel)
    {
        this.TextPanelPrefab = textPanelPrefab;
        this.enableRoot = enableRoot;
        this.disableRoot = disableRoot;
        this.onPointerEnterPanel = onPointerEnterPanel;
        this.onPointerExitPanel = onPointerExitPanel;

        textPanelPool = new ObjectPool<TextPanelSet>(
    createFunc: () => MonoBehaviour.Instantiate(TextPanelPrefab, disableRoot),
    actionOnGet: set => set.gameObject.SetActive(true),
    actionOnRelease: set => set.gameObject.SetActive(false),
    actionOnDestroy: set => MonoBehaviour.Destroy(set.gameObject));
    }

    public async UniTask<TextPanelSet> InstantiatePanelAsync(InteractableTMP tmp, TMP_WordInfo wordInfo, WordData data, int depth)
    {
        var panelSet = textPanelPool.Get();
        panelSet.InteractableTMP.TMP.text = data.description;

        await UniTask.NextFrame();

        panelSet.InteractableTMP.Refresh();
        panelSet.Refresh();

        var position = tmp.GetEnablePanelPosition(wordInfo, panelSet, defaultSpace);
        panelSet.Initialize(position, enableRoot, depth, onPointerEnterPanel, onPointerExitPanel);

        anchoringCTS = new CancellationTokenSource();
        var token = anchoringCTS.Token;
        AnchorPanelTimerAsync(panelSet, token).Forget();

        return panelSet;
    }

    public void DestroyPanel(TextPanelSet textPanelSet)
    {
        DisposeAnchoringCTS();

        textPanelSet.Release(disableRoot);
        textPanelPool.Release(textPanelSet);
    }

    public void DestroyPanel(List<TextPanelSet> textPanelSets)
    {
        DisposeDestroyingDelayCTS();
        DisposeAnchoringCTS();

        foreach (var textPanelSet in textPanelSets)
        {
            textPanelSet.Release(disableRoot);
            textPanelPool.Release(textPanelSet);
        }
    }

    public void DelayDestroyingAnchoredPanel(TextPanelSet panelSet, UnityAction onComplete)
    {
        DisposeDestroyingDelayCTS();
        destroyingDelayCTS = new CancellationTokenSource();
        var token = destroyingDelayCTS.Token;
        DestroyAnchordPanelAsync(panelSet, onComplete, token).Forget();
    }

    public void StopDestroyingAnchoredPanel()
    {
        DisposeDestroyingDelayCTS();
    }

    private async UniTask DestroyAnchordPanelAsync(TextPanelSet panelSet, UnityAction onComplete, CancellationToken token)
    {
        var duration = 0.0f;
        var targetDuration = 0.5f;
        try
        {
            while (duration < targetDuration)
            {
                token.ThrowIfCancellationRequested();

                duration += Time.deltaTime;
                await UniTask.Yield();
            }

            DestroyPanel(panelSet);
            onComplete?.Invoke();
        }
        catch (OperationCanceledException) { }
    }

    private async UniTask AnchorPanelTimerAsync(TextPanelSet textPanel, CancellationToken token)
    {
        var duration = 0.0f;
        var targetDuration = 1.0f;
        var t = duration / targetDuration;
        textPanel.FillImage.fillAmount = t;
        try
        {
            while (duration < targetDuration)
            {
                token.ThrowIfCancellationRequested();

                t = duration / targetDuration;
                textPanel.FillImage.fillAmount = t;

                duration += Time.deltaTime;
                await UniTask.Yield();
            }

            t = 1.0f;
            textPanel.FillImage.fillAmount = t;
            AnchorPanel(textPanel);
            DisposeAnchoringCTS();
        }
        catch (OperationCanceledException) { }
    }

    private void AnchorPanel(TextPanelSet textPanel)
    {
        textPanel.anchorState = TextPanelSet.AnchorState.AnchoredWithWord;
        textPanel.EnableOutline(true);
    }

    private void DisposeAnchoringCTS()
    {
        if (anchoringCTS != null)
        {
            anchoringCTS.Cancel();
            anchoringCTS.Dispose();
            anchoringCTS = null;
        }
    }

    private void DisposeDestroyingDelayCTS()
    {
        if (destroyingDelayCTS != null)
        {
            destroyingDelayCTS.Cancel();
            destroyingDelayCTS.Dispose();
            destroyingDelayCTS = null;
        }
    }
}
