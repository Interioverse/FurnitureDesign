using UnityEngine;
using UnityEngine.UI;

public class ScrollBar : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private RawImage rawImage;
    private Vector2 offset = Vector2.zero;

    private void Start()
    {
        rawImage = GetComponent<RawImage>();
    }

    private void Update()
    {
        if (!(Application.internetReachability == NetworkReachability.NotReachable))
        {
            offset.x -= Time.deltaTime * scrollSpeed; // Subtract offset for left-to-right scroll
            rawImage.uvRect = new Rect(offset, Vector2.one);
        }
    }
}

