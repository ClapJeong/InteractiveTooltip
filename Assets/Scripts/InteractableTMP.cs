using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TextMeshProUGUI))]
public class InteractableTMP : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum PointerState
    {
        Exit,
        OnExit,
        Enter,
    }

    private TextMeshProUGUI tmp;
    public TextMeshProUGUI TMP => tmp;

    [SerializeField] private RectTransform rect;
    [SerializeField] private ManualSizeFittter manualSizeFittter;

    public PointerState pointingState = PointerState.Exit;

    private void Awake()
    {
        if (tmp == null)
            tmp = GetComponent<TextMeshProUGUI>();
        if (rect == null)
            rect = GetComponent<RectTransform>();
    }

    public void Refresh()
    {
        manualSizeFittter.Refresh();
    }

    public Vector2 GetEnablePanelPosition(TMP_WordInfo wordInfo, TextPanelSet panelSet, float defaultSpace)
    {
        var panelWidth = panelSet.rectTransform.rect.width;
        var panelHeight = panelSet.rectTransform.rect.height;

        var screenWidth = Screen.width;
        var screenHeight = Screen.height;

        var directions = new List<TextPanelDirection>() { TextPanelDirection.TopRight, TextPanelDirection.BottomRight, TextPanelDirection.BottomLeft, TextPanelDirection.TopLeft, };
        foreach (var direction in directions)
        {
            var wordBeginDirection = (int)direction < 10 ? TextPanelDirection.TopCenter : TextPanelDirection.BottomCenter;
            var wordPosition = GetWordPosition(wordInfo, wordBeginDirection);
            var targetScreenPoint = RectTransformUtility.WorldToScreenPoint(null, TMP.transform.TransformPoint(wordPosition));

            var maxPoint = targetScreenPoint + direction switch
            {
                TextPanelDirection.TopLeft => new Vector2(-panelWidth, panelHeight),
                TextPanelDirection.TopRight => new Vector2(panelWidth, panelHeight),
                TextPanelDirection.BottomRight => new Vector2(panelWidth, -panelHeight),
                TextPanelDirection.BottomLeft => new Vector2(-panelWidth, -panelHeight),
                _ => Vector2.zero
            };

            if (defaultSpace <= maxPoint.x && maxPoint.x <= screenWidth - defaultSpace &&
                defaultSpace < maxPoint.y && maxPoint.y <= screenHeight - defaultSpace)
                return targetScreenPoint + panelSet.GetRectSizeOffset(direction, defaultSpace);
        }

        return Vector2.zero;
    }

    public Vector3 GetCharPosition(TMP_CharacterInfo info, TextPanelDirection direction)
        => direction switch
        {
            TextPanelDirection.TopLeft => info.topLeft,
            TextPanelDirection.TopCenter => (info.topLeft + info.topRight) * 0.5f,
            TextPanelDirection.TopRight => info.topRight,
            TextPanelDirection.Left => (info.topLeft + info.bottomLeft) * 0.5f,
            TextPanelDirection.Right => (info.topRight + info.bottomRight) * 0.5f,
            TextPanelDirection.BottomLeft => info.bottomLeft,
            TextPanelDirection.BottomCenter => (info.bottomLeft + info.bottomRight) * 0.5f,
            TextPanelDirection.BottomRight => info.bottomRight,
            _ => Vector3.zero,
        };

    public Vector3 GetCharPosition(int charIndex, TextPanelDirection direction)
    {
        var charInfo = TMP.textInfo.characterInfo[charIndex];
        return GetCharPosition(charInfo, direction);
    }

    public Vector3 GetWordPosition(TMP_WordInfo wordInfo, TextPanelDirection direction)
    {
        var firstCharInfo = TMP.textInfo.characterInfo[wordInfo.firstCharacterIndex];
        var lastCharInfo = TMP.textInfo.characterInfo[wordInfo.lastCharacterIndex];
        return direction switch
        {
            TextPanelDirection.TopLeft => firstCharInfo.topLeft,
            TextPanelDirection.TopCenter => (firstCharInfo.topLeft + lastCharInfo.topRight) * 0.5f,
            TextPanelDirection.TopRight => lastCharInfo.topRight,
            TextPanelDirection.Left => (firstCharInfo.topLeft + firstCharInfo.bottomLeft) * 0.5f,
            TextPanelDirection.Right => (lastCharInfo.topRight + lastCharInfo.bottomRight) * 0.5f,
            TextPanelDirection.BottomLeft => firstCharInfo.bottomLeft,
            TextPanelDirection.BottomCenter => (firstCharInfo.bottomLeft + lastCharInfo.bottomRight) * 0.5f,
            TextPanelDirection.BottomRight => lastCharInfo.bottomRight,
            _ => Vector3.zero,
        };
    }

    public Vector3 GetWordPosition(int wordIndex, TextPanelDirection direction)
    {
        var wordInfo = TMP.textInfo.wordInfo[wordIndex];
        return GetWordPosition(wordInfo, direction);
    }

    public void OnPointerExit(PointerEventData eventData)
        => pointingState = PointerState.OnExit;

    public void OnPointerEnter(PointerEventData eventData)
        => pointingState = PointerState.Enter;
}
