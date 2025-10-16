using UnityEngine;

public class UILehu : MonoBehaviour
{
    const string mainText = "Hello World <color=green>Lehu</color>";

    [SerializeField] private TextPanelSet defaultTextPanel;

    private void Start()
    {
        defaultTextPanel.InteractableTMP.TMP.text = mainText;
        defaultTextPanel.InteractableTMP.Refresh();
        defaultTextPanel.Refresh();
    }
}
