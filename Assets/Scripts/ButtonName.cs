using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonName : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject hover;

    private void Start()
    {
        hover.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hover.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hover.SetActive(false);
    }
}
