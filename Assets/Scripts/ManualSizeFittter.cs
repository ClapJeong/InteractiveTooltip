using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ContentSizeFitter))]
public class ManualSizeFittter : MonoBehaviour
{
    private RectTransform rectTransform;
    private ContentSizeFitter contentSizeFitter;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        contentSizeFitter = GetComponent<ContentSizeFitter>();
        contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        contentSizeFitter.enabled = false;
    }

    public void Refresh()
    {
        RefreshAsync().Forget();
    }

    private async UniTask RefreshAsync()
    {
        contentSizeFitter.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        await UniTask.NextFrame();
        contentSizeFitter.enabled = false;
    }
}
