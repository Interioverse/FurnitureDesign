using UnityEngine;
using UnityEngine.UI;

public class TogglePivot : MonoBehaviour
{
    public RectTransform rectTransform1, rectTransform2;
    public Button toggleBtn1, toggleBtn2;

    private void Start()
    {
        // Initialize the position to middle-right
        toggleBtn1.onClick.AddListener(() => TogglePosition(false));
        toggleBtn2.onClick.AddListener(() => TogglePosition(true));
    }

    public void TogglePosition(bool value)
    {
        rectTransform1.gameObject.SetActive(value);
        rectTransform2.gameObject.SetActive(!value);
    }
}