using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public static class TMPExtension
{
    public static Vector2 GetWordPosition(this TextMeshProUGUI tmp, int firstIndex, int lastIndex, TextDirection direction)
    {
        var charInfo = tmp.textInfo.characterInfo;

        var visibleChars = new List<TMP_CharacterInfo>();
        for (int i = firstIndex; i <= lastIndex; i++)
        {
            if (i < 0 || i >= charInfo.Length) continue;
            if (charInfo[i].isVisible)
                visibleChars.Add(charInfo[i]);
        }

        if (visibleChars.Count == 0)
            return Vector2.zero;

        float xMid = (visibleChars.First().bottomLeft.x + visibleChars.Last().topRight.x) * 0.5f;
        float y = direction switch
        {
            TextDirection.Up => visibleChars.Max(c => c.topLeft.y),
            TextDirection.Down => visibleChars.Min(c => c.bottomLeft.y),
            _ => throw new System.NotImplementedException()
        };

        Vector3 localPosition = new Vector3(xMid, y, 0f);
        Vector3 worldPosition = tmp.transform.TransformPoint(localPosition);
        Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(null, worldPosition);

        return screenPosition;
    }
}
