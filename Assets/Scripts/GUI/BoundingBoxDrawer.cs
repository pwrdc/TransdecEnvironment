using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class BoundingBoxDrawer : MonoBehaviour
{
    BoundingBox boundingBox;
    RectTransform rectTransform;
    Image image;

    void Start()
    {
        boundingBox = FindObjectOfType<BoundingBox>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (boundingBox.rect == Rect.zero)
        {
            image.enabled = false;
        }
        else
        {
            image.enabled = true;
            rectTransform.anchorMin = boundingBox.rect.min;
            rectTransform.anchorMax = boundingBox.rect.max;
        }
    }
}
