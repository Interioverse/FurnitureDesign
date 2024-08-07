using System;
using UnityEngine;
using UnityEngine.UI;

public class Acknowledgment : MonoBehaviour
{
    public GameObject acknlm;
    public ScrollRect scrollRect;
    public Button acknowledge, back;

    private const string AcknowledgmentKey = "AcknowledgmentStatus";

    private void Start()
    {
        GetTheAcknowledgmentStatus();

        back.onClick.AddListener(() => BackToShowRoom());
        acknowledge.onClick.AddListener(() => SetAcknowledgmentStatus(true));
        //scrollRect.onValueChanged.AddListener(OnScrollViewValueChanged);
    }

    private void BackToShowRoom()
    {
        UniversalData.Instance.LoadMainScene();
    }

    private void GetTheAcknowledgmentStatus()
    {
        bool acknowledgmentStatus = PlayerPrefs.GetInt(AcknowledgmentKey, 0) == 1;
        if (!acknowledgmentStatus)
        {
            acknlm.SetActive(true);
        }
        else
        {
            acknlm.SetActive(false);
        }
    }

    public void SetAcknowledgmentStatus(bool status)
    {
        int statusValue = status ? 1 : 0;
        PlayerPrefs.SetInt(AcknowledgmentKey, statusValue);
        PlayerPrefs.Save();
        acknlm.SetActive(false);
    }

    //private void OnScrollViewValueChanged(Vector2 value)
    //{
    //    if (value.y <= 0.0f) // Check if the scroll view is scrolled to the bottom
    //    {
    //        acknowledge.gameObject.SetActive(true);
    //    }
    //}
}

