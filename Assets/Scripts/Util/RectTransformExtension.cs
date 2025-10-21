using UnityEngine;

public static class RectTransformExtension
{
    public static Vector2 GetAnchoredOffset(this RectTransform rectTransform, Vector2 anchorPosition, float offsetLength = 15.0f)
    {
        var widthHalf = rectTransform.rect.width;
        var heightHalf = rectTransform.rect.height;

        anchorPosition -= new Vector2(0.5f, 0.5f);

        var boundPosition = new Vector2(widthHalf * anchorPosition.x, heightHalf * anchorPosition.y);
        var defaultOffset = new Vector2(boundPosition.x == 0.0f ? 0.0f : offsetLength * Mathf.Sign(boundPosition.x), boundPosition.y == 0.0f ? 0.0f : offsetLength * Mathf.Sign(boundPosition.y));
        return boundPosition + defaultOffset;
    }
}
