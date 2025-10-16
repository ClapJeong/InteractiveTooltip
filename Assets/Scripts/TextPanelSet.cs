using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TextPanelSet : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum AnchorState
    {
        None,
        AnchoredWithWord,
        AnchoredWithPanel
    }
    [SerializeField] private Outline outLine;

    private InteractableTMP interactableTMP;
    public InteractableTMP InteractableTMP => interactableTMP;

    private RectTransform rect;
    public RectTransform rectTransform => rect;

    [SerializeField] private Image fillImage;
    public Image FillImage => fillImage;

    [SerializeField] private ManualSizeFittter manualSizeFittter;

    private UnityAction<TextPanelSet> onPointerEnter;
    private UnityAction<TextPanelSet> onPointerExit;

    public AnchorState anchorState = AnchorState.None;
    public int deapth = 0;
    public int prevWordIndex = -1;

    private void Awake()
    {
        if (interactableTMP == null)
            interactableTMP = GetComponentInChildren<InteractableTMP>();
        if (rect == null)
            rect = GetComponent<RectTransform>();
        if (manualSizeFittter == null)
            manualSizeFittter = GetComponent<ManualSizeFittter>();
    }

    private void Start()
        => TooptipManager.instance.Register(this);

    private void OnDestroy()
        => TooptipManager.instance.Unregister(this);

    public void Refresh()
        => manualSizeFittter.Refresh();

    public void Initialize(Vector2 position, Transform enableRoot, int deapth, UnityAction<TextPanelSet> onPointerEnter, UnityAction<TextPanelSet> onPointerExit)
    {
        transform.position = position;
        transform.SetParent(enableRoot);
        anchorState = AnchorState.None;
        EnableOutline(false);
        this.deapth = deapth;
        this.onPointerEnter = onPointerEnter;
        this.onPointerExit = onPointerExit;
    }

    public void Release(Transform disablrRoot)
    {
        transform.SetParent(disablrRoot);
        prevWordIndex = -1;
    }

    public void EnableOutline(bool isEnable)
        => outLine.enabled = isEnable;

    public Vector2 GetRectSizeOffset(TextPanelDirection direction, float defaultOffsetLength = 0.0f)
    {
        var width = rect.sizeDelta.x + defaultOffsetLength * 2.0f;
        var height = rect.sizeDelta.y + defaultOffsetLength * 2.0f;
        return direction switch
        {
            TextPanelDirection.TopLeft => new Vector2(-width, height) * 0.5f,
            TextPanelDirection.TopCenter => new Vector2(0.0f, height) * 0.5f,
            TextPanelDirection.TopRight => new Vector2(width, height) * 0.5f,
            TextPanelDirection.Left => new Vector2(-width, 0.0f) * 0.5f,
            TextPanelDirection.Right => new Vector2(width, 0.0f) * 0.5f,
            TextPanelDirection.BottomLeft => new Vector2(-width, -height) * 0.5f,
            TextPanelDirection.BottomCenter => new Vector2(0.0f, -height) * 0.5f,
            TextPanelDirection.BottomRight => new Vector2(width, -height) * 0.5f,
            _ => (Vector2)Vector3.zero,
        };
    }

    public void OnPointerEnter(PointerEventData eventData)
        => onPointerEnter?.Invoke(this);

    public void OnPointerExit(PointerEventData eventData)
        => onPointerExit?.Invoke(this);
}
