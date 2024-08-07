using UnityEngine;
using UnityEngine.EventSystems;

public class UIDraggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    [SerializeField] SmoothOrbitCam smoothOrbitCam;
    private Vector2 offset;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        smoothOrbitCam = FindObjectOfType<SmoothOrbitCam>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        smoothOrbitCam.isUIDragging = true;
        offset = (Vector2)rectTransform.position - eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        smoothOrbitCam.isUIDragging = false;
    }
}
