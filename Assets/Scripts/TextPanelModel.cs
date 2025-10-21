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

    public TextPanelModel(string key, string text, int depth)
    {
        this.key = key;
        this.text = text;
        this.depth = depth;
        this.anchorState = TextPanelAnchorState.None;
    }

    public void SetAnchorState(TextPanelAnchorState anchorState)
    {
        this.anchorState = anchorState;
    }
}
