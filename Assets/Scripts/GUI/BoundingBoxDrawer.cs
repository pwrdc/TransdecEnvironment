using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class BoundingBoxDrawer : MonoBehaviour
{
    public TargetLocator targetLocator;
    RectTransform rectTransform;
    Image image;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (targetLocator.ScreenRect == Rect.zero)
        {
            image.enabled = false;
        }
        else
        {
            image.enabled = true;
            rectTransform.anchorMin = targetLocator.ScreenRect.min;
            rectTransform.anchorMax = targetLocator.ScreenRect.max;
        }
    }
}
