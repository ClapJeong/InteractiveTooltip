using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InteractableTMPController
{
    private readonly UnityAction<InteractableTMP, TMP_WordInfo, int> onEnterWord;
    private readonly UnityAction<string> onLeaveWord;

    public InteractableTMPController(UnityAction<InteractableTMP, TMP_WordInfo, int> onEnterWord, UnityAction<string> onLeaveWord)
    {
        this.onEnterWord = onEnterWord;
        this.onLeaveWord = onLeaveWord;
    }

    public void UpdateDetecting(List<TextPanelSet> panelSets)
    {
        var mousePosition = Input.mousePosition;

        foreach (var panelSet in panelSets)
        {
            var interactableTMP = panelSet.InteractableTMP;
            switch (interactableTMP.pointingState)
            {
                case InteractableTMP.PointerState.Exit:
                    continue;

                case InteractableTMP.PointerState.OnExit:
                    {
                        if (panelSet.prevWordIndex != -1)
                        {
                            var lastWord = interactableTMP.TMP.textInfo.wordInfo[panelSet.prevWordIndex].GetWord();
                            LeaveMousePointWord(lastWord);
                            panelSet.prevWordIndex = -1;
                        }
                        interactableTMP.pointingState = InteractableTMP.PointerState.Exit;
                    }
                    continue;

                case InteractableTMP.PointerState.Enter:
                    {
                        var prevIndex = panelSet.prevWordIndex;
                        var wordIndex = TMP_TextUtilities.FindIntersectingWord(interactableTMP.TMP, mousePosition, null);

                        if (wordIndex != prevIndex)
                        {
                            if (wordIndex != -1)
                            {
                                if (prevIndex != -1)
                                {
                                    var prevWord = interactableTMP.TMP.textInfo.wordInfo[prevIndex].GetWord();
                                    LeaveMousePointWord(prevWord);
                                }

                                var targetWordInfo = interactableTMP.TMP.textInfo.wordInfo[wordIndex];
                                onEnterWord?.Invoke(interactableTMP, targetWordInfo, panelSet.deapth + 1);
                            }
                            else
                            {
                                var lastWord = interactableTMP.TMP.textInfo.wordInfo[prevIndex].GetWord();
                                LeaveMousePointWord(lastWord);
                            }
                            panelSet.prevWordIndex = wordIndex;
                        }
                    }
                    break;
            }
        }
    }

    private void LeaveMousePointWord(string key)
    {
        onLeaveWord?.Invoke(key);
    }
}
