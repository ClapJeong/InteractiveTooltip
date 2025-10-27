public enum TextPanelAnchorState
{
    None,
    WordAnchored,
    PanelAnchored,
}

public class TextPanelModel
{
    public readonly string key;
    public readonly string text;
    public readonly int depth;
    private TextPanelAnchorState anchorState;
    public TextPanelAnchorState AnchorState => anchorState;

    public TextPanelModel(LinkData linkData, int depth)
    {
        this.key = linkData.Key;
        this.text = linkData.Description;
        this.depth = depth;
        this.anchorState = TextPanelAnchorState.None;
    }

    public void SetAnchorState(TextPanelAnchorState anchorState)
    {
        this.anchorState = anchorState;
    }
}
