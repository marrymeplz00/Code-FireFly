using UnityEngine;
using UnityEngine.EventSystems;

public class MapZoom : MonoBehaviour, IScrollHandler
{
    [Header("Map")]
    public RectTransform content;
    public RectTransform viewport;

    [Header("Zoom")]
    public float minZoom = 1f;
    public float maxZoom = 2.5f;
    public float zoomSpeed = 0.2f;

    private float currentZoom = 1f;

    public void OnScroll(PointerEventData eventData)
    {
        // เมาส์อยู่นอก Map ไม่ Zoom
        if (!RectTransformUtility.RectangleContainsScreenPoint(
            viewport, eventData.position, null))
        {
            return;
        }

        float scroll = eventData.scrollDelta.y;

        if (scroll == 0)
            return;

        float oldZoom = currentZoom;

        // คำนวณ Zoom ใหม่
        currentZoom += scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        if (Mathf.Approximately(oldZoom, currentZoom))
            return;

        // จำตำแหน่งเมาส์ใน Content ก่อน Zoom
        Vector2 mouseInContent;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(content, eventData.position, null, out mouseInContent);

        // Zoom
        content.localScale = Vector3.one * currentZoom;

        // หาตำแหน่งเมาส์ใน Content หลัง Zoom
        Vector2 mouseAfterZoom;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(content, eventData.position, null, out mouseAfterZoom);

        Vector2 difference = mouseAfterZoom - mouseInContent;

        content.anchoredPosition += difference * currentZoom;
    }
}