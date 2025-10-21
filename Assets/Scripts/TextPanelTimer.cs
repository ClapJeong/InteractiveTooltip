using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class TextPanelTimer
{
    private class TimerSet : IDisposable
    {
        private CancellationTokenSource cts;
        public float remainDuration;

        public TimerSet(CancellationTokenSource cts, float duration)
        {
            this.cts = cts;
            remainDuration = duration;
        }

        public void Dispose()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
                cts = null;
            }
        }
    }

    private readonly Dictionary<ITextPanelPresenter, TimerSet> workingTimers = new();

    public TextPanelTimer()
    {

    }

    public bool HasTimer(ITextPanelPresenter presenter)
        => workingTimers.ContainsKey(presenter);

    public void CancelTimer(ITextPanelPresenter presenter)
    {
        workingTimers[presenter].Dispose();
    }

    private void RemovePresenter(ITextPanelPresenter presenter)
    {
        workingTimers.Remove(presenter);
    }

    public void PlayTimer(ITextPanelPresenter presenter,
        float duration,
        UnityAction<float> onProgress,
        UnityAction onComplete,
        UnityAction onCanceled)
    {
        var cts = new CancellationTokenSource();
        var token = cts.Token;
        var timerSet = new TimerSet(cts, duration);
        workingTimers.Add(presenter, timerSet);

        PlayTimerAsync(
            presenter,
            token,
            timerSet,
            onProgress,
            onComplete,
            onCanceled).Forget();
    }

    private async UniTask PlayTimerAsync(
        ITextPanelPresenter presenter,
        CancellationToken token,
        TimerSet timerSet,
        UnityAction<float> onProgress,
        UnityAction onComplete,
        UnityAction onCanceled)
    {
        var beginDuration = timerSet.remainDuration;
        try
        {
            while (timerSet.remainDuration > 0.0f)
            {
                token.ThrowIfCancellationRequested();

                timerSet.remainDuration -= Time.deltaTime;
                onProgress?.Invoke(1.0f - (timerSet.remainDuration / beginDuration));
                await UniTask.Yield();
            }
            onProgress?.Invoke(1.0f);
            onComplete?.Invoke();
        }
        catch (OperationCanceledException)
        {
            onCanceled?.Invoke();
        }

        workingTimers[presenter].Dispose();
        RemovePresenter(presenter);
    }
}
