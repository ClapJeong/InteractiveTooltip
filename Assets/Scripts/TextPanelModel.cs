public enum TextPanelAnchorState
{
    None,
    Anchoring,
    Anchored,
}

public class TextPanelModel
{
    public readonly string text;
    public readonly int depth;
    private TextPanelAnchorState anchorState;
    public TextPanelAnchorState AnchorState => anchorState;

    public TextPanelModel(string text, int depth)
    {
        this.text = text;
        this.depth = depth;
        this.anchorState = TextPanelAnchorState.None;
    }

    public void SetAnchorState(TextPanelAnchorState anchorState)
    {
        this.anchorState = anchorState;
    }
}
