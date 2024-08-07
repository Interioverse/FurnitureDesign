using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text errorText;
    public GameObject errorPanel;

    #region Singleton

    public static UIManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion


    public void AccessErrorMessage(string Message)
    {

        errorText.text = Message;
        errorPanel.gameObject.SetActive(true);
        Invoke("DisableMessage", 3f);
    }
    void DisableMessage()
    {
        errorPanel.gameObject.SetActive(false);
    }
}
